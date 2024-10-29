using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


//Returns the elevation of giving lat, long
//Usage: double? elevation = await ElevationFetcher.GetElevationAsync(latitude, longitude);
public class ElevationFetcher
{
    private static readonly string apiKey = "AIzaSyCeS0-vZgIzJGUozRppxVQWq0wnvA4yN6o";
    private static readonly string baseUrl = "https://maps.googleapis.com/maps/api/elevation/json";

    public static async Task<double?> GetElevationAsync(double latitude, double longitude)
    {
        using HttpClient client = new HttpClient();

        /*Tested String:
         * https://maps.googleapis.com/maps/api/elevation/json?
         * locations=40.813139%2c-77.893880
         * &key=AIzaSyCeS0-vZgIzJGUozRppxVQWq0wnvA4yN6o
         */

        // Build the request URL
        string url = $"{baseUrl}?locations={latitude}%{longitude}&key={apiKey}";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            JObject jsonResponse = JObject.Parse(responseBody);

            if (jsonResponse["status"]?.ToString() == "OK")
            {
                // Retrieve the elevation value
                double elevation = (double)jsonResponse["results"][0]["elevation"];
                return elevation;
            }
            else
            {
                Console.WriteLine("API Error: " + jsonResponse["status"]);
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Request or JSON Parsing Error: " + e.Message);
            return null;
        }
    }

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Enter latitude:");
        double latitude = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Enter longitude:");
        double longitude = Convert.ToDouble(Console.ReadLine());

        double? elevation = await GetElevationAsync(latitude, longitude);

        if (elevation.HasValue)
        {
            Console.WriteLine($"Elevation at ({latitude}, {longitude}): {elevation.Value} meters");
        }
        else
        {
            Console.WriteLine("Failed to retrieve elevation data.");
        }
    }
}
