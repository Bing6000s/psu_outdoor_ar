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
    public NavigationBar navigationbar;

    private List<string> storedLocations = new List<string>();

    public void Start()
    {
        LoadStoredLocations();
        DisplayLocations();
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
        float lat = float.Parse(coordinates[2].Split(',')[0]);
        float lon = float.Parse(coordinates[3]);
        // Hide the navigation panel
        navigationPanel.SetActive(false);
<<<<<<< HEAD


=======
        
>>>>>>> parent of 291504a (Added the elevation script inside the scripts folder, untested.)
        // Start the AR Session to display the camera feed
        //arSession.enabled = false;
        navigationbar.inputfield.text = $"{lat},{lon}";
        SceneManager.LoadScene("AR_Navigation");

    }

    public void ReturnToMainMenu()
    {
        navigationPanel.SetActive(true);
        arSession.enabled = true;
        SceneManager.LoadScene("AR_Navigation");
    }

    public void ReturnToObjectRecognition()
    {
        arSession.enabled = false; 
        SceneManager.LoadScene("AR_Navigation");
    }
}