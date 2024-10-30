using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

public class NavigationController : MonoBehaviour
{
    public NavigationManager navigationManager;
    public void NavigationButtonPressed()
    {
        SceneManager.LoadScene("SavedLocations");
        navigationManager.arSession.enabled = false;
    }
}