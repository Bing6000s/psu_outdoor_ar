using System;
using UnityEngine;
using Unity.Barracuda;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ImageClassification : MonoBehaviour
{

    [Header("References")]
    public NNModel modelFile; // Reference the ONNX model file, should be efficientnetlite
    public TextAsset labelsFile; // Reference to text file with model class labels
    public Image outputImage; // Reference the camera viewfinder
    public GameObject boundingBoxPrefab; // Reference a prefab bounding box UI element
    public Transform canvas; // Transform for total canvas as parent of bounding box
    public RectTransform outputCamera; // Transform for output camera dimensions to scale elements
    public TMPro.TextMeshProUGUI runCounterText; // Reference to debug UI element for counting number of runs

    [Header("Config")]
    // Have seen various setups for these, but 127 & 128 appear to work well.
    private const int IMAGE_MEAN = 127; // Pixel mean value of 255
    private const float IMAGE_STD = 128; // Pixel std value out of 255
    public const float MIN_CONF = 0.1f; // The minimum confidence (0-1) to render bounding boxes for
    public int delayMod = 1000;

    [Header("Model config")]
    // Will generally need to search online to find these for various models.
    // Efficientnet uses "images" and "Softmax"
    public string INPUT_NAME; // Input name to first layer of model
    public string OUTPUT_NAME; // Output name from last layer of model

    private const int _image_size = 224; // The size to scale images down to, depending on model
    public int IMAGE_SIZE { get => _image_size; }

    private IWorker worker; // Worker for running ML
    private int runCounter = 0; // Count of times classification has been run
    private int runDelay = 0; // Counter to delay running
    private int locCounter = 0; // Location counter representing order of classfn on center, top left, bot right, etc

    private bool running; // Whether classfn is running
    private GameObject[] boxes; // List of bounding box UI elements
    private Preprocess thisPreprocessor; // Reference to attached preprocessor component
    private string[] labels; // List of label strings read from text file

    // Whitelist of label indices from classification label list
    private int[] whitelist = {928, 929, 930, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944, 945, 946, 947, 948, 949, 950, 951, 952, 953, 954, 955, 956, 957, 958, 959, 960, 961, 962, 963, 964, 965};

    // Function to reset running on suspended
    public void ResetRunning(){
        this.running = false;
    }

    // Function to clear bounding boxes
    public void ClearBoxes(){
        // Null check and init if not
        if(boxes == null){
            InitBoundingBoxesList();
        }

        // Destroy each box gameobject
        for(int i = 0; i < boxes.Length; i++){
            Destroy(boxes[i]);
        }
    }

    // Function to remove bounding box at certain position
    void DestroyBoxAtPosition(int index){
        if(index < boxes.Length){
            if(boxes[index] != null){
                // Destroy without removing
                GameObject boxToDestroy = boxes[index];
                Destroy(boxToDestroy);

                boxes[index] = null;
            }
        }
    }

    // Function to initialize the bounding boxes list
    void InitBoundingBoxesList(){
        int len = 5;

        this.boxes = new GameObject[len];

        // Add null elements for each position
        for(int i = 0; i < len; i ++){
            this.boxes[i] = null;
        }
    }

    // Function that runs on component init by Unity, may not run first on re-activation
    public void Start()
    {
        // Init counter vars
        this.runCounter = 0;
        this.runDelay = 0;
        this.locCounter = 0;

        // Init box list
        InitBoundingBoxesList();

        // Get preprocessor, labels, model, worker
        this.thisPreprocessor = gameObject.GetComponent<Preprocess>();
        this.running = false;
        this.labels = Regex.Split(this.labelsFile.text, "\n|\r|\r\n").Where(s => !String.IsNullOrEmpty(s)).ToArray();
        var model = ModelLoader.Load(this.modelFile);
        // https://docs.unity3d.com/Packages/com.unity.barracuda@1.0/manual/Worker.html
        //These checks all check for GPU before CPU as GPU is preferred if the platform + rendering pipeline support it
        this.worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
    }

    // Function to update run counter UI element with run count
    void UpdateRunGUI(int runs){
        runCounterText.text = "" + runs;
    }

    // Update runs at each Unity timestep
    public void Update(){

        // Tick delay if between runs
        if(!this.running){
            this.runDelay += 1;
        }

        // On sufficient delay rerun classfn
        if(this.runDelay > delayMod * Time.deltaTime && !this.running){

            // Update counters
            this.runCounter += 1;
            this.locCounter += 1;
            if(this.locCounter > 4){
                this.locCounter = 0;
            }
            this.runDelay = 0;

            // Update GUI
            UpdateRunGUI(this.runCounter);

            // Run classification
            this.running = true;
            BeginInference();
        }
        
    }

    // Function to begin inference process with pre-processing on WCT
    void BeginInference(){
        // Grab WCT
        WebCamTexture thisWCT = getWebcameTextureRaw();

        // Send to preprocessing, classify on result
        StartCoroutine(thisPreprocessor.ProcessImage(thisWCT, _image_size, this.locCounter, result =>
        {
            StartCoroutine(Classify(result));
        }));
    }

    // Function to just return WebCamTexture
    WebCamTexture getWebcameTextureRaw(){
        return outputImage.material.mainTexture as WebCamTexture;
    }

    // Run classification on some picture in the form of a Color32 array
    public IEnumerator Classify(Color32[] picture)
    {
        // Transform input to appropriate normalization
        using (var tensor = TransformInput(picture, IMAGE_SIZE, IMAGE_SIZE))
        {
            // Add inputs to tensor and execute model on them
            var inputs = new Dictionary<string, Tensor>();
            inputs.Add(INPUT_NAME, tensor);
            var enumerator = this.worker.Execute(inputs);
            
            // Get model output
            Tensor output = worker.PeekOutput(OUTPUT_NAME);
            
            // Filter model results
            // TODO use whitelist here
            var arr = output.ToReadOnlyArray();
            for(int l = 0; l < arr.Length; l ++){
                if(l < 928 || l > 960){
                    arr[l] = 0.0f;
                }
            }

            // Choose maximum classfn confidence
            var max = Mathf.Max(output.ToReadOnlyArray());
            var index = System.Array.IndexOf(arr, max);
            
            // Log classfn result
            Debug.Log(this.locCounter + " : Key--------" + labels[index] + " Value-------" + String.Format("{0:0.000}%", max*100));

            // Always destroy bounding box at this position once we've rerun
            DestroyBoxAtPosition(this.locCounter);

            // If sufficiently confident, render bounding box at some specified position
            if(max*100 >= MIN_CONF){
                RenderBoundingBox(this.locCounter, labels[index], this.locCounter);

                // Notify UI of label
                LabelsController.labelController.FoundLabel(labels[index]);
            }

            // Dispose of tensor and stop running
            this.running = false;
            output.Dispose();
            tensor.Dispose();

            yield return null;
        }
    }

    // Function to render a bounding box
    void RenderBoundingBox(int cropPosn, string label, int index){
        // Instantiate Unity gameobject
        GameObject newBox = Instantiate(boundingBoxPrefab, canvas);
        if(boxes.Length > index){
            boxes[index] = newBox;
        }
        
        // Set label
        Text thisText = newBox.GetComponentsInChildren<Text>()[0];
        thisText.text = label;

        // Get webcam size
        float wct_width = outputCamera.rect.width;
        float wct_height = outputCamera.rect.height;
        
        // Crop down to smaller dim
        float smallerSq = 0;

        // Handle width/height being cropped
        if(wct_height > wct_width){
            smallerSq = wct_width / 2.0f;
        }
        else{
            smallerSq = wct_height / 2.0f;
        }

        // Set box transform
        RectTransform boxTransform = newBox.GetComponent<RectTransform>();

        // Get half width and height
        float halfBox = smallerSq / 2.0f;

        // Get new x and y
        float newx, newy;
        // Center
        if(cropPosn == 0){
            newx = 0f;
            newy = 0f;
        }
        // Bot Right
        else if(cropPosn == 1){
            newx = wct_width/2.0f - halfBox;
            newy = wct_height/2.0f - halfBox;
        }
        // Top Right
        else if(cropPosn == 2){
            newx = wct_width/2.0f - halfBox;
            newy = -wct_height/2.0f + halfBox;
        }
        // Bot Left
        else if(cropPosn == 3){
            newx = -wct_width/2.0f + halfBox;
            newy = wct_height/2.0f - halfBox;
        }
        // Top Left
        else {
            newx = -wct_width/2.0f + halfBox;
            newy = -wct_height/2.0f + halfBox;
        }

        // Actually update transform
        boxTransform.anchoredPosition = new Vector2(newx, newy);
        boxTransform.sizeDelta = new Vector2(smallerSq, smallerSq);

    }

    // Function to normalize some model input data
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


}