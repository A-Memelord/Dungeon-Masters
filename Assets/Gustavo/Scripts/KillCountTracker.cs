using UnityEngine;

public class KillCountTracker : MonoBehaviour
{
    private int killCount = 0;


    public void IncrementKillCount()
    {
        killCount++;
        Debug.Log("Kill Count: " + killCount);
    }

    private void Update()
    {
        if (killCount <= 10 )
        {
           IncrementKillCount();
        }
            
    }






}
