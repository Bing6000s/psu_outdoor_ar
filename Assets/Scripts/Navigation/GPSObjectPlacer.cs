using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSObjectPlacer : MonoBehaviour
{
    [SerializeField] GameObject objToSpawn;                         // prefab of the marker triangle
    [SerializeField] GameObject obstacleMarker;                     // prefab of the obstacle marker triangle

    [SerializeField] List<Vector2> gpsCoordinates;                  // List of latitude and longitude pairs
    public List<GameObject> markers = new List<GameObject>();       // List of placed markers

    float scaleFactor = 1000f;                                      // Conversion factor for GPS to Unity world space, adjust based on your scene size
    NavArrowMan arrowMan;                                           //reference to Arrow Manger

    void Start()
    {
        // Iterate through the GPS coordinates list and place objects at each location
        foreach (Vector2 gpsCoord in gpsCoordinates)
        {
            PlaceObjectAtGPS(gpsCoord.x, gpsCoord.y);
        }
    }

    // Method to place an object at a given GPS latitude and longitude
    void PlaceObjectAtGPS(float latitude, float longitude)
    {
        // Convert GPS coordinates to Unity world coordinates
        Vector3 worldPosition = GPSLocationToWorld(latitude, longitude);
        
        // Instantiate the object at the calculated world position
        Instantiate(objToSpawn, worldPosition, Quaternion.identity);

        // Add a new target marker (set the second argument to `true` to add as obstacle)
        arrowMan.AddMarker(worldPosition, false);
    }

    // Converts GPS latitude and longitude to Unity world position
    public Vector3 GPSLocationToWorld(float latitude, float longitude)
    {
        // Apply a conversion formula (you can tweak this based on your world setup)
        float x = longitude * scaleFactor;
        float z = latitude * scaleFactor;
        
        // Altitude can be used here, but we're keeping it 0 (ground level) for now
        return new Vector3(x, 0, z);
    }
    /*void PlaceMarker(Vector3 position)
    {
        //spawns Navpoints
        GameObject newMarker = Instantiate(objToSpawn, position, Quaternion.Euler(180, 0, 0));
        markers.Add(newMarker);
    }
    void PlaceObstacle(Vector3 position)
    {
        //spawns obstacles
        GameObject newMarker = Instantiate(obstacleMarker, position, Quaternion.Euler(180, 0, 0));
        markers.Add(newMarker);
    }*/
}
