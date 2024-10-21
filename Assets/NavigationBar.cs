using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using System.Threading.Tasks;
using System;

public class NavigationBar : MonoBehaviour
{
    public TMP_InputField inputfield;
    private string baseUrl = "https://atlas.microsoft.com/route/directions/json?api-version=1.0";
    private string apiKey = "API_KEY"; // Replace with your actual API key

    void Update()
    {
        // Start the API test when the scene starts
        if (Input.GetKeyDown(KeyCode.Return))
        {
            string userInput = inputfield.text;
            StartCoroutine(TestDirectionsAPI(userInput, true));
        }
    }

    // Coroutine to handle the geolocation asynchronously and start the directions API request
    IEnumerator TestDirectionsAPI(string destination_query, bool testing)
    {
        // Coordinates to test the API with (starting and destination)
        // 300 W College Ave
        string StartingLocation = "40.792460,-77.864042";
        // Penn State HUB
        string DestinationLocation = "40.798402,-77.861852";

        if (testing)
        {
            // Call async function and wait for the result
            Task<string> geoTask = GetGeolocationAsString(destination_query);
            yield return new WaitUntil(() => geoTask.IsCompleted);

            // Get the result from the async task
            string geoResult = geoTask.Result;

            // Parse the result to extract latitude and longitude
            if (!string.IsNullOrEmpty(geoResult))
            {
                Debug.Log("Search bar: Geolocation Result: " + geoResult);
                // Assuming the result is in format "Latitude: xx.xxxx, Longitude: yy.yyyy"
                // You would need to extract the coordinates from the geoResult string
                string[] geoParts = geoResult.Replace("Latitude: ", "").Replace("Longitude: ", "").Split(',');
                DestinationLocation = $"{geoParts[0].Trim()},{geoParts[1].Trim()}";
                Debug.Log("Search bar: Destination geocordinates: " + DestinationLocation);
            }
            else
            {
                Debug.LogError("Search bar: Geolocation not found");
                yield break; // Stop further execution if geolocation fails
            }
        }

        // Construct the full URL for the request
        string url = $"{baseUrl}&query={StartingLocation}:{DestinationLocation}&subscription-key={apiKey}&travelMode=pedestrian";
        Debug.Log("Search bar: Request URL: " + url); // Log the URL to verify

        // Make the API request
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send the request and wait for the response
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Search bar: Error: " + webRequest.error);
                Debug.LogError("Search bar: Response Code: " + webRequest.responseCode);
                Debug.LogError("Search bar: Response: " + webRequest.downloadHandler.text);
            }
            else
            {
                // Log the successful response
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Search bar: API Response: " + jsonResponse);

                // Deserialize the JSON response
                DirectionsResponse directionsResponse = JsonConvert.DeserializeObject<DirectionsResponse>(jsonResponse);

                // Access and log route information, including points
                if (directionsResponse != null && directionsResponse.Routes != null && directionsResponse.Routes.Length > 0)
                {
                    Route route = directionsResponse.Routes[0]; // First route

                    // Access the legs of the route
                    if (route.Legs != null && route.Legs.Length > 0)
                    {
                        foreach (var leg in route.Legs)
                        {
                            Debug.Log("Search bar result: Travel Time: " + leg.Summary.TravelTimeInSeconds + " seconds");
                            Debug.Log("Search bar result: Travel Length: " + leg.Summary.LengthInMeters + " meters");

                            // Access the points (latitude and longitude) of each leg
                            if (leg.Points != null && leg.Points.Length > 0)
                            {
                                Debug.Log("Search bar: Total travel in this trip:");
                                foreach (var point in leg.Points)
                                {
                                    Debug.Log($"Search bar: Location{leg}, Latitude = {point.Latitude}, Longitude = {point.Longitude}");
                                }
                            }
                            else
                            {
                                Debug.Log("Search bar: No points found for this leg.");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Search bar: No legs found in the route.");
                    }
                }
                else
                {
                    Debug.LogWarning("Search bar: No routes found in the response.");
                }
            }
        }
    }

    // Async function to handle geolocation and return a string with latitude and longitude
    public async Task<string> GetGeolocationAsString(string address)
    {
        // Call the GetGeolocation method from AzureMapsGeocodingExample
        AzureMapsGeocodingExample.GeolocationResult geolocationResult = await AzureMapsGeocodingExample.GetGeolocation(apiKey, address);

        if (geolocationResult != null)
        {
            string result = $"Latitude: {geolocationResult.Latitude}, Longitude: {geolocationResult.Longitude}";
            Debug.Log("Search bar: Geolocation Result: " + result);
            return result;
        }
        else
        {
            Debug.LogError("Search bar: Failed to get geolocation for the entered address.");
            return null;
        }
    }

    // Direction response classes (unchanged)
    public class DirectionsResponse
    {
        public Route[] Routes { get; set; }
    }

    public class Route
    {
        public Summary Summary { get; set; }
        public Leg[] Legs { get; set; }
    }

    public class Summary
    {
        public int LengthInMeters { get; set; }
        public int TravelTimeInSeconds { get; set; }
    }

    public class Leg
    {
        public Summary Summary { get; set; }
        public Point[] Points { get; set; }
    }

    public class Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
