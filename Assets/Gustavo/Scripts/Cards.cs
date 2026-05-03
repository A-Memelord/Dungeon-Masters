using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cards : MonoBehaviour
{
    public Card LastStand;
    [SerializeField] StatusEffectData _data;
    
    public Card[] CardPool;

    [SerializeField] private float lastStandCooldown = 6f;
    [HideInInspector] public bool LastStandAdded = false;

    private void Update()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).TryGetComponent(out UiCard uiCard))
            {
                uiCard.card.onUpdate.Invoke(uiCard);
            }
        }

        
        if (transform.childCount == 0 && !LastStandAdded)
        {
            AddLastStand();
        }
        
    
    }

    public void Shuffle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        List<int> Indices = Enumerable.Range(0, transform.childCount).OrderBy(card => Random.Range(-1f, 1f)).ToList();
        for (int i = 0; i < Indices.Count; ++i)
        {
            transform.GetChild(i).SetSiblingIndex(Indices[i]);
        }
    }



    public void DestroyFirstObject()
    {
        Destroy(transform.GetChild(transform.childCount - 1).gameObject);

    }

    public void UseCard(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (transform.GetChild(transform.childCount - 1).TryGetComponent(out UiCard uiCard))
        {
            var player = GameObject.FindWithTag("Player");

            // Let the UiCard handle decrementing uses. Only remove when depleted.
            bool shouldDestroy = uiCard.Use(player);
            if (shouldDestroy)
            {
                DestroyFirstObject();
            }
        }
    }

    
    public void AddLastStand()
    {
        if (LastStandAdded)
        {
            
            return;
        }

        WorldCard.AddCardToInventory(LastStand);
        LastStandAdded = true;

        
        

        var effectable = GetComponent<IEffectable>();
        if (effectable != null)
        {
            effectable.ApplyEffect(_data);
        }

        Debug.Log("Added Last Stand to inventory and applied effect.");
    }

    
    public void StartLastStandCooldown(float delay)
    {
        StartCoroutine(WaitForLastStand(delay));
    }

    private IEnumerator WaitForLastStand(float delay)
    {
        yield return new WaitForSeconds(delay);
        LastStandAdded = false;
        Debug.Log("Last Stand can now be added again.");
    }

   
    public void AddRandomCards(int count)
    {
        if (CardPool == null || CardPool.Length == 0)
        {
            Debug.LogWarning("Add cards via inspector");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            var idx = Random.Range(0, CardPool.Length);
            WorldCard.AddCardToInventory(CardPool[idx]);
        }
    }
}

