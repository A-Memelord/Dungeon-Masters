using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenu : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}