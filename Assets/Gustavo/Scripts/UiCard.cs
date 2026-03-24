using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Animator))]
public class UiCard : MonoBehaviour
{
    public Card card;

    void Start()
    {
        if (card != null)
        {
            GetComponent<Image>().sprite = card.sprite;
            GetComponent<Animator>().runtimeAnimatorController = card.animatorController;
        }
    }

    public void Assign()
    {
        GetComponent<Image>().sprite = card.sprite;
        GetComponent<Animator>().runtimeAnimatorController = card.animatorController;
    }

    private void OnValidate()
    {
        Start();
    }
}
