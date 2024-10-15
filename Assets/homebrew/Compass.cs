using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Compass : MonoBehaviour
{
    public Text Orientation;

    private bool Awake = false;

    void Start()
    {
        Input.compass.enabled = true;
        Input.location.Start();
        StartCoroutine(InitializeCompass());
    }

    void Update()
    {
        if (Awake)
        {
            transform.rotation = Quaternion.Euler(0, Input.compass.trueHeading, 0);
            Orientation.text = ((int)Input.compass.trueHeading).ToString() + "° " + DegreesToCardinal(Input.compass.trueHeading);
        }
    }

    IEnumerator InitializeCompass()
    {
        yield return new WaitForSeconds(1f);
        Awake |= Input.compass.enabled;
    }

    private static string DegreesToCardinal(double degrees)
    {
        string[] caridnals = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N" };
        return caridnals[(int)Math.Round(((double)degrees * 10 % 3600) / 225)];
    }
}