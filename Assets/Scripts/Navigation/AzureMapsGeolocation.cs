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
        // Define default coordinates if GPS coordinates are not set
        double latitude = GPS.Instance.latitude != 0 ? GPS.Instance.latitude : 40.810987;
        double longitude = GPS.Instance.longitude != 0 ? GPS.Instance.longitude : -77.893880;

        // Construct base URL with subscription key and coordinates
        string url = $"https://atlas.microsoft.com/search/fuzzy/json?api-version=1.0&subscription-key={apiKey}&lat={latitude}&lon={longitude}&radius=1000";

        using (HttpClient client = new HttpClient())
        {
            // Add the query parameter to the URL
            var uriBuilder = new UriBuilder(url)
            {
                Query = $"query={Uri.EscapeDataString(query)}&limit=1"
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
