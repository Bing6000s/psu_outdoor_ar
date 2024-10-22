using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavArrowMan : MonoBehaviour, IDataPersistence
{
    const int maxMarkers = 3;
    int markerCount = 0;
    [SerializeField] GameObject objToSpawn;             // prefab of the marker triangle
    [SerializeField] GameObject obstacleMarker;         // prefab of the obstacle marker triangle
    //[SerializeField] GameObject player;                 // reference to player
    [SerializeField] Canvas canvas;                     // reference to canvas
    [SerializeField] UnityEngine.UI.Image NavArrow;     // reference to NavArrow

    List<GameObject> targets = new List<GameObject>();
    List<GameObject> obstacles = new List<GameObject>();
    float closest;
    float distance;
    float closestObstacle;
    float distanceToObstacle;

    Vector3 direction;
    Vector3 playerVec;      //Use GPs instead of player Position
    Vector3 targetVec;
    Vector3 obstacleVec;

    GPS gps;                // reference to GPS script

    float adjustmentAngle;
    bool adjustingForObstacle;

    //references to the GameObjects chosen by the FindClosest functions
    GameObject closestTargetObj;
    GameObject closestObstacleObj;

    // Start is called before the first frame update
    void Start()
    {
        closest = Mathf.Infinity;
        closestObstacle = Mathf.Infinity;

        gps = GPS.Instance;                         // Get the instance of the GPS script
        GatherAllMarkers();                         // Gather all existing markers at the start
    }

    // Update is called once per frame
    void Update()
    {
        adjustingForObstacle = false;
        adjustmentAngle = 0f;

        /*if (Input.GetButtonDown("Jump") && markerCount < maxMarkers)
        {
            PlaceMarker();
            markerCount++;
        }*/

        // Update player position using GPS coordinates
        playerVec = GPSLocationToWorld(gps.latitude, gps.longitude);

        // Remove markers if player is close enough
        RemoveMarkersNearPlayer();
        // finds targets
        FindClosestTarget();
        // finds obstacles
        FindClosestObstacle();
        

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

    void PlaceMarker()
    {
        // Spawn markers
        targets.Add(Instantiate(objToSpawn, transform.position, Quaternion.Euler(180, 0, 0)) as GameObject);
    }

     // Converts GPS latitude and longitude into Unity world position (simplified for a flat map)
    Vector3 GPSLocationToWorld(float latitude, float longitude)
    {
        // Example conversion: scale latitude and longitude to Unity units
        // Adjust scale factor as needed to fit the game world size
        float scaleFactor = 1000f;  // You can tweak this based on world size

        float x = longitude * scaleFactor;
        float z = latitude * scaleFactor;

        // Keep y (altitude) as 0 for now, or can use altitude data from the GPS script
        return new Vector3(x, 0, z);
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
            if (distance < 0.5f) // 0.5f is an example threshold
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
        //Debug.Log(closestTargetObj.name);
    }

    void PlaceObstacle()
    {
        //spawns obstacles
        obstacles.Add(Instantiate(obstacleMarker, transform.position, Quaternion.Euler(180, 0, 0)) as GameObject);
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
        // Project the direction vector onto the X-Z plane
        Vector2 direction2D = new Vector2(direction.x, direction.z);

        // Calculate the target angle in degrees
        float targetAngle = Mathf.Atan2(direction2D.y, direction2D.x) * Mathf.Rad2Deg;

        // Get the current angle of the NavArrow around the Z axis
        float currentAngle = NavArrow.transform.eulerAngles.z;

        // Smoothly rotate the NavArrow around the Z axis
        float rotationSpeed = 360f; // Degrees per second
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle - 90 + adjustmentAngle, rotationSpeed * Time.deltaTime);

        // Apply the new angle to the NavArrow's rotation around the Z axis
        NavArrow.transform.rotation = Quaternion.Euler(0f, 0f, newAngle);

    }
}
