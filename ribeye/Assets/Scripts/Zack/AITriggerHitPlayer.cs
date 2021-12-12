using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITriggerHitPlayer : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private string Tag = "CharacterControllerTag";

    [Header("Specifications")]
    [SerializeField] private GameObject Player = null;

    #endregion

    #region Triggers

    private void OnTriggerEnter(Collider other) {
        //Check
        if (other.gameObject.CompareTag(Tag)) {
            //Set
            Player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other) {
        //Check
        if (other.gameObject.CompareTag(Tag)) {
            //Reset
            Player = null;
        }
    }

    #endregion

    #region Public functions

    public bool PlayerWithinDamageZone() {
        //Return
        return Player != null;
    }

    #endregion

}