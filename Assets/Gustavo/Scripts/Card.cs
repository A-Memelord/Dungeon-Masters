using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Card : MonoBehaviour
{
    [SerializeField] public GameObject card1;
    [SerializeField] public GameObject card2;
    [SerializeField] public GameObject card3;

    
    private readonly Vector3[] slotLocalPositions = new Vector3[3];
    private readonly Vector3[] slotWorldPositions = new Vector3[3];
    private readonly bool[] slotIsUI = new bool[3];
    private readonly int[] slotSiblingIndices = new int[3];

    // current mapping of slot -> card (left = 0, middle = 1, right = 2)
    private readonly List<GameObject> currentOrder = new List<GameObject>(3);

    private void Start()
    {
        var rawCards = new List<GameObject> { card1, card2, card3 };
        var infos = new List<(GameObject go, float x, bool isUI, Vector3 localPos, Vector3 worldPos, int sibling)>(3);

        foreach (var go in rawCards)
        {
            if (go == null) continue;
            var rt = go.GetComponent<RectTransform>();
            bool isUI = rt != null;
            Vector3 local = isUI ? rt.localPosition : go.transform.localPosition;
            Vector3 world = go.transform.position;
            float x = isUI ? local.x : world.x;
            int sibling = go.transform.GetSiblingIndex();
            infos.Add((go, x, isUI, local, world, sibling));
        }

        
        infos.Sort((a, b) => a.x.CompareTo(b.x));

        for (int i = 0; i < infos.Count && i < 3; i++)
        {
            slotIsUI[i] = infos[i].isUI;
            slotLocalPositions[i] = infos[i].localPos;
            slotWorldPositions[i] = infos[i].worldPos;
            slotSiblingIndices[i] = infos[i].sibling;
            currentOrder.Add(infos[i].go);
        }
    }

    public void Shuffle(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // rotate left: middle -> left, right -> middle, left -> right
        var first = currentOrder[0];
        currentOrder[0] = currentOrder[1];
        currentOrder[1] = currentOrder[2];
        currentOrder[2] = first;

        ApplySlots();
    }

    private void ApplySlots()
    {
        var parent = currentOrder.Count > 0 ? currentOrder[0].transform.parent : null;
        int childCount = parent != null ? parent.childCount : 0;

        for (int i = 0; i < 3; i++)
        {
            var go = currentOrder.Count > i ? currentOrder[i] : null;
            if (go == null) continue;

            if (!go.activeSelf) go.SetActive(true);

            if (slotIsUI[i])
            {
                var rt = go.GetComponent<RectTransform>();
                if (rt != null)
                    rt.localPosition = slotLocalPositions[i];
                else
                    go.transform.position = slotWorldPositions[i];
            }
            else
            {
                go.transform.position = slotWorldPositions[i];
            }

            if (parent != null)
            {
                int targetIndex = Mathf.Clamp(slotSiblingIndices[i], 0, Mathf.Max(0, childCount - 1));
                go.transform.SetSiblingIndex(targetIndex);
            }
        }
    }

    //void DestroyFirstObject()
    //{
    //    if (currentOrder.Count > 0 && currentOrder[0] != null)
    //    {
    //        GameObject.Destroy(currentOrder[0]);
    //        currentOrder.RemoveAt(0);
    //        ApplySlots();
    //    }
        
    
    //}

    //public void UseCard(InputAction.CallbackContext context)
    //{
    //    DestroyFirstObject();

    //}
}

