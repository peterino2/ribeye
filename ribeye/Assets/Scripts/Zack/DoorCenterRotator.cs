using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCenterRotator : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private Animator animator = null;

    //Declare privates
    private int hash = 0;
    private bool rotatedOpen = false;

    #endregion

    #region Enumerators

    public enum AnimationStates {
        Idle, Open
    }

    #endregion

    #region Constants

    private const string STATE = "State";

    #endregion

    #region Start and initialization

    private void Start() {
        //Set
        hash = Animator.StringToHash(STATE);
    }

    #endregion

    #region Animation

    public void PlayAnimation(AnimationStates animationStateToBe) {
        //Set integer
        animator.SetInteger(hash, (int)animationStateToBe);
    }

    public void FinishedRotatedOpen() {
        //Set
        rotatedOpen = true;
    }

    #endregion

}