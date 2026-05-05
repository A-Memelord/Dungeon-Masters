using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private void Start()
    {
        FindFirstObjectByType<PlayerMovement>().transform.position = transform.position;
        FindFirstObjectByType<PlayerMovement>().GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }
}
