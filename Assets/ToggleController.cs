using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToggleController : MonoBehaviour
{
    // References to detector and classifier
    public GameObject detectorObj;
    public GameObject classifierObj;
    public GameObject debugCounter;
    public GameObject webCamPicture;
    public GameObject labelButtons;
    private Yolo3Detection detector; // Detection monobehavior to be loaded from gameobject
    private ImageClassification classifier; // Classifier monobehavior to be loaded from gameobject

    // On isDetecting = true, use yolo, else efficientnet
    private bool isDetecting;
    private bool isDebug;
    private bool showLabels;

    // Unity start
    void Start()
    {
        // Get object components
        detector = detectorObj.GetComponent<Yolo3Detection>();
        classifier = classifierObj.GetComponent<ImageClassification>();

        // Default to detection
        isDetecting = true;
        classifierObj.SetActive(false);

        isDebug = true;
        showLabels = true;
    }

    // Unity time step
    void Update()
    {
        // Check for spacebar to toggle Added To Button
        // if (Input.GetKeyDown("space"))
        // {
        //     ToggleMode();
        // }
    }

    // Function to switch between the classfn and detection modes
    public void ToggleMode()
    {
        if (isDetecting)
        {
            // Disable detection and activate classfn
            detector.ClearBoxes();
            detectorObj.SetActive(false);
            classifierObj.SetActive(true);
            classifier.ResetRunning();
            classifier.ClearBoxes();
        }
        else
        {
            // Disable classfn and enable detection
            classifier.ClearBoxes();
            classifierObj.SetActive(false);
            detectorObj.SetActive(true);
            detector.ResetRunning();
            detector.ClearBoxes();
        }
        isDetecting = !isDetecting;
    }

    public void ToggleDebug()
    {
        if (isDebug)
        {
            debugCounter.SetActive(false);
        }
        else
        {
            debugCounter.SetActive(true);
        }
        isDebug = !isDebug;
    }

    public void RotateCamera()
    {
        webCamPicture.transform.Rotate(180.0f, 0.0f, 0.0f, Space.World);
    }

    public void toggleLabels()
    {
        if (showLabels)
        {
            labelButtons.transform.position = labelButtons.transform.position + (new Vector3(1000, 0, 0));
        }
        else
        {
            labelButtons.transform.position = labelButtons.transform.position + (new Vector3(-1000, 0, 0));
        }
        showLabels = !showLabels;
    }

    public void shutdown()
    {
        // if (classifier != null)
        // {
        //     classifier.ClearBoxes();  
        //     classifier.ReleaseResources();  // Add a method in ImageClassification to release resources
        // }

        // Destroy instantiated objects if no longer needed
        Destroy(detectorObj);
        Destroy(classifierObj);
        Destroy(debugCounter);
        Destroy(webCamPicture);
        Destroy(labelButtons);

        // unload unused assets to free up memory
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        SceneManager.LoadScene("MainMenu");
    }
}
