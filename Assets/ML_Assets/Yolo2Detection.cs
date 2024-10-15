using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Barracuda;
using System.Linq;
using System.Text.RegularExpressions;
using System;

// Classes for processing YOLO model results
public class DimensionsBase
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Height { get; set; }
    public float Width { get; set; }
}

public class BoundingBoxDimensions : DimensionsBase { }

class CellDimensions : DimensionsBase { }

// Class storing bounding box data 
public class BoundingBox
{
    public BoundingBoxDimensions Dimensions { get; set; }

    public string Label { get; set; }

    public float Confidence { get; set; }

    public Rect Rect
    {
        get { return new Rect(Dimensions.X, Dimensions.Y, Dimensions.Width, Dimensions.Height); }
    }

    public override string ToString()
    {
        return $"{Label}:{Confidence}, {Dimensions.X}:{Dimensions.Y} - {Dimensions.Width}:{Dimensions.Height}";
    }
}

/**

This is kept for reference only - we now use Yolo3 via Yolo3Detection.

*/

public class RunDetection : MonoBehaviour
{

    // References
    public Image outputImage;
    public NNModel tinyYoloModel;
    public GameObject boundingBoxPrefab;
    public Transform canvas;
    public RectTransform outputCamera;

    public int cam_crop_size = 416;

    // Local Vars
    private int count;
    private Model runtimeModel;
    private IWorker barracudaWorker;
    private Preprocess thisPreprocessor;
    private List<GameObject> boxes;

    private const int IMAGE_MEAN = 0;
    private const float IMAGE_STD = 1f;
    private const string INPUT_NAME = "image";
    private const string OUTPUT_NAME = "grid";

    public const int ROW_COUNT = 13;
    public const int COL_COUNT = 13;
    public const int BOXES_PER_CELL = 5;
    public const int BOX_INFO_FEATURE_COUNT = 5;
    public const int CLASS_COUNT = 20;
    public const float CELL_WIDTH = 32;
    public const float CELL_HEIGHT = 32;

    // Labels for Tiny YOLO v2
    string[] labels = {
        "aeroplane", "bicycle", "bird", "boat", "bottle", "bus", "car", "cat",
        "chair", "cow", "diningtable", "dog", "horse", "motorbike", "person",
        "pottedplant", "sheep", "sofa", "train", "tvmonitor"
    };

    // Anchor points for Tiny YOLO v2
    //float[] anchors = {1.08F, 1.19F, 3.42F, 4.41F, 6.63F, 11.38F, 9.42F, 5.11F, 16.62F, 10.52F};

    // Anchor points for Tiny YOLO v2 Food
    float[] anchors = { 0.57273F, 0.677385F, 1.87446F, 2.06253F, 3.33843F, 5.47434F, 7.88282F, 3.52778F, 9.77052F, 9.16828F };

    void Start(){

        count = 0;

        boxes = new List<GameObject>();

        // Load Tiny Yolo v2 ONNX model to runtime
        runtimeModel = ModelLoader.Load(tinyYoloModel);

        // Setup worker via Barracuda
        barracudaWorker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, runtimeModel);

        // Grab preprocessor component
        thisPreprocessor = gameObject.GetComponent<Preprocess>();

    }

    // Function to compute offset from YOLO data
    // Referencing https://github.com/hollance/Forge/blob/master/Examples/YOLO/YOLO/YOLO.swift#L99 
    int getYoloTensorOffset(int channel, int x, int y){
        int slice = channel / 4;
        int index = channel - slice * 4;
        return (slice * 13 * 13 * 4) + (y * 13 * 4) + (x * 4) + index;
    }

    // Function to get the 2D Texture from the pixels of the webcam
    // Per https://forum.unity.com/threads/webcamtexture-texture2d.154057/
    Texture2D getTextureFromWebcam(){
        WebCamTexture webcamTexture = outputImage.material.mainTexture as WebCamTexture;
        Texture2D tx2d = new Texture2D(webcamTexture.width, webcamTexture.height);
        tx2d.SetPixels(webcamTexture.GetPixels());
        tx2d.Apply();
        return tx2d;
    }

    // Function to just return WebCamTexture
    WebCamTexture getWebcameTextureRaw(){
        return outputImage.material.mainTexture as WebCamTexture;
    }

    float sigmoid(float value)
    {
        var k = (float)Mathf.Exp(value);

        return k / (1.0f + k);
    }

    // Softmax implementation from https://github.com/Syn-McJ/TFClassify-Unity-Barracuda/blob/master/Assets/Scripts/Detector.cs
    float[] Softmax(float[] values)
    {
        var maxVal = values.Max();
        var exp = values.Select(v => Mathf.Exp(v - maxVal));
        var sumExp = exp.Sum();

        return exp.Select(v => (float)(v / sumExp)).ToArray();
    }

    // Function to clear bounding boxes
    void ClearBoxes(){
        foreach(GameObject boxObj in boxes){
            Destroy(boxObj);
        }
    }

    // Function to render a bounding box
    void RenderBoundingBox(float x, float y, float width, float height){
        GameObject newBox = Instantiate(boundingBoxPrefab, canvas);
        boxes.Add(newBox);

        // Save output size
        float outputSize = (float)cam_crop_size;

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

        Debug.Log("Using WCT dimensions " + wct_width + " , " + wct_height);

        // Set box transform
        RectTransform boxTransform = newBox.GetComponent<RectTransform>();

        boxTransform.anchoredPosition = new Vector2(newx, newy);
        boxTransform.sizeDelta = new Vector2(newwidth, newheight);

    }

    // Function to parse output from a YOLO v2 Model above some threshold
    // From https://github.com/Syn-McJ/TFClassify-Unity-Barracuda/blob/master/Assets/Scripts/Detector.cs
    private List<BoundingBox> ParseOutputs(Tensor yoloModelOutput, float threshold = .3F)
    {
        var boxes = new List<BoundingBox>();

        for (int cy = 0; cy < COL_COUNT; cy++)
        {
            for (int cx = 0; cx < ROW_COUNT; cx++)
            {
                for (int box = 0; box < BOXES_PER_CELL; box++)
                {
                    var channel = (box * (CLASS_COUNT + BOX_INFO_FEATURE_COUNT));
                    var bbd = ExtractBoundingBoxDimensions(yoloModelOutput, cx, cy, channel);
                    float confidence = GetConfidence(yoloModelOutput, cx, cy, channel);

                    if (confidence < threshold)
                    {
                        continue;
                    }

                    float[] predictedClasses = ExtractClasses(yoloModelOutput, cx, cy, channel);
                    var (topResultIndex, topResultScore) = GetTopResult(predictedClasses);
                    var topScore = topResultScore * confidence;

                    if (topScore < threshold)
                    {
                        continue;
                    }

                    var mappedBoundingBox = MapBoundingBoxToCell(cx, cy, box, bbd);
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

    // Function to handle Tiny YOLO v2 output
    void ProcessModelOutput(Tensor outputTensor){

        List<float> floatData = outputTensor.ToReadOnlyArray().ToList();

        for(int cy = 0; cy < 13; cy++){
            for(int cx = 0; cx < 13; cx++){
                for(int b = 0; b < 5; b++){
                    
                    int channel = b * (20 + 5);
                    float tx = floatData[getYoloTensorOffset(channel, cx, cy)];
                    float ty = floatData[getYoloTensorOffset(channel + 1, cx, cy)];
                    float tw = floatData[getYoloTensorOffset(channel + 2, cx, cy)];
                    float th = floatData[getYoloTensorOffset(channel + 3, cx, cy)];
                    float tc = floatData[getYoloTensorOffset(channel + 4, cx, cy)];

                    float x = ((float)cx + sigmoid(tx)) * 32;
                    float y = ((float)cy + sigmoid(ty)) * 32;

                    float w = Mathf.Exp(tw) * anchors[2*b] * 32;
                    float h = Mathf.Exp(th) * anchors[(2*b) + 1] * 32;

                    float confidence = sigmoid(tc);

                    if(confidence > 0.9F){
                        Debug.Log("Confident box rated " + confidence + " at " + x + ", " + y + " sized " + w + ", " + h);
                        RenderBoundingBox(x,y,w,h);
                    }

                    float[] classConfidences = new float[20];

                    for(int i = 0; i < 20; i ++){
                        classConfidences[i] = floatData[getYoloTensorOffset(channel + 5 + i, cx, cy)];
                    }

                }
            }
        }
    }

    // Routine to handle model using pixel data
    // Based on code from Third-Aurora https://github.com/Third-Aurora/Barracuda-Image-Classification
    IEnumerator RunModelRoutinePixels(byte[] pixels) {

        float[] tensorData = new float[pixels.Length];

        for(int i = 0; i < pixels.Length; i++){
            tensorData[i] = (float)pixels[i];
        }

		Tensor tensor = new Tensor(1, cam_crop_size, cam_crop_size, 3, tensorData);

		barracudaWorker.Execute(tensor);
		Tensor outputTensor = barracudaWorker.PeekOutput();

        ProcessModelOutput(outputTensor);

		Debug.Log("Did Output.");

        // Clean up Tensors
        tensor.Dispose();
		outputTensor.Dispose();
		yield return null;
	}

    // Routine to handle model using Color32 data
    // Based on code from https://github.com/Syn-McJ/TFClassify-Unity-Barracuda/blob/master/Assets/Scripts/Detector.cs
    IEnumerator RunModelRoutineColors(Color32[] imageData) {

        // Get Tensor from image data
        Tensor tensor = thisPreprocessor.TransformImageData(imageData, cam_crop_size, cam_crop_size, IMAGE_MEAN, IMAGE_STD);

        // Setup input for model
        Dictionary<string, Tensor> inputs = new Dictionary<string, Tensor>();
        
        inputs.Add("image", tensor);

		barracudaWorker.Execute(tensor);
		Tensor outputTensor = barracudaWorker.PeekOutput();

        var results = ParseOutputs(outputTensor);
        List<BoundingBox> boxes = FilterBoundingBoxes(results, 5, 0.1f);

        foreach(BoundingBox box in boxes){
            RenderBoundingBox(box.Dimensions.X,box.Dimensions.Y,box.Dimensions.Width,box.Dimensions.Height);
            Debug.Log(box.ToString());
        }

		Debug.Log("Did Output.");

        // Clean up Tensors
        tensor.Dispose();
		outputTensor.Dispose();
		yield return null;
	}

    // Function to handle byte data from webcam image
    // Used for ScaleAndCropImage preprocessing
    void HandleModelData(byte[] pixels){
        StartCoroutine(RunModelRoutinePixels(pixels));
    }
    
    // Function to begin inference process with pre-processing on WCT
    void BeginInference(){
        // Grab WCT
        WebCamTexture thisWCT = getWebcameTextureRaw();

        // Send to preprocessing
        //thisPreprocessor.ScaleAndCropImage(thisWCT, cam_crop_size, HandleModelData);

        StartCoroutine(thisPreprocessor.ProcessImage(thisWCT, cam_crop_size, result =>
        {
            StartCoroutine(RunModelRoutineColors(result));
        }));
    }

    private float Sigmoid(float value)
    {
        var k = (float)Math.Exp(value);

        return k / (1.0f + k);
    }

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

    private float GetConfidence(Tensor modelOutput, int x, int y, int channel)
    {
        return Sigmoid(modelOutput[0, x, y, channel + 4]);
    }

    private CellDimensions MapBoundingBoxToCell(int x, int y, int box, BoundingBoxDimensions boxDimensions)
    {
        return new CellDimensions
        {
            X = ((float)y + Sigmoid(boxDimensions.X)) * CELL_WIDTH,
            Y = ((float)x + Sigmoid(boxDimensions.Y)) * CELL_HEIGHT,
            Width = (float)Math.Exp(boxDimensions.Width) * CELL_WIDTH * anchors[box * 2],
            Height = (float)Math.Exp(boxDimensions.Height) * CELL_HEIGHT * anchors[box * 2 + 1],
        };
    }

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

    private ValueTuple<int, float> GetTopResult(float[] predictedClasses)
    {
        return predictedClasses
            .Select((predictedClass, index) => (Index: index, Value: predictedClass))
            .OrderByDescending(result => result.Value)
            .First();
    }

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


    private List<BoundingBox> FilterBoundingBoxes(List<BoundingBox> boxes, int limit, float threshold)
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

    void Update(){

        count += 1;

        if(count % 50 == 0){

            //Texture2D thisTexture = getTextureFromWebcam();

            Debug.Log("Making Predictions...");

            ClearBoxes();

            BeginInference();

            count = 0;

            //Destroy(thisTexture); // Prevent mem leak
        }

    }

    
}
