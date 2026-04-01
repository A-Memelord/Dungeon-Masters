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

    private void Update()
    {
        GetComponent<Animator>().SetFloat("Health", FindFirstObjectByType<Health>().health);
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
