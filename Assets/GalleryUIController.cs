using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GalleryUIController : MonoBehaviour
{
    
    public Button backButton;
    
    // Start is called before the first frame update
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        backButton = root.Q<Button>("back-button");

        backButton.clicked += BackButtonPressed;
    }

    void BackButtonPressed()
    {
        SceneManager.LoadScene("MyImages");
    }
}
