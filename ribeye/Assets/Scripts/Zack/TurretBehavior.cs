using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private TurretManager turretManager = null;
    public Transform turretMuzzlePoint = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private TurretObjectPool turretObjectPool = null;

    [Header("Specifications")]
    [SerializeField] private float detectionDistance = 25f;
    public float shootingForce = 7000f;

    [Header("Debugging")]
    [SerializeField] private TargetingStates targetingState = TargetingStates.DoingNothing;
    [SerializeField] private ShootingStates shootingState = ShootingStates.DoingNothing;

    //Declare privates
    private int hash = 0;

    #endregion

    #region Enumerators

    private enum TargetingStates {
        DoingNothing, Targeting
    }

    private enum ShootingStates {
        DoingNothing, Shooting
    }

    #endregion

    #region Constants

    private const string STATE = "State";

    #endregion

    #region Start and initialization

    private void Start() {
        //Set
        hash = Animator.StringToHash(STATE);
    }

    #endregion

    #region Update

    private void Update() {
        //Check state
        switch (targetingState) {
            case TargetingStates.DoingNothing:
                break;
            case TargetingStates.Targeting:
                Targeting();
                break;
        }
    }

    private void Targeting() {
        //Target
        turretManager.Target();
        //Raycast
        if (Physics.Raycast(turretMuzzlePoint.position, turretMuzzlePoint.forward, out RaycastHit raycastHit, detectionDistance)) {
            //Check
            if (turretManager.lookAtAndLockerScripts[0].target != null) {
                //Check if matches
                if (raycastHit.transform == turretManager.lookAtAndLockerScripts[0].target && shootingState == ShootingStates.DoingNothing) {
                    //Shoot
                    Shoot();
                }
            }
        }
    }

    private void Shoot() {
        //Set state
        shootingState = ShootingStates.Shooting;
        //Set state for animation
        animator.SetInteger(hash, 1);
        //Shoot bullet
        turretObjectPool.Shoot();
    }

    #endregion

    #region Animation events

    public void FinishedShooting() {
        //Change state
        shootingState = ShootingStates.DoingNothing;
        //Change animation
        animator.SetInteger(hash, 0);
    }

    #endregion

}