using UnityEngine;

public class FireCard : MonoBehaviour
{
    
    [SerializeField] private Cards cards;

    private void OnUse()
    {
        Debug.Log($"{name} was used (OnDestroy called).");

        if (cards != null)
        {
            Debug.Log($"Referenced Cards instance: {cards.name}");
        }
    }
}
