using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RagDoll : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private Animator animator = null;
    [SerializeField] private Rigidbody[] rigidbodies = null;
    [SerializeField] private NavMeshAgent navMeshAgent = null;
    [SerializeField] private AIController AIControllerScript = null;
    [SerializeField] private Collider collider = null;

    #endregion

    #region Start and initialize

    private void Start() {
        //Loop
        for (int i = 0; i < rigidbodies.Length; i++) {
            //Turn on kinematic
            rigidbodies[i].isKinematic = true;
        }
    }

    #endregion

    #region Public functions

    [SerializeField] private bool Testing = false;

    private void Update() {
        if (Testing) {
            EnableRagDoll();
            Testing = false;
        }
    }

    public void EnableRagDoll() {
        //Disable
        animator.enabled = false;
        //Disable
        navMeshAgent.enabled = false;
        //Disable
        AIControllerScript.enabled = false;
        //Disable
        collider.enabled = false;
        //Loop
        for (int i = 0; i < rigidbodies.Length; i++) {
            //Turn off kinematic
            rigidbodies[i].isKinematic = false;
        }
    }

    #endregion

}