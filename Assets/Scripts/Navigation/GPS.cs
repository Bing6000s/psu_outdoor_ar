using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class GPS : MonoBehaviour
{
    public static GPS Instance { set; get; }
    public float latitude;
    public float longitude;
    public float altitude;
    public float waittime = 1;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Request permissions
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        StartCoroutine(GetStartLocationService());
    }

    private IEnumerator GetStartLocationService()
    {
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location not enabled on device or app does not have permission to access location");
            yield break;
        }

        // Start the location service
        Input.location.Start();

        // Wait until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(waittime);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait <= 0)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            yield break;
        }

        // Update GPS coordinates continuously
        while (true)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                // Update latitude, longitude, and altitude
                latitude = Input.location.lastData.latitude;
                longitude = Input.location.lastData.longitude;
                altitude = Input.location.lastData.altitude;

                Debug.Log($"Updated GPS Coordinates: Latitude: {latitude}, Longitude: {longitude}, Altitude: {altitude}");
            }
            else
            {
                Debug.LogError("Unable to update location");
            }

            // Wait before the next update
            yield return new WaitForSeconds(waittime);  // wait for the next update
        }
    }
}