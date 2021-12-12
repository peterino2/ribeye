using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Stats;
using UnityEngine;

public class AnimationAttackHitFinished : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private AITriggerHitPlayer AITriggerHitPlayerScript = null;

    #endregion

    #region Animation functions

    private RibPlayer _player;
    private void Start()
    {
    }

    public void AnimationAttackHitFinishedNow(){
        //Check within
        if (AITriggerHitPlayerScript.PlayerWithinDamageZone()) {
            //HIT
            var x = FindObjectOfType<RibPlayer>();
            {
                x.TakeDamageFromSource(1, gameObject);
            }
        }
    }

    #endregion

}