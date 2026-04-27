using System.Collections.Generic;
using UnityEngine;

public class moov : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float amplitudeX = 1f;
    [SerializeField] private float speedX = 1f;

    private float startX;

    private void Awake()
    {
        startX = transform.position.x;
    }

    private void Update()
    {
       
        float newX = startX + Mathf.Sin(Time.time * speedX) * amplitudeX;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }
}