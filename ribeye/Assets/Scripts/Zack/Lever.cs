using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private Animator animator = null;
    [SerializeField] private SlidingDoors slidingDoors = null;

    [Header("Testing")]
    [SerializeField] private bool Testing = false;

    //Declare privates
    private int hash = 0;

    #endregion

    #region Constants

    //Declare private constants
    private const string STATE = "State";

    #endregion

    #region Start and initialization

    private void Start() {
        //Set
        hash = Animator.StringToHash(STATE);
    }

    #endregion

    #region Update

    public void OpenDoor()
    {
        animator.SetInteger(hash, 1);
    }
    
    #endregion

    #region Public functions

    public void DoorLeverDone() {
        //Set state
        slidingDoors.state = SlidingDoors.States.Opening;
    }

    #endregion

}