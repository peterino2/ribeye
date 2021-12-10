using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationFinished : MonoBehaviour {

    public bool AnimationHasFinished = false;

    public void AnimationHasFinishedNow() {
        //Set
        AnimationHasFinished = true;
    }

}