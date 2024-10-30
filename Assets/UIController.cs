using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;
public class UIController : MonoBehaviour
{
    public Button startButton;
    public Button myImagesButton;
    public Button settingsButton;

    public NavigationManager navigationManager;
    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        startButton = root.Q<Button>("start-button");
        myImagesButton = root.Q<Button>("my-images-button");
        settingsButton = root.Q<Button>("settings-button");

        startButton.clicked += StartButtonPressed;
        myImagesButton.clicked += MyImagesButtonPressed;
        settingsButton.clicked += SettingsButtonPressed;
    }

    void StartButtonPressed()
    {
        SceneManager.LoadScene("AR_Navigation");
        //navigationManager.Start();
        //navigationManager.arSession.enabled = true;
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
