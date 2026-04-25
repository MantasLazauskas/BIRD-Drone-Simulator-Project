using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(string sceneName)
    {
        SceneManager.LoadScene("GameScene"); //change to game scene name
    }

    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}


