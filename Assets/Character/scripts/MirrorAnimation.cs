using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorAnimation : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float force = 5000;
    [SerializeField] private float targetRot;
    [SerializeField] private Rigidbody2D rb2d;
    private bool isRagdoll;

    public void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        if (!isRagdoll)
        {
            targetRot = target.eulerAngles.z;
            rb2d.MoveRotation(Mathf.LerpAngle(rb2d.rotation, targetRot, force * Time.fixedDeltaTime));
        }
    }

    public void setRagdoll(bool b){
        isRagdoll = b;
    }

    public bool getRagdollStatus(){
        return isRagdoll;
    }

}