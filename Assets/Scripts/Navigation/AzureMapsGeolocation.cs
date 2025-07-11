using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AzureMapsGeocodingExample
{
    public class GeolocationResult
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public static async Task<GeolocationResult> GetGeolocation(string apiKey, string query)
    {
        double latitude = GPS.Instance.latitude;
        double longitude = GPS.Instance.longitude;
        if (GPS.Instance.latitude == 0 && GPS.Instance.longitude == 0)
        {
            latitude = 40.810987;
            longitude = -77.892420;
        }


        string lat = latitude.ToString();
        string lon = longitude.ToString();
        string limit = "1";
        string radius = "10000";
        // Construct base URL
        string url = $"https://atlas.microsoft.com/search/fuzzy/json?";
        using (HttpClient client = new HttpClient())
        {
            // Build query parameters
            var queryParams = new Dictionary<string, string>
            {
                { "api-version", "1.0" },
                { "subscription-key", apiKey },
                { "query", query },
                { "lat", lat },
                { "lon", lon },
                {"limit", limit },
                { "radius", radius } // Changed "Radius" to "radius"

            };

            // Add query parameters to the URL
            var uriBuilder = new UriBuilder(url)
            {
                Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))
            };

            try
            {
                // Send the request and get the response
                HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Parse the JSON response
                    JObject json = JObject.Parse(jsonResponse);

                    if (json["results"] != null && json["results"].HasValues)
                    {
                        var position = json["results"][0]["position"];
                        return new GeolocationResult
                        {
                            Latitude = position["lat"].Value<double>(),
                            Longitude = position["lon"].Value<double>()
                        };
                    }
                    else
                    {
                        Console.WriteLine("No results found.");
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("Error occurred: " + response.ReasonPhrase);
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request error: " + e.Message);
                return null;
            }
        }
    }
}
