using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation_Button : MonoBehaviour
{
    public void NavigationButtonPressed()
    {
        SceneManager.LoadScene("NavigationScene");
    }
}