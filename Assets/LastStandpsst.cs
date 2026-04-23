using UnityEngine;

public class LastStandpsst : MonoBehaviour
{
    
    private bool IsLastStandActive()
    {
        var cards = FindFirstObjectByType<Cards>();
        if (cards == null || cards.LastStand == null) return false;

        UiCard[] uiCards = Object.FindObjectsByType<UiCard>(FindObjectsSortMode.None);
        foreach (var uiCard in uiCards)
        {
            if (uiCard.card == cards.LastStand && uiCard.gameObject.activeInHierarchy)
                return true;
        }

        return false;
    }
}
