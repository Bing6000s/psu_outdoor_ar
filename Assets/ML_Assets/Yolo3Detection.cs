using System;
using UnityEngine;
using Unity.Barracuda;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Yolo3Detection : MonoBehaviour
{

    [Header("References")]
    public NNModel modelFile; // ONNX model file ref
    public TextAsset labelsFile; // Text file with class labels ref
    public Image outputImage; // Camera output image, should be webcam texture
    public GameObject boundingBoxPrefab; // Prefab for UI bounding box to render
    public Transform canvas; // Parent transform for canvas
    public RectTransform outputCamera; // Output camera transform, referenced for size
    public TMPro.TextMeshProUGUI runCounterText; // Text for debug run counting

    private const int IMAGE_MEAN = 0; // Mean pixel value
    private const float IMAGE_STD = 255.0F; // Std. Dev on pixel values

    [Header("Model Config")]
    public string INPUT_NAME; // Name of input to first layer in model, 000_net on yolo3
    public string OUTPUT_NAME_L; // Yolo3 first output name, 016_convolutional on yolo3
    public string OUTPUT_NAME_M; // Yolo3 second output name, 023_convolutional on yolo3

    private const int _image_size = 416; // Size to scale image down to
    public int IMAGE_SIZE { get => _image_size; }

    [Header("Results Config")]
    public bool enableWhitelist = true;
    public List<string> whitelist = new List<string>{"apple", "banana"};
    public float MINIMUM_CONFIDENCE = 0.25f; // Minimum detection confidence to track a detection

    // public const int ROW_COUNT_L = 13;
    // public const int COL_COUNT_L = 13;
    // public const int ROW_COUNT_M = 26;
    // public const int COL_COUNT_M = 26;

    public Dictionary<string, int> params_l = new Dictionary<string, int>(){{"ROW_COUNT", 13}, {"COL_COUNT", 13}, {"CELL_WIDTH", 32}, {"CELL_HEIGHT", 32}};
    public Dictionary<string, int> params_m = new Dictionary<string, int>(){{"ROW_COUNT", 26}, {"COL_COUNT", 26}, {"CELL_WIDTH", 16}, {"CELL_HEIGHT", 16}};
    public const int BOXES_PER_CELL = 3; // Number of boxes detected in each YOLO cell
    public const int BOX_INFO_FEATURE_COUNT = 5; // Feature count for each box

    // Count of total classes to recognize, depending on model, 80 for yolo3 tiny
    public int CLASS_COUNT;

    // Local vars
    private IWorker worker; // Worker to run model via Barracuda
    private int runCounter = 0; // Count of times run detection
    private bool running; // Whether detection is running
    private List<GameObject> boxes; // List of UI bounding box objects
    private Preprocess thisPreprocessor; // Reference to attached preprocessor component

    // public const float CELL_WIDTH_L = 32;
    // public const float CELL_HEIGHT_L = 32;
    // public const float CELL_WIDTH_M = 16;
    // public const float CELL_HEIGHT_M = 16;

    // String of read labels
    private string[] labels;

    // Anchors are necessary points depending on YOLO version to interpret output
    // Can generally reference online sources for different versions
    private float[] anchors = new float[]
    {
        10F, 14F,  23F, 27F,  37F, 58F,  81F, 82F,  135F, 169F,  344F, 319F // yolov3-tiny
    };

        

    // Function to reset running on suspended
    public void ResetRunning(){
        this.running = false;
    }

    // Function to clear bounding boxes
    public void ClearBoxes(){
        foreach(GameObject boxObj in boxes){
            Destroy(boxObj);
        }
        boxes.Clear();
    }

    // Start is run by Unity on component init
    public void Start()
    {
        // Init run counter and bounding box list
        this.runCounter = 0;
        this.boxes = new List<GameObject>();

        // Get attached preprocessor component
        this.thisPreprocessor = gameObject.GetComponent<Preprocess>();

        // Set not running
        this.running = false;

        // Load labels, model file, init Barracuda worker
        this.labels = Regex.Split(this.labelsFile.text, "\n|\r|\r\n")
            .Where(s => !String.IsNullOrEmpty(s)).ToArray();
        var model = ModelLoader.Load(this.modelFile);
        // https://docs.unity3d.com/Packages/com.unity.barracuda@1.0/manual/Worker.html
        //These checks all check for GPU before CPU as GPU is preferred if the platform + rendering pipeline support it
        this.worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
    }

    // Function to update run counter GUI text with new count
    void UpdateRunGUI(int runs){
        runCounterText.text = "" + runs;
    }

    // Update is run by Unity at each time step
    public void Update(){

        // Continuously run
        if(!this.running){
            this.runCounter += 1;
            UpdateRunGUI(this.runCounter);

            this.running = true;
            BeginInference();
        }
        
    }

    // Function to begin inference process with pre-processing on WCT
    void BeginInference(){
        // Grab WCT
        WebCamTexture thisWCT = getWebcameTextureRaw();

        // Send Web cam texture to preprocessing, call detection on result
        StartCoroutine(thisPreprocessor.ProcessImage(thisWCT, _image_size, result =>
        {
            StartCoroutine(Detect(result));
        }));
    }

    // Function to just return WebCamTexture
    WebCamTexture getWebcameTextureRaw(){
        return outputImage.material.mainTexture as WebCamTexture;
    }


    // Function to run YOLO detection on some Color32 arr
    public IEnumerator Detect(Color32[] picture)
    {
        // Normalize input
        using (var tensor = TransformInput(picture, IMAGE_SIZE, IMAGE_SIZE))
        {
            // Create input tensor, send to model for execution
            var inputs = new Dictionary<string, Tensor>();
            inputs.Add(INPUT_NAME, tensor);
            yield return StartCoroutine(worker.StartManualSchedule(inputs));

            // If not a coroutine, can also call model via Execute():
            //worker.Execute(inputs);

            // Get model output
            var output_l = worker.PeekOutput(OUTPUT_NAME_L);
            var output_m = worker.PeekOutput(OUTPUT_NAME_M);
           
            // Parse outputs and concatenate
            var results_l = ParseOutputs(output_l, MINIMUM_CONFIDENCE, params_l);
            var results_m = ParseOutputs(output_m, MINIMUM_CONFIDENCE, params_m);
            var results = results_l.Concat(results_m).ToList();

            // Filter down to relevant bounding boxes 
            var boxes = FilterBoundingBoxes(results, 5, MINIMUM_CONFIDENCE);

            var filteredResults = from box in boxes
                                    where whitelist.Contains((box.Label).ToString())
                                    select box;
            // Clear UI
            ClearBoxes();

            if (!enableWhitelist)
                filteredResults = boxes;
            
            // Render each bounding box and console log results
            foreach(BoundingBox box in filteredResults){
                RenderBoundingBox(box.Dimensions.X,box.Dimensions.Y,box.Dimensions.Width,box.Dimensions.Height, box.Label);
                Debug.Log("aaaaa" + box.ToString());

                // Notify UI of label
                LabelsController.labelController.FoundLabel(box.Label);
            }

            // Stop run to restart
            this.running = false;
        }
    }

    // Function to render a bounding box
    void RenderBoundingBox(float x, float y, float width, float height, string label){
        // Instantiate unity gameobject based on prefab
        GameObject newBox = Instantiate(boundingBoxPrefab, canvas);
        boxes.Add(newBox);
        
        // Set label
        Text thisText = newBox.GetComponentsInChildren<Text>()[0];
        thisText.text = label;

        // Save output size
        float outputSize = (float)_image_size;

        // Get webcam size
        float wct_width = outputCamera.rect.width;
        float wct_height = outputCamera.rect.height;
        
        // Save larger image size to blowup to
        float offset = 0;
        float largerSq = 0;
        bool offsetOnWidth = false;

        // Handle width/height being cropped
        if(wct_height > wct_width){
            offset = (wct_height - wct_width) / 2F;
            largerSq = wct_width;
            offsetOnWidth = false;
        }
        else{
            offset = (wct_width - wct_height) / 2F;
            largerSq = wct_height;
            offsetOnWidth = true;
        }

        // Compute new x,y
        float newx = x / outputSize;
        newx = newx * largerSq;
        if(offsetOnWidth){
            newx = newx + offset;
        }

        float newy = y / outputSize;
        newy = -(newy * largerSq);
        if(!offsetOnWidth){
            newy = newy - offset;
        }

        // Compute new width, height
        float newwidth = (width / outputSize) * largerSq;
        float newheight = (height / outputSize) * largerSq;

        // Debug.Log("Using WCT dimensions " + wct_width + " , " + wct_height);

        // Set box transform
        RectTransform boxTransform = newBox.GetComponent<RectTransform>();

        // Update box position and size
        boxTransform.anchoredPosition = new Vector2(newx, newy);
        boxTransform.sizeDelta = new Vector2(newwidth, newheight);

    }

    // Function to normalize input data
    public static Tensor TransformInput(Color32[] pic, int width, int height)
    {
        float[] floatValues = new float[width * height * 3];

        for (int i = 0; i < pic.Length; ++i)
        {
            var color = pic[i];

            floatValues[i * 3 + 0] = (color.r - IMAGE_MEAN) / IMAGE_STD;
            floatValues[i * 3 + 1] = (color.g - IMAGE_MEAN) / IMAGE_STD;
            floatValues[i * 3 + 2] = (color.b - IMAGE_MEAN) / IMAGE_STD;
        }

        return new Tensor(1, height, width, 3, floatValues);
    }

    // Function to parse model outputs
    private IList<BoundingBox> ParseOutputs(Tensor yoloModelOutput, float threshold, Dictionary<string, int> parameters)
    {
        var boxes = new List<BoundingBox>();

        // iterate each row and col processed by YOLO and evaluate boxes identified in each
        for (int cy = 0; cy < parameters["COL_COUNT"]; cy++)
        {
            for (int cx = 0; cx < parameters["ROW_COUNT"]; cx++)
            {
                for (int box = 0; box < BOXES_PER_CELL; box++)
                {
                    // Get results for each box
                    // Note - this is based on the Yolo spec, can consult Unity docs and ONNX model zoo docs for more details
                    var channel = (box * (CLASS_COUNT + BOX_INFO_FEATURE_COUNT));
                    var bbd = ExtractBoundingBoxDimensions(yoloModelOutput, cx, cy, channel);
                    float confidence = GetConfidence(yoloModelOutput, cx, cy, channel);

                    if (confidence < threshold)
                    {
                        continue;
                    }

                    // Extract top classification result within that detected box
                    float[] predictedClasses = ExtractClasses(yoloModelOutput, cx, cy, channel);
                    var (topResultIndex, topResultScore) = GetTopResult(predictedClasses);
                    var topScore = topResultScore * confidence;
                    Debug.Log("DEBUG: results: " + topResultIndex.ToString());

                    if (topScore < threshold)
                    {
                        continue;
                    }

                    // Map bounding box to appropriate region of overall picture
                    var mappedBoundingBox = MapBoundingBoxToCell(cx, cy, box, bbd, parameters);
                    boxes.Add(new BoundingBox
                    {
                        Dimensions = new BoundingBoxDimensions
                        {
                            X = (mappedBoundingBox.X - mappedBoundingBox.Width / 2),
                            Y = (mappedBoundingBox.Y - mappedBoundingBox.Height / 2),
                            Width = mappedBoundingBox.Width,
                            Height = mappedBoundingBox.Height,
                        },
                        Confidence = topScore,
                        Label = labels[topResultIndex]
                    });
                }
            }
        }

        return boxes;
    }

    // Sigmoid function implementation
    private float Sigmoid(float value)
    {
        var k = (float)Math.Exp(value);

        return k / (1.0f + k);
    }

    // Softmax function implementation
    private float[] Softmax(float[] values)
    {
        var maxVal = values.Max();
        var exp = values.Select(v => Math.Exp(v - maxVal));
        var sumExp = exp.Sum();

        return exp.Select(v => (float)(v / sumExp)).ToArray();
    }

    // Function to extract the dimensions of some bounding box, by position, from the output tensor
    private BoundingBoxDimensions ExtractBoundingBoxDimensions(Tensor modelOutput, int x, int y, int channel)
    {
        return new BoundingBoxDimensions
        {
            X = modelOutput[0, x, y, channel],
            Y = modelOutput[0, x, y, channel + 1],
            Width = modelOutput[0, x, y, channel + 2],
            Height = modelOutput[0, x, y, channel + 3]
        };
    }

    // Function to get the confidence at some position from model output
    private float GetConfidence(Tensor modelOutput, int x, int y, int channel)
    {
        //Debug.Log("ModelOutput " + modelOutput);
        return Sigmoid(modelOutput[0, x, y, channel + 4]);
    }

    // Function to map a model output bounding box to its corresponding cell
    private CellDimensions MapBoundingBoxToCell(int x, int y, int box, BoundingBoxDimensions boxDimensions, Dictionary<string, int> parameters)
    {
        return new CellDimensions
        {
            X = ((float)y + Sigmoid(boxDimensions.X)) * parameters["CELL_WIDTH"],
            Y = ((float)x + Sigmoid(boxDimensions.Y)) * parameters["CELL_HEIGHT"],
            Width = (float)Math.Exp(boxDimensions.Width) * anchors[6 + box * 2],
            Height = (float)Math.Exp(boxDimensions.Height) * anchors[6 + box * 2 + 1],
        };
    }

    // Function to extract the prediction values for each of N classes from some position in the YOLO output
    public float[] ExtractClasses(Tensor modelOutput, int x, int y, int channel)
    {
        float[] predictedClasses = new float[CLASS_COUNT];
        int predictedClassOffset = channel + BOX_INFO_FEATURE_COUNT;

        for (int predictedClass = 0; predictedClass < CLASS_COUNT; predictedClass++)
        {
            predictedClasses[predictedClass] = modelOutput[0, x, y, predictedClass + predictedClassOffset];
        }

        return Softmax(predictedClasses);
    }

    // Function to get the top result from a list of conf values for each class
    private ValueTuple<int, float> GetTopResult(float[] predictedClasses)
    {
        return predictedClasses
            .Select((predictedClass, index) => (Index: index, Value: predictedClass))
            .OrderByDescending(result => result.Value)
            .First();
    }

    // Function to join intersecting boxes 
    private float IntersectionOverUnion(Rect boundingBoxA, Rect boundingBoxB)
    {
        var areaA = boundingBoxA.width * boundingBoxA.height;

        if (areaA <= 0)
            return 0;

        var areaB = boundingBoxB.width * boundingBoxB.height;

        if (areaB <= 0)
            return 0;

        var minX = Math.Max(boundingBoxA.xMin, boundingBoxB.xMin);
        var minY = Math.Max(boundingBoxA.yMin, boundingBoxB.yMin);
        var maxX = Math.Min(boundingBoxA.xMax, boundingBoxB.xMax);
        var maxY = Math.Min(boundingBoxA.yMax, boundingBoxB.yMax);

        var intersectionArea = Math.Max(maxY - minY, 0) * Math.Max(maxX - minX, 0);

        return intersectionArea / (areaA + areaB - intersectionArea);
    }

    // Function to filter down boxes not matching the desired confidence threshold, and down to a certain number
    private IList<BoundingBox> FilterBoundingBoxes(IList<BoundingBox> boxes, int limit, float threshold)
    {
        var activeCount = boxes.Count;
        var isActiveBoxes = new bool[boxes.Count];

        for (int i = 0; i < isActiveBoxes.Length; i++)
        {
            isActiveBoxes[i] = true;
        }

        var sortedBoxes = boxes.Select((b, i) => new { Box = b, Index = i })
                .OrderByDescending(b => b.Box.Confidence)
                .ToList();

        var results = new List<BoundingBox>();

        for (int i = 0; i < boxes.Count; i++)
        {
            if (isActiveBoxes[i])
            {
                var boxA = sortedBoxes[i].Box;
                results.Add(boxA);

                if (results.Count >= limit)
                    break;

                for (var j = i + 1; j < boxes.Count; j++)
                {
                    if (isActiveBoxes[j])
                    {
                        var boxB = sortedBoxes[j].Box;

                        if (IntersectionOverUnion(boxA.Rect, boxB.Rect) > threshold)
                        {
                            isActiveBoxes[j] = false;
                            activeCount--;

                            if (activeCount <= 0)
                                break;
                        }
                    }
                }

                if (activeCount <= 0)
                    break;
            }
        }
        return results;
    }
    public void ReleaseResources()
    {
        // Release the Barracuda worker
        if (worker != null)
        {
            worker.Dispose();
            worker = null;
        }

        // Clear and destroy all bounding box GameObjects
        if (boxes != null)
        {
            foreach (GameObject box in boxes)
            {
                if (box != null)
                {
                    Destroy(box);
                }
            }
            boxes.Clear();
        }

        // Optional: Clear other references
        labels = null;
        thisPreprocessor = null;

        // Optionally unload unused assets to free memory
        Resources.UnloadUnusedAssets();
        System.GC.Collect(); // Trigger garbage collection
    }

    // Ensure ReleaseResources is called when this object is destroyed
    private void OnDestroy()
    {
        ReleaseResources();
    }
}