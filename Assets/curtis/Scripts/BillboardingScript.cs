using UnityEngine;

public class BillboardingScript : MonoBehaviour
{

    void Start()
    {
        
    }


    void Update()
    {
        transform.forward = -Camera.main.transform.forward;
    }
}
