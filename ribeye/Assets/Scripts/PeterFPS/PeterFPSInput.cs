using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PeterFPSInput : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Keyboard/Mouse or Controller")]
    public inputTypes inputType = inputTypes.KeyboardAndMouse;

    [Header("Debugging")]
    [SerializeField] private Vector2 controllerLeftJoystickAxis = Vector2.zero;
    [SerializeField] private Vector2 controllerRightJoystickAxis = Vector2.zero;

    //Declare publics
    [NonSerialized] public float vertical = 0f;
    [NonSerialized] public float horizontal = 0f;
    [NonSerialized] public myAxis axisJoystickRight = new myAxis("ControllerRightJoystickVertical", "ControllerRightJoystickHorizontal");
    [NonSerialized] public bool RunPressed = false;

    //Declare privates
    private myAxis axisKeyboard = new myAxis("KeyboardVertical", "KeyboardHorizontal");
    private myAxis axisJoystickLeft = new myAxis("ControllerLeftJoystickVertical", "ControllerLeftJoystickHorizontal");

    // private myAxis axisKeyboard = new myAxis("Vertical", "Horizontal");
    //private myAxis axisJoystickLeft = new myAxis("ControllerLeftJoystickVertical", "ControllerLeftJoystickHorizontal");
    #endregion

    #region Classes

    public class myAxis {
        //Declare
        public axisFloatString vertical = null;
        public axisFloatString horizontal = null;
        //Constructor
        public myAxis(string stringVertical, string stringHorizontal) {
            //Create
            vertical = new axisFloatString(stringVertical);
            horizontal = new axisFloatString(stringHorizontal);
        }
        //Set input
        public void SetInput(bool setVertical = true, bool setHorizontal = true) {
            //Check
            if (setVertical) { vertical.axisFloat = Input.GetAxisRaw(vertical.axisString); }
            if (setHorizontal) { horizontal.axisFloat = Input.GetAxisRaw(horizontal.axisString); }
        }
    }
    public class axisFloatString {
        //Declare
        public float axisFloat = 0f;
        public string axisString = string.Empty;
        //Constructor
        public axisFloatString(string strAxisString) {
            //Set
            axisString = strAxisString;
        }
    }

    #endregion

    #region Constants

    //Declare private constants
    private const string strBUTTON_RUN = "Run";

    #endregion

    #region Enumerators

    public enum inputTypes {
        KeyboardAndMouse, Controller
    }

    #endregion

    #region Set input

    public void SetInput() {
        //Check input type
        if (inputType == inputTypes.KeyboardAndMouse) {
            //Set input
            axisKeyboard.SetInput();
            //Set vertical and horizontal
            setVerticalAndHorizontal(axisKeyboard);
            //Set
            RunPressed = Input.GetKey(KeyCode.LeftShift);
        } else {
            /*
            //Set input
            axisJoystickLeft.SetInput();
            axisJoystickRight.SetInput();
            //Set vertical and horizontal
            setVerticalAndHorizontal(axisJoystickLeft);
            //Set
            RunPressed = Input.GetButton(strBUTTON_RUN);
            //Set for debugging
            controllerLeftJoystickAxis = new Vector2(axisJoystickLeft.horizontal.axisFloat, axisJoystickLeft.vertical.axisFloat);
            controllerRightJoystickAxis = new Vector2(axisJoystickRight.horizontal.axisFloat, axisJoystickRight.vertical.axisFloat);
            */
        }
    }

    private void setVerticalAndHorizontal(myAxis udcAxisToBe) {
        //Set input axis
        vertical = udcAxisToBe.vertical.axisFloat;
        horizontal = udcAxisToBe.horizontal.axisFloat;
    }

    #endregion

}