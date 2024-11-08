using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArAnchorManager : MonoBehaviour
{
    /*private ARAnchorManager arAnchorManager;
    private ARAnchor anchor;

    void Start()
    {
        // Initialize ARAnchorManager
        arAnchorManager = FindObjectOfType<ARAnchorManager>();

        // Place an anchor at the initial position
        PlaceInitialAnchor();
    }

    private void PlaceInitialAnchor()
    {
        // Create a new anchor at the starting point (you can specify a specific position if needed)
        Pose initialPose = new Pose(Camera.main.transform.position, Quaternion.identity);
        anchor = arAnchorManager.AddAnchor(initialPose);

        if (anchor != null)
        {
            Debug.Log("Anchor placed at initial position.");
        }
        else
        {
            Debug.LogError("Failed to place anchor.");
        }
    }

    public void PlaceObjectRelativeToAnchor(GameObject objToPlace, Vector3 offset)
    {
        if (anchor != null)
        {
            // Calculate the position relative to the anchor
            Vector3 worldPosition = anchor.transform.position + offset;

            // Place the object at this calculated position
            objToPlace.transform.position = worldPosition;
        }
        else
        {
            Debug.LogWarning("Anchor not found. Object placement may be unstable.");
        }
    }*/
}
