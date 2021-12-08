using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurretGameManager : MonoBehaviour {

    #region Declares

    //Declare publics
    [NonSerialized] public Transform DummyTransform = default;

    #endregion

    #region Constants

    //Declare private constants
    private const string strDUMMY_TRANSFORM = "DummyTransform";

    #endregion

    #region Start and initialization

    private void Start() {
        //Create dummy transform
        CreateDummyTransform();
    }

    private void CreateDummyTransform() {
        //Create dummy transform
        GameObject objDummy = new GameObject();
        //Create dummy transform
        DummyTransform = objDummy.transform;
        //Change name
        DummyTransform.name = strDUMMY_TRANSFORM;
        //Disable
        DummyTransform.gameObject.SetActive(false);
    }

    #endregion

}