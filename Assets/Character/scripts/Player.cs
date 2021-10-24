using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [SerializeField] float xSpeed = 6, jumpheight = 4, timeToJumpApex = .4f, accelerationTimeground = .1f, accelerationTimeAirbone = .2f;
    [SerializeField] private Vector3 velocity;
    private Controller2D controller;
    private float jumpVelocity, gravity, velocityXSmoothing;
    private Animator animator;
    private bool isFacingRight = true, isRagdoll = false;
    private Transform playerTransfom;
    [SerializeField] private Vector2 input;

    //Ragdoll
    [SerializeField] private Transform skeleton;
    private Rigidbody2D[] rigidbody2Ds;
    private HingeJoint2D[] hingeJoint2Ds;
    private CapsuleCollider2D[] capsuleCollider2Ds;
    private IKManager2D iKManager2D;

    //Ik Aim
    [SerializeField] private Transform r_UpperArm;
    [SerializeField]
    private float aimOffset
     = -90f;
    private Vector3 startingSize;
    private Vector3 armStartingSize;

    void Start()
    {
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        playerTransfom = GetComponent<Transform>();

        gravity = -(2 * jumpheight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        //disable physics on skeleton to play animation
        rigidbody2Ds = skeleton.GetComponentsInChildren<Rigidbody2D>();
        hingeJoint2Ds = skeleton.GetComponentsInChildren<HingeJoint2D>();
        capsuleCollider2Ds = skeleton.GetComponentsInChildren<CapsuleCollider2D>();
        iKManager2D = GetComponent<IKManager2D>();

        // toggleRagdoll();
        enableBodyPhysics(false);

        //Ik Aim
        startingSize = transform.localScale;
        armStartingSize = r_UpperArm.localScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            toggleRagdoll();
        }
        if (!isRagdoll)
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

            if (controller.collisions.left || controller.collisions.right)
            {
                velocity.x = 0f;
            }
            animator.SetFloat("xVelocity", velocity.x);
            animator.SetFloat("yVelocity", velocity.y);

        }


    }

    void LateUpdate()
    {
        //IK AIm
        Vector3 skelPos = skeleton.transform.position;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        print("skelPos - mousePos"+(mousePos));
        animator.SetFloat("xAim",mousePos.x);
        animator.SetFloat("yAim",mousePos.y);

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

        if (input.x > 0 && !isFacingRight)
        {
            flip();
        }
        else if (input.x < 0 && isFacingRight)
        {
            flip();
        }
    }

    private void flip()
    {
        isFacingRight = !isFacingRight;
        playerTransfom.localScale = new Vector3(-1 * playerTransfom.localScale.x, playerTransfom.localScale.y, playerTransfom.localScale.z);
    }
    private void yDirectionAnimationHandler(Vector2 input)
    {
        // animator.SetBool("inAir", !controller.collisions.below);
        animator.SetBool("jump", !controller.collisions.below);
    }

    private void jumpInputKey()
    {
        if (Input.GetKey(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
            animator.SetBool("jump", true);
        }
    }

    public Vector2 getInput()
    {
        return input;
    }

    private void toggleRagdoll()
    {
        isRagdoll = !isRagdoll;

        enableBodyPhysics(isRagdoll);

        animator.enabled = !isRagdoll;
        iKManager2D.enabled = !isRagdoll;
        controller.playerBoxCollider.enabled = !isRagdoll;

    }

    private void enableBodyPhysics(bool b)
    {
        foreach (Rigidbody2D rigidbody2D in rigidbody2Ds)
        {
            rigidbody2D.simulated = b;
        }
        foreach (HingeJoint2D hingeJoint2D in hingeJoint2Ds)
        {
            hingeJoint2D.enabled = b;
        }
        foreach (CapsuleCollider2D capsuleCollider2D in capsuleCollider2Ds)
        {
            capsuleCollider2D.enabled = b;
        }
    }
}
