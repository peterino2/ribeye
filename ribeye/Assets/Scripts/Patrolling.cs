using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class Patrolling : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private AIController TheAIController = null;
    [SerializeField] private WayPoints wayPoints = null;

    [Header("Specifications")]
    [SerializeField] private bool startAtFirstPoint = false;
    [SerializeField] private float withinDistance = 0.1f;

    [Header("Debugging")]
    [SerializeField] private DestinationInfo destinationInfo = new DestinationInfo();

    #endregion

    #region Classes

    [Serializable]
    private class DestinationInfo {
        //Declare
        public WayPoint CurrentWayPoint = null;
        public WayPoint NextWayPoint = null;
        //Set destination
        public void SetDestination(WayPoint currentWayPoint, WayPoint nextWayPoint, NavMeshAgent navMeshAgent) {
            //Set
            CurrentWayPoint = currentWayPoint;
            NextWayPoint = nextWayPoint;
            //Set destination
            Debug.Log(navMeshAgent.SetDestination(NextWayPoint.transform.position));
        }
    }

    #endregion

    #region Start and initialization

    public void Initialize() {
        //Check
        if (startAtFirstPoint) {
            //Move
            transform.position = wayPoints.wayPoints[0].transform.position;
            //Set destination info
            destinationInfo.SetDestination(wayPoints.wayPoints[0], wayPoints.wayPoints[0].nextTransforms[UnityEngine.Random.Range(0, wayPoints.wayPoints[0].nextTransforms.Length)], TheAIController.navMeshAgent);
        }
    }

    #endregion

    #region Public methods

    public void Patrol() {
        //Check within distance
        if (Vector3.Distance(transform.position, destinationInfo.NextWayPoint.transform.position) <= withinDistance) {
            //Set next destination
            destinationInfo.SetDestination(destinationInfo.NextWayPoint, destinationInfo.NextWayPoint.nextTransforms[UnityEngine.Random.Range(0, destinationInfo.NextWayPoint.nextTransforms.Length)], TheAIController.navMeshAgent);
        }
    }

    #endregion

}