using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed, jumpForce, inputX, inputY;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private Transform mirrorAnimationSpine, physicsSpine;
    [SerializeField] private Animator animator;
    [SerializeField]private Limit[] leftfacingAngleLimits;
    private Rigidbody2D rb;
    private Rigidbody2D[] childRbs;
    private HingeJoint2D[] childJoints;
    private List<JointAngleLimits2D> rightfacingAngleLimits = new List<JointAngleLimits2D>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        childRbs = GetComponentsInChildren<Rigidbody2D>();
        childJoints = GetComponentsInChildren<HingeJoint2D>();

        //store rightfacing angle limits
        foreach (HingeJoint2D childJoint in childJoints)
        {
            rightfacingAngleLimits.Add(childJoint.limits);
        }
    }


    void FixedUpdate()
    {
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        rb.velocity = new Vector2(inputX * speed, rb.velocity.y);

        xAnimationHandler(inputX);


    }

    private void xAnimationHandler(float inputX)
    {
        if (inputX == 0)
        {
            animator.SetBool("walk", false);
            animator.SetBool("walkBack", false);
        }
        else if (inputX > 0)
        {
            animator.SetBool("walk", true);
            animator.SetBool("walkBack", false);

            if (!isFacingRight)
            {
            //    flipCharacter();
            }

        }
        else if (inputX < 0)
        {
            animator.SetBool("walkBack", true);
            animator.SetBool("walk", false);
            if (isFacingRight)
            {
                // flipCharacter();
            }
        }
    }

    private void flipCharacter()
    {
        isFacingRight = !isFacingRight;

        //make child rbs kinematic and use limits to false
        // foreach (Rigidbody2D childRb in childRbs)
        // {
        //     childRb.isKinematic = true;
        // }

        // for (int i = 0; i < childJoints.Length; i++)
        // {
        //     // childJoints[i].useLimits = false;
        // }


        //flip mirror
        Vector3 mirrorAnimationSpineScaler = mirrorAnimationSpine.localScale;
        mirrorAnimationSpineScaler.y *= -1;
        mirrorAnimationSpine.localScale = mirrorAnimationSpineScaler;

        //flip physics body
        Vector3 physicsSpineScaler = physicsSpine.localScale;
        physicsSpineScaler.y *= -1;
        physicsSpine.localScale = physicsSpineScaler;

        //make child rbs NOT kinematic and use set stored limits
        // foreach (Rigidbody2D childRb in childRbs)
        // {
        //     childRb.isKinematic = false;
        // }

        // for (int i = 0; i < childJoints.Length; i++)
        // {
        //     if (isFacingRight)
        //     {
        //         childJoints[i].limits = rightfacingAngleLimits[i];
        //     }
        //     else
        //     {
        //         JointAngleLimits2D tmp = new JointAngleLimits2D();
        //         tmp.min = leftfacingAngleLimits[i].min;
        //         tmp.max = leftfacingAngleLimits[i].max;

        //         childJoints[i].limits = tmp;
        //     }
        //     childJoints[i].useLimits = true;
        // }
    }
    [System.Serializable]
    private class Limit{
        public float min,max;
    }

}
