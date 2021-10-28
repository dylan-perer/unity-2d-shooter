using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBG : MonoBehaviour
{
    //
    [SerializeField] private Vector2 parallaxEffectMutliplier;
    private Transform cammeraTransform;
    private Vector3 lastCammeraPostion;
    private void Start()
    {   
        cammeraTransform = Camera.main.transform;
        lastCammeraPostion = cammeraTransform.transform.position;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cammeraTransform.position - lastCammeraPostion;
        transform.position += new Vector3((deltaMovement.x * parallaxEffectMutliplier.x), deltaMovement.y * parallaxEffectMutliplier.y);
        lastCammeraPostion = cammeraTransform.position;

        // if(Mathf.Abs(cammeraTransform.position.x - transform.position.x) >= textureUnitSizeX){
        //     // float offsetPositionX = (cammeraTransform.position.x - transform.position.x) % textureUnitSizeX;
        //     transform.position = new Vector3(cammeraTransform.position.x, transform.position.y);
        // }
    }
}
