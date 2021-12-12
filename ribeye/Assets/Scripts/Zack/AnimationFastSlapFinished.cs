using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Stats;
using UnityEngine;

public class AnimationFastSlapFinished : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private AITriggerHitPlayer AITriggerHitPlayerScript = null;

    [SerializeField] private AIController _controller;

    #endregion

    #region Animation functions

    private RibPlayer _player;
    private void Start()
    {
    }

    public void AnimationFastSlapAttackPoint(){
        //Check within
        if (AITriggerHitPlayerScript.PlayerWithinDamageZone()) {
            //HIT
            print("FastSlap");
            var x = FindObjectOfType<RibPlayer>();
            {
                x.TakeDamageFromSource(1, gameObject);
            }
        }
    }
    
    public void AnimationFastSlapEndPoint()
    {
        _controller.FastSlapFinished = true;
    }

    #endregion

}
