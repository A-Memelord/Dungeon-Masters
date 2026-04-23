using System.Collections;
using UnityEngine;

public class KillCountTracker : MonoBehaviour
{
    public int killCount = 0;
    private bool lastStandDestroyed = false;
    private StatusEffectData _data;


 
    public float lastStandHealPerKill = 10f;

    public void IncrementKillCount()
    {
        // Only count kills while LastStand is present in the player's UI.
        if (!IsLastStandInUI())
        {
            Debug.Log("LastStand not present in UI — kill not counted.");
            return;
        }

        killCount++;
        Debug.Log("Kill Count: " + killCount);

        
        var player = GameObject.FindWithTag("Player");
        if (player != null && player.TryGetComponent(out Health health))
        {
            health.Heal(lastStandHealPerKill);
            Debug.Log($"Player healed by {lastStandHealPerKill} due to LastStand kill.");
        }

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

        UiCard[] uiCards = Object.FindObjectsByType<UiCard>(FindObjectsSortMode.None);
        foreach (var uiCard in uiCards)
        {
            if (uiCard.card == cards.LastStand)
            {
                
                StartCoroutine(WaitAndReplace(uiCard, cards));
            }
        }

    }

    private IEnumerator WaitAndReplace(UiCard uiCard, Cards cards)
    {
        Debug.Log("Before Destroy");
        yield return new WaitForSeconds(2f);
        Debug.Log("Destroying Last Stand card after 1 seconds.");
        Destroy(uiCard.gameObject);

       
        yield return new WaitForEndOfFrame();

        // Add 4 random cards to the UI via the Cards instance
        cards.AddRandomCards(4);

        // Immediately mark LastStand as no longer present so systems (MeleeCombat) revert to normal
        cards.LastStandAdded = false;

        var player = GameObject.FindWithTag("Player");
        if (player != null && player.TryGetComponent(out IEffectable effectable))
        {
            effectable.RemoveEffect();
        }
    }

   
    private bool IsLastStandInUI()
    {
        var cards = FindFirstObjectByType<Cards>();
        if (cards == null || cards.LastStand == null)
            return false;

        UiCard[] uiCards = Object.FindObjectsByType<UiCard>(FindObjectsSortMode.None);
        foreach (var uiCard in uiCards)
        {
            if (uiCard.card == cards.LastStand && uiCard.gameObject.activeInHierarchy)
                return true;
        }

        return false;
    }
}
