using TMPro;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public string soulTag;
    private int Coin = 0;

    public TextMeshProUGUI coinText;

    void Start()
    {

    }


    void Update()
    {

    }

    private void OnTriggerEnter(Collider soul)
    {
        if (soul.CompareTag(soulTag))
        {
            Coin++;
            coinText.text = "Souls: " + Coin.ToString();
            Debug.Log("Coin");
            Destroy(soul.gameObject);
        }
    }
}
