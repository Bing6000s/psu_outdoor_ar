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
            if(button != null)
            {
                button.onClick.AddListener(() => OnLocationSelected(location));
            }
        }
    }

    private void OnLocationSelected(string location)
    {
        // Handle the logic when a location is selected
        Debug.Log("Selected Location: " + location);

        // Split the location string and parse the lat and lon
        string[] coordinates = location.Split(':');
        if (coordinates.Length >= 2)
        {
            float lat = float.Parse(coordinates[2].Split(',')[0]);
            float lon = float.Parse(coordinates[3]);

            // Set the targetLocation for the ArrowDirection
            arrowDirectionScript.SetTargetLocation(new Vector2(lat, lon));
        }

        // Hide the navigation panel
        navigationPanel.SetActive(false);
        
        // Start the AR Session to display the camera feed
        arSession.enabled = true;
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