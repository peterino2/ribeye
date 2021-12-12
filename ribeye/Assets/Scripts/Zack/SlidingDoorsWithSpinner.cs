using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlidingDoorsWithSpinner : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private Door[] doors = null;
    [SerializeField] private DoorCenterRotator doorCenterRotator = null;
    [SerializeField] private Trigger trigger = null;
    [SerializeField] private Lever lever = null;

    [Header("Specifications")]
    [SerializeField] private float speed = 25f;

    [Header("Debugging")]
    public States state = States.Closed;
    public DoorCenterStates doorCenterState = DoorCenterStates.Closed;

    #endregion

    #region Enumerators

    public enum States {
        Closed, Opening, Open, Closing
    }

    public enum DoorCenterStates {
        Closed, Opening, Open, Closing
    }

    #endregion

    #region Classes

    [Serializable]
    private class Door {
        //Declare
        public Transform DoorTransform = null;
        public Transform DoorClosed = null;
        public Transform DoorOpen = null;
    }

    #endregion

    #region Update

    private void Update() {
        ////Check state
        //switch (doorCenterState) {
        //    case DoorCenterStates.Closed:
        //        break;
        //    case DoorCenterStates.Opening:
        //        break;
        //    case DoorCenterStates.Open:
        //        break;
        //    case DoorCenterStates.Closing:
        //        break;
        //}
        //Check state
        switch (state) {
            case States.Closed:
                Closed();
                break;
            case States.Opening:
                Opening();
                break;
            case States.Open:
                Open();
                break;
            case States.Closing:
                Closing();
                break;
        }
    }

    private void Closed() {
        //Check if not null
        if (trigger != null) {
            //Check if triggered
            if (trigger.HasObjects()) {
                //Change state
                doorCenterState = DoorCenterStates.Opening;
                //Play animation
                doorCenterRotator.PlayAnimation(DoorCenterRotator.AnimationStates.Open);
                //Change state
                state = States.Opening;
            }
        }
    }

    private void Opening() {
        //Check if not null
        if (trigger != null) {

        } else {

        }

        ////Check if not null
        //if (trigger != null) {
        //    //Check if not triggered
        //    if (!trigger.HasObjects()) {
        //        //Change state
        //        state = States.Closing;
        //    } else {
        //        //Opening now
        //        OpeningNow();
        //    }
        //} else {
        //    //Opening now
        //    OpeningNow();
        //}
    }

    private void OpeningNow() {
        ////Loop
        //for (int i = 0; i < doors.Length; i++) {
        //    //Move door
        //    doors[i].DoorTransform.position = Vector3.MoveTowards(doors[i].DoorTransform.position, doors[i].DoorOpen.position, speed);
        //}
        ////Check
        //if (doors.Length > 1) {
        //    //Check
        //    if (doors[0].DoorTransform.position == doors[0].DoorOpen.position && doors[1].DoorTransform.position == doors[1].DoorOpen.position) {
        //        //Change state
        //        state = States.Open;
        //    }
        //} else {
        //    //Check
        //    if (doors[0].DoorTransform.position == doors[0].DoorOpen.position) {
        //        //Change state
        //        state = States.Open;
        //    }
        //}
    }

    private void Open() {
        ////Check if not null
        //if (trigger != null) {
        //    //Check if not triggered
        //    if (!trigger.HasObjects()) {
        //        //Change state
        //        state = States.Closing;
        //    }
        //}
    }

    private void Closing() {
        ////Check if not null
        //if (trigger != null) {
        //    //Check if triggered
        //    if (trigger.HasObjects()) {
        //        //Change state
        //        state = States.Opening;
        //    } else {
        //        //Loop
        //        for (int i = 0; i < doors.Length; i++) {
        //            //Move door
        //            doors[i].DoorTransform.position = Vector3.MoveTowards(doors[i].DoorTransform.position, doors[i].DoorClosed.position, speed);
        //        }
        //        //Check
        //        if (doors.Length > 1) {
        //            //Check
        //            if (doors[0].DoorTransform.position == doors[0].DoorClosed.position && doors[1].DoorTransform.position == doors[1].DoorClosed.position) {
        //                //Change state
        //                state = States.Closed;
        //            }
        //        } else {
        //            //Check
        //            if (doors[0].DoorTransform.position == doors[0].DoorClosed.position) {
        //                //Change state
        //                state = States.Closed;
        //            }
        //        }
        //    }
        //}
    }

    #endregion

}