using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurretManager : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    public LookAtAndLocker[] lookAtAndLockerScripts = default;

    #endregion

    #region Update

    public void Target() {
        //Loop
        for (int intLoop = 0; intLoop < lookAtAndLockerScripts.Length; intLoop++) {
            //Look at
            lookAtAndLockerScripts[intLoop].RotateTowardsTarget();
        }
    }

    #endregion

}