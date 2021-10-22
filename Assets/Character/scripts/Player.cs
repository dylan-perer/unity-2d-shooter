using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [SerializeField] float xSpeed = 6, jumpheight = 4, timeToJumpApex = .4f, accelerationTimeground = .1f, accelerationTimeAirbone = .2f;
    [SerializeField] private Vector3 velocity;
    private Controller2D controller;
    private float jumpVelocity, gravity, velocityXSmoothing;
    private Animator animator;
    [SerializeField] private Vector2 input; 
    void Start()
    {
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();

        gravity = -(2 * jumpheight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        isCollidingVertically();
        input = asignInput();

        xDirectionAnimationHandler(input);
        yDirectionAnimationHandler(input);

        jumpInputKey();

        float targetVelocityX = input.x * xSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeground : accelerationTimeAirbone));
        velocity.y += gravity * Time.deltaTime;
        controller.move(velocity * Time.deltaTime);

        if(controller.collisions.left || controller.collisions.right){
            velocity.x = 0f;
        }
        animator.SetFloat("xVelocity", velocity.x);
        animator.SetFloat("yVelocity", velocity.y);
        print(velocity.y);
        print("BELOW: "+controller.collisions.below);
    }

    private bool isCollidingVertically()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    private Vector2 asignInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void xDirectionAnimationHandler(Vector2 input)
    {
        if (input.x > 0 && controller.collisions.below)
        {
            animator.SetBool("walk", true);
            animator.SetBool("walkBack", false);
        }
        else if (input.x < 0 && controller.collisions.below)
        {
            animator.SetBool("walkBack", true);
            animator.SetBool("walk", false);
        }
        else
        {
            animator.SetBool("walk", false);
            animator.SetBool("walkBack", false);
        }
    }

    private void yDirectionAnimationHandler(Vector2 input)
    {
        animator.SetBool("inAir", !controller.collisions.below);
    }

    private void jumpInputKey()
    {
        if (Input.GetKey(KeyCode.Space) && controller.collisions.below)
        {   
            velocity.y = jumpVelocity;
        }
    }

    public Vector2 getInput(){
        return input;
    }
}
