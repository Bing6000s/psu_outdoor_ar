using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.XR.ARCoreExtensions; // For ARCore Geospatial APIs
using CesiumForUnity; // For Cesium Georeferencing (if needed)
using CesiumForUnity;


public class GPSObjectPlacer : MonoBehaviour
{
    //public ARGeospatialAnchorManager arAnchorManager; // ARCore Anchor Manager
    public CesiumGeoreference cesiumGeoreference;                   // Cesium Georeference component
    public Transform PollockArea;                                   // Cesium Sub Scene
    [SerializeField] GameObject objToSpawn;                         // prefab of the marker
    [SerializeField] GameObject obstacleMarker;                     // prefab of the obstacle marker

    [SerializeField] List<Vector3> gpsCoordinates;                  // List of latitude and longitude pairs
    [SerializeField] Vector2 inputVector;
    public List<GameObject> markers = new List<GameObject>();       // List of placed markers

    GPS gps;                                                        //reference to Arrow Manger
    int maxObjects = 1;
    int count = 0;

    void Start()
    {
        gps = GPS.Instance;
        //Vector3 temp = GPSLocationToWorld(gps.latitude,gps.longitude,0f/*gps.altitude*/);
        inputVector = new Vector2(gps.latitude,gps.longitude);
        //PlaceObjectAtGPS(inputVector.x,inputVector.y,0f);

    }

    void Update()
    {
        
        // Iterate through the GPS coordinates list and place objects at each location
        foreach (Vector3 gpsCoord in gpsCoordinates)
        {
            if(count < maxObjects)
            {
                PlaceObjectAtGPS(gpsCoord.x, gpsCoord.y, 319f/*gpsCoord.z*/);
            }
        }
    }

    // Method to place an object at a given GPS latitude and longitude
    void PlaceObjectAtGPS(float latitude, float longitude, float altitude)
    {
        // Instantiate the object and add CesiumGlobeAnchor for geospatial placement
        GameObject newMarker = Instantiate(objToSpawn,new Vector3(latitude,altitude,longitude),Quaternion.identity, PollockArea);
        CesiumGlobeAnchor globeAnchor = newMarker.AddComponent<CesiumGlobeAnchor>();

        // Set the geospatial coordinates
        globeAnchor.latitude = latitude;
        globeAnchor.longitude = longitude;
        globeAnchor.height = altitude;

        // Add the marker to the list and increment the counter
        markers.Add(newMarker);
        count++;
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
