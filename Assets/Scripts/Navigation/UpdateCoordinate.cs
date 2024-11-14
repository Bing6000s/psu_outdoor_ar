using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCoordinate : MonoBehaviour
{
    [SerializeField] Text coordinates;
    GPS gps;

    void Start()
    {
        gps = GPS.Instance;            // Initialize the GPS instance in Start
    }

    void Update()
    {
        if (gps != null)
        {
            float latitude = gps.latitude;
            float longitude = gps.longitude;
            float altitude = gps.altitude;
            updateText(latitude, longitude, altitude, 0);
            
        }
        else
        {
            coordinates.text = "Fetching GPS data...";
            Debug.LogWarning("GPS Instance not available!");
        }
    }
    public void updateText(float latitude, float longitude, float altitude, float distance)
    {
        string latSTR = latitude.ToString();
        string longSTR = longitude.ToString();
        string altSTR = altitude.ToString();
        string disSTR = distance.ToString();

        if(latitude == null)
        {
            latSTR = "NOT_FOUND";
        }
        if(longitude == null)
        {
            longSTR = "NOT_FOUND";
        }
        if(altitude == null)
        {
            altSTR = "NOT_FOUND";
        }
        if(distance == null)
        {
            disSTR = "NOT_FOUND";
        }

        coordinates.text = $"Latitude: {latSTR}\n" +
                            $"Longitude: {longSTR}\n" +
                            $"Altitude: {altSTR} meters \n" +
                            $"Distance: {disSTR}\n";

        Debug.Log($"Updating Coordinates: Lat: {latitude}, Lon: {longitude}, Alt: {altitude}");

    }
}
