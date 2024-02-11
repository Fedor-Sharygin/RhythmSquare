using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public bool bInputActive;
    private void Update()
    {
        if (!bInputActive)
        {
            return;
        }

        if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            LoadMainMenu();
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
