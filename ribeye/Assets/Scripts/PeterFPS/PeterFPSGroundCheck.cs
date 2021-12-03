using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PeterFPSGroundCheck : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private GameObject[] notAccepts = default;
    [SerializeField] private string[] tagNotAccepts = default;

    //Declare privates
    [Header("Debugging")]
    [SerializeField] private List<GameObject> entries = new List<GameObject>();

    #endregion

    #region Trigger

    private void OnTriggerEnter(Collider other) {
        //Check if can be accepted
        if (CanBeAccepted(other.gameObject)) {
            //Check if doesn't already exist
            if (!entries.Contains(other.gameObject)) {
                //Add
                entries.Add(other.gameObject);
            }
        }
    }

    private bool CanBeAccepted(GameObject objGameObject) {
        //Loop
        for (int i = 0; i < notAccepts.Length; i++) {
            //Check
            if (notAccepts[i].GetInstanceID() == objGameObject.GetInstanceID()) {
                //Return
                return false;
            }
        }
        //Loop
        for (int i = 0; i < tagNotAccepts.Length; i++) {
            //Check
            if (objGameObject.CompareTag(tagNotAccepts[i])) {
                //Return
                return false;
            }
        }
        //Return
        return true;
    }

    private void OnTriggerExit(Collider other) {
        //Check if exists
        if (entries.Contains(other.gameObject)) {
            //Remove
            entries.Remove(other.gameObject);
        }
    }

    #endregion

    #region Public methods

    public bool OnGround() {
        //Return
        return entries.Count > 0;
    }

    #endregion

}