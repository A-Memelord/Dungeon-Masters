using UnityEngine;

public class SwordLSAnim : MonoBehaviour
{

    private Animator anim;
    private bool isAttacking = false;
    public GameObject[] SwordVariant;
    public GameObject[] swordBase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            PlayAttack();

        }
    }

    private void PlayAttack()
    {
        if (anim != null)
        {
            // Reset trigger to avoid stacking
            anim.ResetTrigger("Attacking");
            anim.SetTrigger("Attacking");
        }
    }

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

    private GameObject[] GetActiveSwords()
    {
        bool lastStand = IsLastStandActive();
        if (lastStand && SwordVariant != null && SwordVariant.Length > 0)
            return SwordVariant;
        return swordBase;
    }

    private void UpdateSwordVisibility()
    {
        GameObject[] activeSwords = GetActiveSwords();
        foreach (var sword in swordBase)
        {
            sword.SetActive(System.Array.Exists(activeSwords, s => s == sword));
        }
    }
}
