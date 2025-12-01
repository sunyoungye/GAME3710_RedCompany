using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start 버튼
    public void StartGame()
    {
        SceneManager.LoadScene("RedCompany");
    }

    // Exit 버튼
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit"); 
    }
}
