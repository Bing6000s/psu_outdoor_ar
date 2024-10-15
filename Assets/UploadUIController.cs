using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.IO;

public class UploadUIController : MonoBehaviour
{
    
    public Button uploadButton;
    public Button createLabelButton;
    public Button backButton;
    public DropdownField labelsDropdown;
    public TextField newLabel;
    public List<string> dropdownChoices = new List<string>();
    public string rootFolder = "Psuedo_Data/Database";
    string modelFile = "Psuedo_Data/Model";

    
    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        uploadButton = root.Q<Button>("upload-button");
        createLabelButton = root.Q<Button>("create-label-button");
        backButton = root.Q<Button>("back-button");
        labelsDropdown = root.Q<DropdownField>("labels-dropdown");
        newLabel = root.Q<TextField>("new-label");

        UpdateDropdownChoices();

        uploadButton.clicked += UploadButtonPressed;
        createLabelButton.clicked += CreateLabelButtonPressed;
        backButton.clicked += BackButtonPressed;
    }

    void UploadButtonPressed()
    {
        //add functionality for uploading images here
        if(labelsDropdown.value != null){
            string selectedLabel = (string) labelsDropdown.value; //this is the label that the user has selected in the dropdown field

        }
    }

    void CreateLabelButtonPressed()
    {
        string labelValue = (string) newLabel.value;

        if(!Directory.Exists("Psuedo_Data/Database/Gallery/" + labelValue)){
        
            System.IO.Directory.CreateDirectory("Psuedo_Data/Database/Gallery/" + labelValue);
            System.IO.Directory.CreateDirectory("Psuedo_Data/Database/Model/" + labelValue);

            newLabel.value = "";

            UpdateDropdownChoices();
        }
    }

    void BackButtonPressed()
    {
        SceneManager.LoadScene("MyImages");
    }

    void UpdateDropdownChoices()
    {
        List<string> folders = new List<string>(Directory.GetDirectories(rootFolder + "/Gallery"));
        dropdownChoices = new List<string>();

        for(int i = 0; i < folders.Count; i++){
            dropdownChoices.Add(new DirectoryInfo(folders[i]).Name);
        }

        labelsDropdown.choices = dropdownChoices;
    }


}
