using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    [SerializeField] bool mobileMode = false;
    [SerializeField] float xSpeed = 6, jumpheight = 4, timeToJumpApex = .4f, accelerationTimeground = .1f, accelerationTimeAirbone = .2f;
    [SerializeField] private Vector3 velocity;
    private Controller2D controller;
    private float jumpVelocity, gravity, velocityXSmoothing;
    private Animator animator;
    private bool isFacingRight = true, isRagdoll = false, touchJump = false;
    private Transform playerTransfom;
    [SerializeField] private Vector2 input;
    [SerializeField] private Vector2 jumpUiScreenMultiplier, jumpUiSize;

    //Ragdoll
    [SerializeField] private Transform skeleton;
    private Rigidbody2D[] rigidbody2Ds;
    private HingeJoint2D[] hingeJoint2Ds;
    private CapsuleCollider2D[] capsuleCollider2Ds;
    private IKManager2D iKManager2D;

    //Ik Aim
    [SerializeField] private float aimOffset, flipOffset;
    [SerializeField] private Vector3 mousePosition, rootPostition;
    [SerializeField] Joystick moveJoystick, aimJoystick;
    [SerializeField] GameObject moveJoystickObj, aimJoystickObj;

    private float xAnimVelocity;


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

        moveJoystick.enabled = mobileMode;
        aimJoystick.enabled = mobileMode;

        moveJoystickObj.SetActive(mobileMode);
        aimJoystickObj.SetActive(mobileMode);

        animator.SetBool("mobileMode", mobileMode);
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
            if (mobileMode)
            {
                input.x = moveJoystick.Horizontal;
                input.y = aimJoystick.Horizontal;
            }

            xDirectionAnimationHandler(input);
            yDirectionAnimationHandler(input);

            jumpInputKey();
            if (touchJump)
            {
                jump();

            }

            float targetVelocityX = input.x * xSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeground : accelerationTimeAirbone));
            velocity.y += gravity * Time.deltaTime;
            controller.move(velocity * Time.deltaTime);

            if (controller.collisions.left || controller.collisions.right)
            {
                velocity.x = 0f;
            }
            xAnimVelocity = velocity.x;
            if (!isFacingRight)
            {
                xAnimVelocity *= -1;
            }
            animator.SetFloat("xVelocity", xAnimVelocity);
            animator.SetFloat("yVelocity", velocity.y);

        }


    }

    void LateUpdate()
    {
        //IK AIm

        animator.SetFloat("xAim", mousePosition.x);

        if (mobileMode)
        {
            animator.SetFloat("yAim", aimJoystick.Vertical);

            if (aimJoystick.Horizontal > 0 && !isFacingRight)
            {
                flip();
            }
            else if (aimJoystick.Horizontal < 0 && isFacingRight)
            {
                flip();
            }
        }
        else
        {
            animator.SetFloat("yAim", (mousePosition.y - rootPostition.y) + aimOffset);


            float mousePlayerDiff = mousePosition.x - rootPostition.x;
            if (mousePlayerDiff > 0 + flipOffset && !isFacingRight)
            {
                flip();
            }
            else if (mousePlayerDiff < 0 - flipOffset && isFacingRight)
            {
                flip();
            }
        }

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
        rootPostition = skeleton.transform.position;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }

    private void flip()
    {
        isFacingRight = !isFacingRight;
        playerTransfom.localScale = new Vector3(-1 * playerTransfom.localScale.x, playerTransfom.localScale.y, playerTransfom.localScale.z);
    }
    private void yDirectionAnimationHandler(Vector2 input)
    {
        animator.SetBool("jump", !controller.collisions.below);
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = new Color(1, 0, 0, .5f);
    //     Gizmos.DrawCube(new Vector3(Screen.width * jumpUiScreenMultiplier.x - (jumpUiSize.x / 2), Screen.height * jumpUiScreenMultiplier.y), new Vector3(jumpUiSize.x, jumpUiSize.y));
    // }

    private void jumpInputKey()
    {
        //Touch controll
        if (Input.touchCount > 0 && controller.collisions.below)
        {

            Rect r = new Rect(
             Screen.width * jumpUiScreenMultiplier.x, Screen.height * jumpUiScreenMultiplier.y,
             Screen.width * jumpUiSize.x, Screen.height * jumpUiSize.y);
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    var pos = touch.position;
                    pos.y = Screen.height - pos.y;  // must invert Y position

                    if (r.Contains(pos))
                    {
                        jump();
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.Space) && controller.collisions.below)
        {
            jump();
        }
        // if (moveJoystick.Vertical >= .4 && controller.collisions.below)
        // {
        //     velocity.y = jumpVelocity;
        //     animator.SetBool("jump", true);
        // }


    }
    private void jump()
    {
        if (controller.collisions.below)
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
