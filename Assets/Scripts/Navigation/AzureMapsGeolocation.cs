using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq; // Add this for JSON handling

public class AzureMapsGeocodingExample
{
    // Class to store the geolocation results
    public class GeolocationResult
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    // Method to get geolocation data from Azure Maps API
    public static async Task<GeolocationResult> GetGeolocation(string apiKey, string query)
    {
        string url = $"https://atlas.microsoft.com/search/address/json";

        using (HttpClient client = new HttpClient())
        {
            var queryParams = new Dictionary<string, string>
            {
                { "api-version", "1.0" },
                { "subscription-key", apiKey },
                { "query", query },
                { "limit", "1" }
            };

            var uriBuilder = new UriBuilder(url)
            {
                Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"))
            };

            try
            {
                HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Parse the JSON response using Newtonsoft.Json
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
                        return null; // No results found
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
