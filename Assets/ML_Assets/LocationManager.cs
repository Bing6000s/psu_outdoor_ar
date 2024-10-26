using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    public List<string> storedLocations = new List<string>(); // List to store the coordinates

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

    public void OnStoreLocationButtonClicked()
    {
        string locationName = locationNameInputField.text;
        if (string.IsNullOrEmpty(locationName))
        {
            Debug.LogError("Please provide a name for the location.");
            return;
        }

        string newLocation = locationName + ": Lat:" + GPS.Instance.latitude + ", Lon:" + GPS.Instance.longitude;

        // Check if the location already exists in the list
        if (storedLocations.Contains(newLocation))
        {
            Debug.LogError("This location has already been stored.");
            return;
        }

        // Add location to the list
        storedLocations.Add(newLocation);

        // Save the location
        SaveStoredLocations();

        // Disable the button for a short duration to prevent rapid presses
        StartCoroutine(DisableButtonTemporarily(storeLocationButton));
    }

    public void OnViewLocationButtonClicked()
    {
        viewLocationsText.text = string.Join("\n", storedLocations);
        viewLocationsPanel.SetActive(true);
        StartCoroutine(HideViewLocationPanelAfterDelay());
    }

    public void OnNavigationButtonClicked()
    {
        SceneManager.LoadScene("Navigation");
    }

    public void OnClearLocationsButtonClicked()
    {
        // Remove the specific key for stored locations
        PlayerPrefs.DeleteKey("storedLocations");
        // Clear the list in memory
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

    private IEnumerator DisableButtonTemporarily(Button button)
    {
        button.interactable = false;
        yield return new WaitForSeconds(0.5f); // Adjust the duration as needed
        button.interactable = true;
    }
}