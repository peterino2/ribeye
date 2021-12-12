using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAttackHitFinished : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private AITriggerHitPlayer AITriggerHitPlayerScript = null;

    #endregion

    #region Animation functions

    public void AnimationAttackHitFinishedNow(){
        //Check within
        if (AITriggerHitPlayerScript.PlayerWithinDamageZone()) {
            //HIT
            Debug.Log("AI HIT PLAYER");
        }
    }

    #endregion

}