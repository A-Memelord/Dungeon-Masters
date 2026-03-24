using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cards : MonoBehaviour
{
    public void Shuffle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        List<int> Indices = Enumerable.Range(0, transform.childCount).OrderBy(card => Random.Range(-1f, 1f)).ToList();
        for (int i = 0; i < Indices.Count; ++i)
        {
            transform.GetChild(i).SetSiblingIndex(Indices[i]);
        }
    }


    void DestroyFirstObject()
    {
        Destroy(transform.GetChild(transform.childCount - 1).gameObject);

    }

    public void UseCard(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (transform.GetChild(transform.childCount - 1).TryGetComponent(out UiCard uiCard))
        {
            uiCard.card.onUse.Invoke();
        }
        DestroyFirstObject();

    }
}

