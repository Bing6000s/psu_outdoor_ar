using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LabelsController : MonoBehaviour
{
    public int labelsLength;
    public TMPro.TextMeshProUGUI[] labelUIElements;

    private string[] recentLabels;
    private float[] labelTimestamps;

    public static LabelsController labelController;

    // Setup arrays on start
    void Start()
    {
        // Register as singleton, so there is only one global LabelsController
        if(labelController == null){
            labelController = this;
        }
        else{
            Destroy(gameObject);
        }

        recentLabels = new string[labelsLength];
        labelTimestamps = new float[labelsLength];
    }

    // Function called from models when they find any label
    public void FoundLabel(string label){
        // Ignore if already present
        if(recentLabels.Contains(label)){
            return;
        }

        // Evict oldest label
        float oldestTime = labelTimestamps.Min();
        int oldestIndex = System.Array.IndexOf(labelTimestamps, oldestTime);

        // Update with new index
        recentLabels[oldestIndex] = label;
        labelTimestamps[oldestIndex] = Time.time;

        // Update UI
        labelUIElements[oldestIndex].text = label;
    }
    
}
