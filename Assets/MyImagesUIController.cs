using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.IO;

public class MyImagesUIController : MonoBehaviour
{

    public Button uploadButton;
    public Button currentObjectsButton;
    public Button trainModelButton;
    public Button backButton;
    public string rootFolder = "Psuedo_Data/Database";
    string modelFile = "Psuedo_Data/Model";


    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        uploadButton = root.Q<Button>("upload-button");
        currentObjectsButton = root.Q<Button>("current-objects-button");
        trainModelButton = root.Q<Button>("train-button");
        backButton = root.Q<Button>("back-button");

        uploadButton.clicked += UploadButtonPressed;
        currentObjectsButton.clicked += CurrentObjectsButtonPressed;
        trainModelButton.clicked += TrainModelButtonPressed;
        backButton.clicked += BackButtonPressed;

        System.IO.Directory.CreateDirectory("Psuedo_Data/Database/Gallery");
        System.IO.Directory.CreateDirectory("Psuedo_Data/Database/Model");
    }

    void UploadButtonPressed()
    {
        SceneManager.LoadScene("UploadScreen");
    }

    void CurrentObjectsButtonPressed()
    {
        SceneManager.LoadScene("CurrentObjects");
    }

    void TrainModelButtonPressed()
    {
        //add functionality for training the model here
    }

    void BackButtonPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
