using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSObjectPlacer : MonoBehaviour
{
    [SerializeField] GameObject objToSpawn;                         // prefab of the marker triangle
    [SerializeField] GameObject obstacleMarker;                     // prefab of the obstacle marker triangle

    [SerializeField] List<Vector3> gpsCoordinates;                  // List of latitude and longitude pairs
    public List<GameObject> markers = new List<GameObject>();       // List of placed markers

    float scaleFactor = 1000f;                                      // Conversion factor for GPS to Unity world space, adjust based on your scene size
    NavArrowMan arrowMan;                                           //reference to Arrow Manger
    GPS gps;                                                        //reference to Arrow Manger

    void Start()
    {
        gps = GPS.Instance;
        //Vector3 temp = GPSLocationToWorld(gps.latitude,gps.longitude,0f/*gps.altitude*/);
        Vector3 temp = GPSEncoder.GPSToUCS(new Vector2(gps.latitude,gps.longitude));
        PlaceObjectAtGPS(temp.x, temp.z, 0f/*temp.y*/);

        // Iterate through the GPS coordinates list and place objects at each location
        foreach (Vector3 gpsCoord in gpsCoordinates)
        {
            PlaceObjectAtGPS(gpsCoord.x, gpsCoord.y, 0f/*gpsCoord.z*/);
        }
    }

    // Method to place an object at a given GPS latitude and longitude
    void PlaceObjectAtGPS(float latitude, float longitude, float altitude)
    {
        // Convert GPS coordinates to Unity world coordinates
        //Vector3 worldPosition = GPSLocationToWorld(latitude, longitude, 0f/*altitude*/);
        Vector3 worldPosition = GPSEncoder.GPSToUCS(new Vector2(latitude,longitude));
        
        // Instantiate the object at the calculated world position
        Instantiate(objToSpawn, worldPosition, Quaternion.identity);

        // Add a new target marker
        arrowMan.AddMarker(worldPosition);
    }

    // Converts GPS latitude and longitude to Unity world position
    //! Does not work. off by ~10,000 meter
    public Vector3 GPSLocationToWorld(float latitude, float longitude, float altitude)
    {
        // Apply a conversion formula (you can tweak this based on your world setup)
        float x = longitude * scaleFactor;
        float z = latitude * scaleFactor;
        float y = altitude * scaleFactor; 
        
        // Altitude can be used here, but we're keeping it 0 (ground level) for now
        return new Vector3(x, y, z);
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
