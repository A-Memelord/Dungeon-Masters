using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            container.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1;
    }


    public void MainMenuButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
