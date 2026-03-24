using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
public class WorldCard : MonoBehaviour
{
    public Card card;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = card.sprite;
        GetComponent<Animator>().runtimeAnimatorController = card.animatorController;
    }

    private void OnValidate()
    {
        Start();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);

            GameObject card = GameObject.Find("Canvas/Cards");
            if (card != null)
            {
                UiCard uiCard = new GameObject("Card").AddComponent<UiCard>();

                uiCard.card = this.card;
                uiCard.transform.SetParent(card.transform);

                if (uiCard.TryGetComponent(out RectTransform rectTransform))
                {
                    rectTransform.anchoredPosition3D = Vector3.zero;
                    rectTransform.localScale = Vector3.one;
                    rectTransform.localRotation = Quaternion.identity;
                }

                if(uiCard.TryGetComponent(out Image image))
                {
                    image.preserveAspect = true;
                }

                uiCard.Assign();
            }
        }
    }
}
