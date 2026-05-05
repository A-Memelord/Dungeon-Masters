using UnityEngine;

public class SwordAnim : MonoBehaviour
{

    private Animator anim;
    private bool isAttacking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isAttacking = !isAttacking;

            anim.SetBool("IsAttacking", isAttacking);
        }
    }
}
