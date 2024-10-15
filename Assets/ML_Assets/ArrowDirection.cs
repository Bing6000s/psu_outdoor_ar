using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    public Transform userTransform;  // Assign the AR Camera
    public Vector2 targetLocation;  // Target GPS location
    public GameObject arrowPrefab;  // Drag your arrow prefab here
    private GameObject instantiatedArrow;

    private void Start()
    {
        if(arrowPrefab != null)
        {
            instantiatedArrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity, transform);
        }
    }

    private void Update()
    {
        Vector2 userLocation = new Vector2(GPS.Instance.latitude, GPS.Instance.longitude);
        float bearing = CalculateBearing(userLocation.x, userLocation.y, targetLocation.x, targetLocation.y);

        // Apply bearing to the arrow
        transform.eulerAngles = new Vector3(0, -bearing, 0);
    }

    public void SetTargetLocation(Vector2 newTargetLocation)
    {
        targetLocation = newTargetLocation;
    }

    public static float CalculateBearing(float startLat, float startLon, float endLat, float endLon)
    {
        float startRadLat = Mathf.Deg2Rad * startLat;
        float startRadLon = Mathf.Deg2Rad * startLon;
        float endRadLat = Mathf.Deg2Rad * endLat;
        float endRadLon = Mathf.Deg2Rad * endLon;

        float dLon = endRadLon - startRadLon;

        float x = Mathf.Atan2(
            Mathf.Sin(dLon) * Mathf.Cos(endRadLat),
            Mathf.Cos(startRadLat) * Mathf.Sin(endRadLat) -
            Mathf.Sin(startRadLat) * Mathf.Cos(endRadLat) * Mathf.Cos(dLon)
        );

        float bearing = Mathf.Rad2Deg * x;
        bearing = (bearing + 360) % 360;

        return bearing; // Returns bearing in degrees between 0 to 360.
    }
}

