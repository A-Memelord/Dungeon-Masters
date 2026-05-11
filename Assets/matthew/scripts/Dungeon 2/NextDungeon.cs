using UnityEngine;
using UnityEngine.SceneManagement;

public class NextDungeon : MonoBehaviour
{
    public DungeronGenerator2 dunGen;
    bool hasTriggered = false;
    public bool endDungeon = false;

    private void Start()
    {
        dunGen = FindFirstObjectByType<DungeronGenerator2>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (endDungeon == true)
        {
            SceneManager.LoadScene("Menu");
        }
        if (endDungeon == false)
        {
            if (!hasTriggered) 
            {
                Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                dunGen.GenNextDungeon();
                hasTriggered = true;
            }
            if (!hasTriggered)
            {
                hasTriggered = false; 
            }  
        }
    }
}
