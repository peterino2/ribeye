using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlidingDoors : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private Door[] doors = null;
    [SerializeField] private Trigger trigger = null;

    [Header("Specifications")]
    [SerializeField] private float speed = 25f;

    [Header("Debugging")]
    [SerializeField] private States state = States.Closed;

    #endregion

    #region Enumerators

    private enum States {
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
        //Check if triggered
        if (trigger.HasObjects()) {
            //Change state
            state = States.Opening;
        }
    }

    private void Opening() {
        //Check if not triggered
        if (!trigger.HasObjects()) {
            //Change state
            state = States.Closing;
        } else {
            //Loop
            for (int i = 0; i < doors.Length; i++) {
                //Move door
                doors[i].DoorTransform.position = Vector3.MoveTowards(doors[i].DoorTransform.position, doors[i].DoorOpen.position, speed);
            }
            //Check
            if (doors[0].DoorTransform.position == doors[0].DoorOpen.position && doors[1].DoorTransform.position == doors[1].DoorOpen.position) {
                //Change state
                state = States.Open;
            }
        }
    }

    private void Open() {
        //Check if not triggered
        if (!trigger.HasObjects()) {
            //Change state
            state = States.Closing;
        }
    }

    private void Closing() {
        //Check if triggered
        if (trigger.HasObjects()) {
            //Change state
            state = States.Opening;
        } else {
            //Loop
            for (int i = 0; i < doors.Length; i++) {
                //Move door
                doors[i].DoorTransform.position = Vector3.MoveTowards(doors[i].DoorTransform.position, doors[i].DoorClosed.position, speed);
            }
            //Check
            if (doors[0].DoorTransform.position == doors[0].DoorClosed.position && doors[1].DoorTransform.position == doors[1].DoorClosed.position) {
                //Change state
                state = States.Closed;
            }
        }
    }

    #endregion

}