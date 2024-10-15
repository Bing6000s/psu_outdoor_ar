using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationController : MonoBehaviour
{
    public void NavigationButtonPressed()
    {
        SceneManager.LoadScene("SavedLocations");
    }
}