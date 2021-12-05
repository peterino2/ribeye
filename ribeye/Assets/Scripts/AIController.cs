using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private Transform destination = null;

    [Header("Specifcations")]
    [SerializeField] private States startingState = States.Stand;

    //Declare privates
    private NavMeshAgent navMeshAgent = null;
    private Vector3 previousTargetLocation = Vector3.zero;
    private States state = States.Stand;

    #endregion

    #region Enumerators

    private enum States {
        Stand, GoToDestination, Patrol
    }

    #endregion

    #region Start and initialization

    private void Start() {
        //Set
        navMeshAgent = GetComponent<NavMeshAgent>();
        //Set starting state
        state = startingState;
        //Set destination
        if (destination != null && state == States.GoToDestination) {
            //Set destination now
            SetDestinationNow();
        }
    }

    private void SetDestinationNow() {
        //Set
        previousTargetLocation = destination.position;
        //Set
        navMeshAgent.SetDestination(destination.position);
    }

    #endregion

    private void Update() {
        //Check state
        switch (state) {
            case States.Stand:
                break;
            case States.GoToDestination:
                GoToDestinationState();
                break;
            case States.Patrol:
                break;
        }
    }

    private void GoToDestinationState() {
        //Check
        if (destination.position != previousTargetLocation) {
            //Set destination now
            SetDestinationNow();
        }
    }

}