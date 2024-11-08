using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCoordinate : MonoBehaviour
{
    public Text coordinates;
    GPS GPS;                    //reference GPS script

    void Update()
    {
        if (GPS.Instance != null)
        {
            float latitude = GPS.Instance.latitude;
            float longitude = GPS.Instance.longitude;
            float altitude = GPS.Instance.altitude;
            float distance = Distance.distance;
            coordinates.text = $"Latitude: {latitude.ToString()}\n" +
                               $"Longitude: {longitude.ToString()}\n" +
                               $"Altitude: {altitude.ToString()} meters \n" +
                               $"Distance: {distance.ToString()} m";

            Debug.Log($"Updating Coordinates: Lat: {latitude}, Lon: {longitude}, Alt: {altitude}, Dist: {distance}");
        }
        else
        {
            coordinates.text = "Fetching GPS data...";
            Debug.LogWarning("GPS Instance not available!");
        }
    }
}
