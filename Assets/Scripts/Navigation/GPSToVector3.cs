using System;
using UnityEngine;

public class GPSToVector3
{
    // Radius of the Earth in miles
    private const double EarthRadiusMiles = 3958.8;

    public static Vector3 CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double lat1Rad = DegreesToRadians(lat1);
        double lon1Rad = DegreesToRadians(lon1);
        double lat2Rad = DegreesToRadians(lat2);
        double lon2Rad = DegreesToRadians(lon2);

        // Haversine formula
        double dLat = lat2Rad - lat1Rad;
        double dLon = lon2Rad - lon1Rad;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Distance in miles
        double distanceInMiles = EarthRadiusMiles * c;

        // Return as a Vector3 with lat, lon, and distance as components
        return new Vector3((float)lat1, (float)lon1, (float)distanceInMiles);
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
