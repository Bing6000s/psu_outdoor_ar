using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    public GameObject locationItemPrefab; // Drag a button prefab here
    public Transform locationsContainer; // Drag the parent transform of where the buttons should appear
    public GameObject navigationPanel;
    public ARSession arSession;
    public Button mainMenuButton;
    public Button orButton;
    public ArrowDirection arrowDirectionScript;

    private List<string> storedLocations = new List<string>();

    void Start()
    {
        LoadStoredLocations();
        DisplayLocations();
        arSession.enabled = false;
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        orButton.onClick.AddListener(ReturnToObjectRecognition);
    }

    private void LoadStoredLocations()
    {
        string savedLocations = PlayerPrefs.GetString("storedLocations", "");
        if (!string.IsNullOrEmpty(savedLocations))
        {
            storedLocations = new List<string>(savedLocations.Split(';'));
        }
    }

    private void DisplayLocations()
    {
        foreach (string location in storedLocations)
        {
            GameObject item = Instantiate(locationItemPrefab, locationsContainer);
            item.GetComponentInChildren<Text>().text = location;

            Button button = item.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnLocationSelected(location));
            }
        }
    }

    private void OnLocationSelected(string location)
    {
        // Handle the logic when a location is selected
        Debug.Log("Selected Location: " + location);

        if (string.IsNullOrEmpty(location))
        {
            Debug.LogError("Location string is empty or null.");
            return;
        }

        // Split the string into lines
        string[] lines = location.Split('\n');
        if (lines.Length < 2)
        {
            Debug.LogError("Invalid location string format. Expected two lines.");
            return;
        }

        // Parse the second line for latitude and longitude
        string coordinatesLine = lines[1];

        // Example: "Latitude: 40.79494, Longitude: -77.866942"
        string[] parts = coordinatesLine.Split(',');
        if (parts.Length < 2)
        {
            Debug.LogError("Invalid coordinates format in location string.");
            return;
        }

        // Extract and parse latitude
        string latitudePart = parts[0].Trim(); // "Latitude: 40.79494"
        float latitude = float.Parse(latitudePart.Split(':')[1].Trim());

        // Extract and parse longitude
        string longitudePart = parts[1].Trim(); // "Longitude: -77.866942"
        float longitude = float.Parse(longitudePart.Split(':')[1].Trim());

        // Set the target location for the ArrowDirection script
        /* 
        Note from Ethan: This will need to get replaced. Clicking an location should be able to display 
        all of the points within the scroll view and act as if the application is still performing the same way it would 
        receive input from the main search bar.
        */
        arrowDirectionScript.SetTargetLocation(new Vector2(latitude, longitude));

        // Hide the navigation panel and enable AR session
        navigationPanel.SetActive(false);
        arSession.enabled = true;
        SceneManager.LoadScene("AR_Navigation");
    }

    public void ReturnToMainMenu()
    {
        navigationPanel.SetActive(true);
        arSession.enabled = false;
        SceneManager.LoadScene("AR_Navigation");
    }

    public void ReturnToObjectRecognition()
    {
        arSession.enabled = false;
        SceneManager.LoadScene("AR_Navigation");
    }
}