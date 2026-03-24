using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] public GameObject GameObject;
    private float rotationSpeed = 50f;
    private float hoverSpeed = 0.5f;
    private float oldY;

    private void Awake()
    {
        oldY = transform.position.y;
    }

    private void Update()
    {
        this.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float newY = Mathf.Sin(Time.time * hoverSpeed) * 0.5f;
        this.transform.position = new Vector3(transform.position.x, oldY + newY, transform.position.z);
    }
}
