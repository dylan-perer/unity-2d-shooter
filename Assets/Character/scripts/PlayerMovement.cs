using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed, jumpForce, inputX, inputY;
    [SerializeField] private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(inputX * speed, rb.velocity.y);

        if (inputX == 0)
        {
            animator.SetBool("walk", false);
            animator.SetBool("walkBack", false);


        }
        else if (inputX > 0)
        {
            animator.SetBool("walk", true);
            animator.SetBool("walkBack", false);
        }
        else if (inputX < 0)
        {
            animator.SetBool("walkBack", true);
            animator.SetBool("walk", false);
        }
    }

    private void Run(Vector2 dir)
    {

    }




}
