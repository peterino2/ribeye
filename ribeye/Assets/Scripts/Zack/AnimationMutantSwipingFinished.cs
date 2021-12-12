using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMutantSwipingFinished : MonoBehaviour {
    

    public bool AnimationHasFinished = false;

    public void MutantSwipingHasFinishedNow() {
        //Set
        AnimationHasFinished = true;
    }

}