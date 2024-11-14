using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSObjectPlacer : MonoBehaviour
{
    [SerializeField] GameObject objToSpawn;                         // prefab of the marker triangle
    [SerializeField] GameObject obstacleMarker;                     // prefab of the obstacle marker triangle

    [SerializeField] List<Vector3> gpsCoordinates;                  // List of latitude and longitude pairs
    [SerializeField] Vector2 inputVector;
    public List<GameObject> markers = new List<GameObject>();       // List of placed markers

    GPS gps;                                                        //reference to Arrow Manger
    int maxObjects = 2;
    int count;

    void Start()
    {
        gps = GPS.Instance;
        //Vector3 temp = GPSLocationToWorld(gps.latitude,gps.longitude,0f/*gps.altitude*/);
        inputVector = new Vector2(gps.latitude,gps.longitude);
        PlaceObjectAtGPS(inputVector.x,inputVector.y,0f);

    }

    void Update()
    {
        
        // Iterate through the GPS coordinates list and place objects at each location
        foreach (Vector3 gpsCoord in gpsCoordinates)
        {
            if(count != maxObjects)
            {
                PlaceObjectAtGPS(gpsCoord.x, gpsCoord.y, 0f/*gpsCoord.z*/);
            }
        }
    }

    // Method to place an object at a given GPS latitude and longitude
    void PlaceObjectAtGPS(float latitude, float longitude, float altitude)
    {
        // Convert GPS coordinates to Unity world coordinates
        Vector3 worldPosition = GPSEncoder.GPSToUCS(new Vector2(latitude,longitude));

        // Add a new target marker
        count++;
        PlaceMarker(worldPosition);
    }

    void PlaceMarker(Vector3 position)
    {
        //spawns Navpoints
        GameObject newMarker = Instantiate(objToSpawn, position, Quaternion.identity);
        markers.Add(newMarker);
    }
    void PlaceObstacle(Vector3 position)
    {
        //spawns obstacles
        GameObject newMarker = Instantiate(obstacleMarker, position, Quaternion.identity);
        markers.Add(newMarker);
    }
}
