using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

public class ARObjectPlacer : MonoBehaviour
{
    public GameObject objectToPlace; // Prefab that will be placed in AR
    public GameObject placementIndicator; // Optional: Visual feedback for object placement

    private ARRaycastManager raycastManager;
    private Pose placementPose;
    private bool placementPoseIsValid = false;

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
        if (placementIndicator != null)
        {
            placementIndicator.SetActive(false); // Hide the indicator initially
        }
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        // Check for touch input and whether the placement pose is valid
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("Screen tapped at: " + Input.GetTouch(0).position);  

            if (!IsPointerOverUI(Input.GetTouch(0))) // Ensure not tapping on UI
            {
                PlaceObject();
            }
            else
            {
                Debug.Log("Tap ignored, it was over UI.");
            }
        }
    }

    // Checks if the user tapped over UI to avoid placing objects on UI elements
    private bool IsPointerOverUI(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = touch.position };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    // Update the position of the placement indicator based on AR raycast
    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid && placementIndicator != null)
        {
            placementIndicator.SetActive(true); // Show the placement indicator
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else if (placementIndicator != null)
        {
            placementIndicator.SetActive(false); // Hide the placement indicator when no valid plane is detected
        }
    }

    // Cast a ray from the screen touch point and check if it hits a plane
    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes); // Cast a ray to detect planes

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
            Debug.Log("Placement Pose Valid: " + placementPose.position);  // Log the detected position for feedback

            // Optional: Adjust rotation to align with the camera's forward direction
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
        else
        {
            Debug.Log("No valid planes detected.");  // Log when no plane is detected
        }
    }

    // Place the object at the valid placement pose
    private void PlaceObject()
    {
        if (objectToPlace != null)
        {
            Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
            Debug.Log("Object placed at: " + placementPose.position);  // Log the object's position after placement
        }
        else
        {
            Debug.LogError("No object to place. Please assign a prefab to 'objectToPlace' in the inspector.");
        }
    }
}
