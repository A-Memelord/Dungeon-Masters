using System.Collections;
using UnityEngine;

public class Destroy : MonoBehaviour
{

    public GameObject DestroyPrefab;
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(waitToDestory());
        Debug.Log("Collided"+collision.gameObject.name);
    }

    public IEnumerator waitToDestory()
    {
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }
}
