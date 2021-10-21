using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    [SerializeField] private List<MirrorAnimation> mirrorAnimatons;
    [SerializeField] private Rigidbody2D upperBodyRb;
    void Start()
    {
        upperBodyRb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            if (!mirrorAnimatons[0].getRagdollStatus())
            {
                upperBodyRb.bodyType = RigidbodyType2D.Dynamic;
                upperBodyRb.constraints = RigidbodyConstraints2D.None;
                foreach (MirrorAnimation mirrorAnimaton in mirrorAnimatons)
                {
                    mirrorAnimaton.setRagdoll(true);
                }
            }
            else
            {
                upperBodyRb.SetRotation(90f);
                upperBodyRb.bodyType = RigidbodyType2D.Kinematic;
                upperBodyRb.constraints = RigidbodyConstraints2D.FreezeRotation;
                foreach (MirrorAnimation mirrorAnimaton in mirrorAnimatons)
                {
                    mirrorAnimaton.setRagdoll(false);
                }
            }


        }

    }
}

