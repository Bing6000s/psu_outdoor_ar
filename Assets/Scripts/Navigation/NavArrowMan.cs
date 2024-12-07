using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavArrowMan : MonoBehaviour, IDataPersistence
{
    int maxMarkers = 3;
    int markerCount = 0;
    [SerializeField] UnityEngine.UI.Image NavArrow;     // reference to NavArrow
    [SerializeField] Camera mainCamera;                 // Add a reference to the camera
    [SerializeField] GPSObjectPlacer gpsObjectPlacer;   // Reference to the GPSObjectPlacer script
    List<GameObject> targets = new List<GameObject>();
    List<GameObject> obstacles = new List<GameObject>();
    float closest;
    float distance;
    float closestObstacle;
    float distanceToObstacle;

    Vector3 direction;
    Vector3 playerVec;      //Use GPs instead of player Position
    Vector3 previousPlayerVec;  // New variable to store previous position
    Vector3 targetVec;
    Vector3 obstacleVec;

    GPS gps;                // reference to GPS script
    float adjustmentAngle;
    bool adjustingForObstacle;

    //references to the GameObjects chosen by the FindClosest functions
    GameObject closestTargetObj;
    GameObject closestObstacleObj;

    // Define a movement threshold (adjust based on your game world scale)
    float movementThreshold = 0.0001f;
    private float initialDeviceHeading = 0f;

    IEnumerator WaitForMarkers()
    {
        yield return new WaitForEndOfFrame(); // Wait for the end of the current frame
        GatherAllMarkers(); // Now gather the markers
    }

    // Start is called before the first frame update
    void Start()
    {
        closest = Mathf.Infinity;
        closestObstacle = Mathf.Infinity;
        
        // GPS test
        /*Vector2 playerVec2 = new Vector2(gps.latitude, gps.longitude);
        Vector3 pos = GPSEncoder.GPSToUCS(playerVec2);
        print("pos : " + pos);
        print("GPS : " + GPSEncoder.USCToGPS(pos));
        updateCoordinate.updateText(pos.x,pos.z,pos.y,0, pos);*/

        //place markers
        // PlaceMarker();

        gps = GPS.Instance;                         // Get the instance of the GPS script
        initialDeviceHeading = gps.GetRelativeHeading();  // Capture the initial device heading
        StartCoroutine(WaitForMarkers());           // Gather all existing markers at the start
        previousPlayerVec = Vector3.zero;           // Initialize to zero for the first comparison
        //inputVector = new Vector2(gps.latitude, gps.longitude);
        //GPSEncoder.SetLocalOrigin(inputVector);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(WaitForMarkers());           // Gather all existing markers
        // GPS test
        /*Vector3 pos = GPSEncoder.GPSToUCS(inputVector);
        print("pos : " + pos);
        print("GPS : " + GPSEncoder.USCToGPS(pos));
        updateCoordinate.updateText(pos.x,pos.z,pos.y,0, pos);*/

        // Update maxMarkers based on the number of markers in GPSObjectPlacer
        maxMarkers = gpsObjectPlacer.markers.Count;
        adjustingForObstacle = false;
        adjustmentAngle = 0f;

        // Update player position using GPS coordinates
        //playerVec = gpsObjectPlacer.GPSLocationToWorld(gps.latitude, gps.longitude,0f/*gps.altitude*/);
        playerVec = GPSEncoder.GPSToUCS(new Vector2(gps.latitude, gps.longitude));
        
        // Check if the player has moved significantly
        //if (Vector3.Distance(previousPlayerVec, playerVec) > movementThreshold)
        //{
            // Debug.Log("significant movement..."); 
            // Only update if the movement is significant
            previousPlayerVec = playerVec;

            // Remove markers if player is close enough
            // RemoveMarkersNearPlayer();

            // finds targets
            FindClosestTarget();

            // finds obstacles
            FindClosestObstacle();
        //}

        // Rotate the NavArrow to point towards the closest marker
        if (targets.Count > 0)
        {
            RotateNavArrow();
        }

        //Debug that prints info about avoiding obstacle status
        //Debug.Log(adjustingForObstacle + ", Distance: " + closestObstacle.ToString() + ", Angle: " + adjustmentAngle);
    }

    public void LoadData(GameData data)
    {
        this.playerVec = data.playerVec;
    }

    public void SaveData(ref GameData data)
    {
        data.playerVec = this.playerVec;
    }    

    void RemoveMarkersNearPlayer()
    {
        List<GameObject> markersToRemove = new List<GameObject>();

        foreach (GameObject target in targets)
        {
            // playerVec = player.transform.position;
            targetVec = target.transform.position;
            distance = Vector3.Distance(playerVec, targetVec);

            // Adjust the distance threshold as needed
            if (distance < 10f)
            {
                markersToRemove.Add(target);
            }
        }

        // Remove markers that are too close
        foreach (GameObject marker in markersToRemove)
        {
            targets.Remove(marker);
            Destroy(marker);
            markerCount--; // Decrement marker count
        }
    }

    void GatherAllMarkers()
    {
        // Clear existing markers to avoid duplicates
        targets.Clear();

        // Find all game objects with the "Mark" tag
        GameObject[] allMarkers = GameObject.FindGameObjectsWithTag("Mark");
        GameObject[] allObstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        // Add found markers to the targets list
        foreach (GameObject marker in allMarkers)
        {
            targets.Add(marker);
        }
        foreach (GameObject obstacle in allObstacles)
        {
            obstacles.Add(obstacle);
        }

        // Update the marker count
        markerCount = targets.Count;
    }


    void FindClosestTarget()
    {
        closest = Mathf.Infinity;
        //update when significant movement is detected.
        
        foreach (GameObject target in targets)
        {
            // playerVec = player.transform.position;
            targetVec = target.transform.position;
            distance = Vector3.Distance(playerVec, targetVec);
            
            if(distance < closest || closest == 0)
            {
                closestTargetObj = target;
                closest = distance;
                direction = targetVec - playerVec;
            }
        }

        //Debug that prints the closet target name
        //Debug.Log("target object" + closestTargetObj.name);
    }
   void FindClosestObstacle()
    {
        closestObstacle = Mathf.Infinity;

        if (targets.Count != 0 && obstacles.Count != 0)
        {
            foreach (GameObject obstacle in obstacles)
            {
                // playerVec = player.transform.position;
                obstacleVec = obstacle.transform.position;
                distanceToObstacle = Vector3.Distance(playerVec, obstacleVec);

                if (distanceToObstacle < closestObstacle || closestObstacle == 0)
                {
                    closestObstacle = distanceToObstacle;
                    closestObstacleObj = obstacle;

                    if (distanceToObstacle < 10f && Vector3.Angle(direction, obstacleVec - playerVec) <= 60f)
                    {
                        adjustingForObstacle = true;
                        adjustmentAngle = 60f - Vector3.Angle(direction, obstacleVec - playerVec);

                        if (Vector3.Cross(direction, obstacleVec - playerVec).y < 0)
                        {
                            adjustmentAngle = -adjustmentAngle;
                        }
                    }
                }
            }
        }

        //Debug that prints avoided obstacle info
        //Debug.Log(closestObstacleObj.name + ", Angle: " + Vector3.Angle(targetVec - playerVec, obstacleVec - playerVec).ToString() + ", Distance: " + closestObstacle);
    }

    void RotateNavArrow()
    {
        // Get the relative heading from the GPS
        float relativeHeading = gps.GetRelativeHeading();

        // direction from device to object
        Vector3 targetDirection = playerVec - direction;

        // Project the direction vector onto the X-Z plane
        Vector2 direction2D = new Vector2(targetDirection.x, targetDirection.z).normalized;

        // Get the camera's forward direction projected onto the X-Z plane
        //! removed so the camera position no longer affects orientation
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector2 cameraForward2D = new Vector2(cameraForward.x, cameraForward.z).normalized;

        // Calculate the relative direction from the target to relative heading (true north).
        float angleBetween = Vector2.SignedAngle(cameraForward2D, direction2D);

        // Adjust the target angle based on the initial device heading to ensure consistent rotation
        float targetAngle = Mathf.Atan2(direction2D.y, direction2D.x) * Mathf.Rad2Deg;
        float adjustedAngle = targetAngle + angleBetween - (relativeHeading - initialDeviceHeading);

        //combine rotation relative to where camera is facing

        // Smoothly rotate the NavArrow around the Z axis based on the relative direction
        float rotationSpeed = 360f;

        // Adjust the angle based on the compass heading
        //float adjustedAngle = angleBetween - relativeHeading;
        float newAngle = Mathf.MoveTowardsAngle(NavArrow.transform.eulerAngles.z, adjustedAngle - adjustmentAngle, rotationSpeed * Time.deltaTime);

        // Apply the new angle to the NavArrow's rotation around the Z axis
        NavArrow.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        UpdateNavArrowColor();

    }
    void UpdateNavArrowColor()
    {
        // Update the color based on distance to the closest target
        if (closest < 30f)
        {
            NavArrow.color = Color.red;  // Close to the target
        }
        else if (closest < 720f)
        {
            NavArrow.color = Color.yellow;  // Medium distance
        }
        else
        {
            NavArrow.color = Color.green;  // Far away
        }
    }
}
