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
    private float initialHeading = 0f;  // Stores the initial compass heading
    private bool isInitialHeadingSet = false;

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
        Input.compass.enabled = true;
        Input.location.Start();

        // Wait until the location service initializes
        float maxWait = 20f;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
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

        // Retrieve initial GPS data
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        altitude = Input.location.lastData.altitude;

        // Capture the initial heading to use as the reference point
        StartCoroutine(CaptureInitialHeading());

    }

    private IEnumerator CaptureInitialHeading()
    {
        yield return new WaitForSeconds(2);  //wait for a stable compass reading

        if (!isInitialHeadingSet && Input.compass.enabled)
        {
            initialHeading = Input.compass.trueHeading;
            isInitialHeadingSet = true;
            Debug.Log("Captured initial heading: " + initialHeading);
        }
    }

    // Method to get the adjusted heading relative to the initial direction
    public float GetRelativeHeading()
    {
        if (isInitialHeadingSet)
        {
            float currentHeading = Input.compass.trueHeading;
            return (currentHeading - initialHeading + 360) % 360;  // Normalized to 0-360 degrees
        }
        return 0;
    }

    void Update()
    {
        if (isInitialHeadingSet)
        {
            float relativeHeading = GetRelativeHeading();
            // Use relativeHeading to place objects or update HUD elements

            // Continuously update GPS data
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            altitude = Input.location.lastData.altitude;
        }
    }

}