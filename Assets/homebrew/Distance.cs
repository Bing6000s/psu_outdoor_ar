using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distance : MonoBehaviour
{
    public static float distance;
    float currentLat, currentLong, targetLat, targetLong, R;
    // Start is called before the first frame update
    void Start()
    {
        currentLat=GPS.Instance.latitude;
        currentLong=GPS.Instance.longitude;
        //pattee lib for test
        targetLat=40.7980655f;
        targetLong=-77.8684361f;
    }

    // Update is called once per frame
    void Update()
    {
        currentLat=GPS.Instance.latitude;
        currentLong=GPS.Instance.longitude;
        distance=distanceInMeters(currentLat, currentLong, targetLat, targetLong);
    }

    private float distanceInMeters(float currentLat, float currentLong, float targetLat, float targetLong){
        float R=6371f;
        
        float deltaLat= Mathf.Deg2Rad*(targetLat-currentLat);
        float deltaLong= Mathf.Deg2Rad*(targetLong-currentLong);
        currentLat*= Mathf.Deg2Rad;
        targetLat*= Mathf.Deg2Rad;

        float a= Mathf.Sin(deltaLat/2)*Mathf.Sin(deltaLat/2) + Mathf.Cos(currentLat) * Mathf.Cos(targetLat) * Mathf.Sin(deltaLong/2)*Mathf.Sin(deltaLong/2);
        float c= 2* Mathf.Atan2(Mathf.Sqrt(a),Mathf.Sqrt(1-a));
        float d= R * c * 1000;
        return Mathf.Round(d*10)/10f;
    }

}
