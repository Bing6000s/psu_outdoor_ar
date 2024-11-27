using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class LocationManager : MonoBehaviour
{
    public GameObject locationPanel; // Reference to the LocationPanel
    public GameObject viewLocationsPanel; // Panel that displays stored locations
    public InputField locationNameInputField; // Reference to the InputField where user enters location name
    public Text viewLocationsText; // Reference to a Text component where stored locations will be displayed

    private CanvasGroup locationPanelCanvasGroup;
    private CanvasGroup viewLocationsPanelCanvasGroup;

    public Button closeButton;
    public Button storeLocationButton;
    public Button viewLocationsButton;
    public Button clearLocationsButton;
    public Button navigationButton;

    public List<string> storedLocations = new List<string>();
    public Dictionary<string, string> storedLocationsDictionary = new Dictionary<string, string>();
    private void Start()
    {
        locationPanelCanvasGroup = locationPanel.GetComponent<CanvasGroup>();
        viewLocationsPanelCanvasGroup = viewLocationsPanel.GetComponent<CanvasGroup>();

        HideLocationPanel();
        viewLocationsPanel.SetActive(false);

        // Load previously stored locations
        LoadStoredLocations();

        // Add listeners for the buttons
        closeButton.onClick.AddListener(HideLocationPanel);
        storeLocationButton.onClick.AddListener(OnStoreLocationButtonClicked);
        viewLocationsButton.onClick.AddListener(OnViewLocationButtonClicked);
        clearLocationsButton.onClick.AddListener(OnClearLocationsButtonClicked);
        navigationButton.onClick.AddListener(OnNavigationButtonClicked);
    }

    public void OnManageLocationsButtonPressed()
    {
        if (locationPanelCanvasGroup.alpha == 0)
            ShowLocationPanel();
        else
            HideLocationPanel();
    }

    private void ShowLocationPanel()
    {
        locationPanelCanvasGroup.alpha = 1;
        locationPanelCanvasGroup.interactable = true;
        locationPanelCanvasGroup.blocksRaycasts = true;
    }

    private void HideLocationPanel()
    {
        locationPanelCanvasGroup.alpha = 0;
        locationPanelCanvasGroup.interactable = false;
        locationPanelCanvasGroup.blocksRaycasts = false;
    }

    public async void OnStoreLocationButtonClicked()
    {
        // Grab the input from the user
        string locationName = locationNameInputField.text;
        if (string.IsNullOrEmpty(locationName))
        {
            Debug.LogError("String is empty.");
            return;
        }
        // Construct string to add location and send towards API for coordinate parsing
        string newLocation = locationName;
        string locationResult = await GetGeolocationAsStoredString(newLocation);
        if (string.IsNullOrEmpty(locationResult))
        {
            Debug.LogError("Error. Given location is either too far/does not exist.");
            return;
        }
        storedLocationsDictionary[newLocation] = locationResult;
        storedLocations.Add(newLocation + "\n" + locationResult);
        

        // Save the location
        SaveStoredLocations();
    }

    public void OnViewLocationButtonClicked()
    {
        viewLocationsText.text = string.Join("\n", storedLocations);
        viewLocationsPanel.SetActive(true);
        StartCoroutine(HideViewLocationPanelAfterDelay());
    }

    public void OnNavigationButtonClicked()
    {
        SceneManager.LoadScene("SavedLocations");
    }

    public void OnClearLocationsButtonClicked()
    {
        // Remove the specific key for stored locations
        PlayerPrefs.DeleteKey("storedLocations");

        // Clear the list and dictionary in memory
        storedLocationsDictionary.Clear();
        storedLocations.Clear();

        // Optionally, you can give feedback to the user
        Debug.Log("Stored locations cleared!");
    }

    private IEnumerator HideViewLocationPanelAfterDelay()
    {
        yield return new WaitForSeconds(6);
        viewLocationsPanel.SetActive(false);
    }

    private void SaveStoredLocations()
    {
        PlayerPrefs.SetString("storedLocations", string.Join(";", storedLocations));
        PlayerPrefs.Save();
    }

    private void LoadStoredLocations()
    {
        string savedLocations = PlayerPrefs.GetString("storedLocations", "");
        if (!string.IsNullOrEmpty(savedLocations))
        {
            storedLocations = new List<string>(savedLocations.Split(';'));
        }
    }

    public async Task<string> GetGeolocationAsStoredString(string address)
    {
        // Call the GetGeolocation method from AzureMapsGeocodingExample
        AzureMapsGeocodingExample.GeolocationResult geolocationResult = await AzureMapsGeocodingExample.GetGeolocation("28wliaKNAA7BkAk9JsOalLkkR81nyYHK9vgSd7Fd7zaPnLL7zjDVJQQJ99AIACYeBjFL5h9IAAAgAZMPD6O2", address);

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
}
