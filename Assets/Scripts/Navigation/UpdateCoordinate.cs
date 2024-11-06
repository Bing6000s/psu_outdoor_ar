using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCoordinate : MonoBehaviour
{
    public Text coordinates;

    void Update()
    {
        coordinates.text = "Latitude:" + GPS.Instance.latitude.ToString() + "\nLongitude:" + GPS.Instance.longitude.ToString() + "\nAltitude:" + GPS.Instance.altitude.ToString() + "\nDistance:" + Distance.distance.ToString() + "m";
    }
}
