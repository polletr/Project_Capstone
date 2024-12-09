using UnityEngine;

public class ToMenu : MonoBehaviour
{
    public void LoadMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void EndGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndMenu");
    }
}
