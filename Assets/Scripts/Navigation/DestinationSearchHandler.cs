using UnityEngine;
using TMPro;
public class DestinationSearchHandler : MonoBehaviour
{
    // Reference to the InputField (Search Bar)
    public TMP_InputField inputField;
    private string apiKey = "28wliaKNAA7BkAk9JsOalLkkR81nyYHK9vgSd7Fd7zaPnLL7zjDVJQQJ99AIACYeBjFL5h9IAAAgAZMPD6O2";
    // Reference to the Text Area (Gray Box)
    public TMP_Text searchDisplay;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the inputField and searchDisplay are assigned
        if (inputField != null && searchDisplay != null)
        {
            // Add a listener to the input field to call the OnInputChange method whenever the text changes
            inputField.onEndEdit.AddListener(OnInputSubmitted);
        }
    }

    // This method is called whenever the input text changes
    async void OnInputSubmitted(string input)
    {
        // Display the input in the gray box
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // Update the output textbox with the input value
            AzureMapsGeocodingExample.GeolocationResult apiResults = await AzureMapsGeocodingExample.GetGeolocation(apiKey, input);
            searchDisplay.text = $"Results display here:\n\nLatitude: {apiResults.Latitude}\nLongitude: {apiResults.Longitude} ";

            // Clear the input field
            inputField.text = "";
        }
    }
}
