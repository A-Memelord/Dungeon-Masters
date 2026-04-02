using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cards : MonoBehaviour
{
    public Card LastStand;

    private void Update()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).TryGetComponent(out UiCard uiCard))
            {
                uiCard.card.onUpdate.Invoke(uiCard);
            }
        }

        if (transform.childCount == 0)
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

    public void RandomCard()
    {

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
            uiCard.card.onUse.Invoke(GameObject.FindWithTag("Player"));
        }
        DestroyFirstObject();

    }

    public void AddLastStand()
    {
        WorldCard.AddCardToInventory(LastStand);
        
    }

    //public void Burst()
    //{
    //    GameObject canvas = GameObject.Find("Canvas/Cards");
    //    if (canvas == null)
    //    {
    //        AddLastStand();
    //    }
    //}
}

