using System.Collections;
using UnityEngine;

public class KillCountTracker : MonoBehaviour
{
    public int killCount = 0;
    private bool lastStandDestroyed = false;
    

    public void IncrementKillCount()
    {
        killCount++;
        Debug.Log("Kill Count: " + killCount);

        if (!lastStandDestroyed && killCount >= 10)
        {
            DestroyLastStandCard();
            lastStandDestroyed = true;
            ResetKillCount();


        }
    }

    
    
    public void ResetKillCount()
    {
        killCount = 0;
        lastStandDestroyed = false;
    }

    private void DestroyLastStandCard()
    {
        Cards cards = FindFirstObjectByType<Cards>();
        if (cards == null || cards.LastStand == null)
            return;

        UiCard[] uiCards = FindObjectsByType<UiCard>(FindObjectsSortMode.None);
        foreach (var uiCard in uiCards)
        {
            if (uiCard.card == cards.LastStand)
            { 
                StartCoroutine(Wait(uiCard));
            }
        }
    }

    

    private IEnumerator Wait(UiCard uiCard)
    {
        Debug.Log("Before Destroy");
        yield return new WaitForSeconds(2f);
        Debug.Log("Destroying Last Stand card after 3 seconds.");
        Destroy(uiCard.gameObject);
        
    }
}
