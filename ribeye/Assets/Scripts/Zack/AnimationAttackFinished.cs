using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAttackFinished : MonoBehaviour {

    public bool AnimationHasFinished = false;

    public void AttackHasFinishedNow() {
        //Set
        AnimationHasFinished = true;
    }

}