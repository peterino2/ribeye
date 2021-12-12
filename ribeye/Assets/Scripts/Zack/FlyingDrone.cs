using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDrone : MonoBehaviour {

    #region Declares

    //Declare serializables
    [SerializeField] private Animator animator = null;

    //Declare privates
    private int hash = 0;
    private int random = -1;
    private bool animationFinished = false;

    #endregion

    #region Constants

    private const string STATE = "State";

    #endregion

    #region Start and initialization

    private void Start() {
        //Set
        hash = Animator.StringToHash(STATE);
        //Play random animation
        PlayRandomAnimation();
    }

    #endregion

    #region Update

    private void Update() {
        //Check
        if (animationFinished) {
            //Reset
            animationFinished = false;
            //Play random animation
            PlayRandomAnimation();
        }
    }

    #endregion

    #region Animation

    private void PlayRandomAnimation() {
        //Declare
        int newRandom = GetRandom();
        //Get a different one
        while (newRandom == random) {
            //Set
            newRandom = GetRandom();
        }
        //Set
        random = newRandom;
        //Random animation
        animator.SetInteger(hash, random);
    }

    private int GetRandom() {
        //Return
        return Random.Range(0, 5);
    }

    public void AnimationFinishedNow() {
        //Set
        animationFinished = true;
    }

    #endregion

}