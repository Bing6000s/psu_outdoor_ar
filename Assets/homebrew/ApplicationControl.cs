using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationControl : MonoBehaviour
{
    public GameObject outputCamera;
    public GameObject outputTarget;
    public GameObject infographic;

    public int rotation = 1;

    // Start is called before the first frame update
    void Start()
    {
        rotation = (int)Input.deviceOrientation;
    }

    // Update is called once per frame
    void Update()
    {
        // An Switch case checking all of the possible device orientations
        //switch ((int) Input.deviceOrientation)
        //{
        //    case (int)DeviceOrientation.Portrait:
        //        // Infographic Positioning
        //        infographic.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        //        infographic.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
        //        // Camera Viewfind Positioning
        //        outputCamera.transform.position = new Vector3(0.0f, 3.5f, 0.0f);
        //        outputCamera.transform.rotation = new Quaternion(0.0f, 270.0f, 90.0f, 0.0f);
        //        break;
        //    case (int)DeviceOrientation.PortraitUpsideDown:
        //        // Infographic Positioning
        //        infographic.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        //        infographic.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

        //        break;
        //    case (int)DeviceOrientation.LandscapeLeft:
        //        // Infographic Positioning
        //        infographic.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        //        infographic.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

        //        break;
        //    case (int)DeviceOrientation.LandscapeRight:
        //        // Infographic Positioning
        //        infographic.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        //        infographic.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

        //        break;
        //    default:
        //        break;
        //}
    }
}
