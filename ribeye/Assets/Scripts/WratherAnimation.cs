using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WratherAnimation : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private Animator theAnimator = null;
    [SerializeField] private string parameterName = "State";

    [Header("Debugging")]
    [SerializeField] private int state = 0;

    //Declare privates
    private int hash = 0;

    #endregion

    #region Start and initialization

    private void Start() {
        //Set hash
        hash = Animator.StringToHash(parameterName);
    }

    #endregion

    #region Update

    private void Update() {
        
    }

    private void PlayAnimation(int stateIndex) {
        //Set
        state = stateIndex;
        //Play animation
        theAnimator.SetInteger(hash, state);
    }

    #endregion

}