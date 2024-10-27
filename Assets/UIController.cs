using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public Button startButton;
    public Button myImagesButton;
    public Button settingsButton;

    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        startButton = root.Q<Button>("start-button");
        myImagesButton = root.Q<Button>("my-images-button");
        settingsButton = root.Q<Button>("settings-button");

        if (startButton == null) Debug.LogError("Start Button not found!");
        if (myImagesButton == null) Debug.LogError("My Images Button not found!");
        if (settingsButton == null) Debug.LogError("Settings Button not found!");

        startButton.clicked += StartButtonPressed;
        myImagesButton.clicked += MyImagesButtonPressed;
        settingsButton.clicked += SettingsButtonPressed;
    }

    void StartButtonPressed()
    {
        SceneManager.LoadScene("AR_Navigation");
    }

    void MyImagesButtonPressed()
    {
        SceneManager.LoadScene("MyImages");
    }

    void SettingsButtonPressed()
    {
        SceneManager.LoadScene("Settings");
    }
}
