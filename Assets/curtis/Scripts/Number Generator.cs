using System.Collections;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    public Transform uiHolder;
    public GameObject shopUI;
    public AudioSource shop;
    public AudioClip[] randomOpening;
    public AudioClip[] randomClosing;
    public Animator anim;

    void Start()
    {
        uiHolder = GameObject.FindWithTag("shopUI").transform;
        shopUI = uiHolder.transform.Find("Holder").gameObject;
    }
    public void Interact()
    {
        if (shopUI == null)
        {
            shopUI = GameObject.FindWithTag("shopUI");
        }
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
        AudioClip clip = randomOpening[Random.Range(0, randomOpening.Length)];
        shop.PlayOneShot(clip);
        StartCoroutine(Talking(clip));
        anim.Play("Goblin Talking");
    }

    public void DeactivateObject()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        shopUI.SetActive(false);
        AudioClip clip = randomClosing[Random.Range(0, randomClosing.Length)];
        shop.PlayOneShot(clip);
        StartCoroutine(Talking(clip));
        anim.Play("Goblin talking");
    }

    private IEnumerator Talking(AudioClip clip)
    {
            anim.SetBool("isTalking", true);

        yield return new WaitForSeconds(clip.length);

            anim.SetBool("isTalking", false); 
    }
}