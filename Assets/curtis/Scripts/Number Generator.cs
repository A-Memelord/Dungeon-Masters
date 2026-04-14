using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    public GameObject shopUI;
    public AudioSource shop;
    public AudioClip[] randomOpening;
    public AudioClip[] randomClosing;

    public void Interact()
    {
        if (shopUI == null)
            return;

        if (shopUI.activeSelf == false)
        {
            ActivateObject();
        }
        else
        {
            DeactivateObject();
        }
    }


    public void ActivateObject()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        shopUI.SetActive(true);
        shop.PlayOneShot(randomOpening[Random.Range(0, randomOpening.Length)]);
    }

    public void DeactivateObject()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        shopUI.SetActive(false);
        shop.PlayOneShot(randomClosing[Random.Range(0, randomClosing.Length)]);
    }
}
