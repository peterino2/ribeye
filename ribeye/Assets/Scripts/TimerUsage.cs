using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimerUsage {

    //Declare
    public float Set = 1f;
    [NonSerialized] public float Subtract = 0f;

    //Constructor
    public TimerUsage(float fltSet = 1f, bool blnInitializeTimer = true) {
        //Set
        Set = fltSet;
        //Check
        if (blnInitializeTimer) {
            //Set
            Subtract = fltSet;
        }
    }

    public void SetTimer() {
        //Set
        Subtract = Set;
    }

    public bool UpdateTimer(bool blnResetSet = true, bool blnCountWithSlowMotion = true) {
        //Check timer
        if (Subtract > 0f) {
            //Check if need to count slow motion
            if (blnCountWithSlowMotion) {
                //Subtract time
                Subtract -= Time.deltaTime;
            } else {
                //Subtract time
                Subtract -= Time.deltaTime / Time.timeScale;
            }
        }
        //Check time
        if (Subtract <= 0f) {
            //Check if need to reset timer
            if (blnResetSet) {
                //Reset
                Subtract = Set;
            }
            //Return
            return true;
        } else {
            //Return
            return false;
        }
    }

    public bool TimeUp() {
        //Return
        return Subtract <= 0f;
    }

}