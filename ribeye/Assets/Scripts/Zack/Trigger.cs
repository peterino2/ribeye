using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

    #region Declares

    //Declare serializables
    [SerializeField] private string[] acceptableTags = null;

    //Declare privates
    private List<GameObject> objectsWithinTrigger = new List<GameObject>();

    #endregion

    #region Triggers

    private void OnTriggerEnter(Collider other) {
        //Check tag
        if (CanBeAdded(other.gameObject)) {
            //Add
            objectsWithinTrigger.Add(other.gameObject);
        }
    }

    private bool CanBeAdded(GameObject gameObjectOther) {
        //Loop
        for (int i = 0; i < acceptableTags.Length; i++) {
            //Check tag
            if (gameObjectOther.gameObject.CompareTag(acceptableTags[0])) {
                //Return
                return true;
            }
        }
        //Return
        return false;
    }

    private void OnTriggerExit(Collider other) {
        //Check if in contents
        if (objectsWithinTrigger.Contains(other.gameObject)) {
            //Remove
            objectsWithinTrigger.Remove(other.gameObject);
        }
    }

    #endregion

    #region Public functions

    public bool HasObjects() {
        //Return
        return objectsWithinTrigger.Count > 0;
    }

    #endregion

}