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
    [SerializeField] private Transform target = null;
    [SerializeField] private Transform emptyTarget = null;
    [SerializeField] private Transform xAxisEmptyRotator = null;
    [SerializeField] private Transform xAxisEmptyRotatorReset = null;
    [SerializeField] private Transform xAxisEmptyRotatorLeftScan = null;
    [SerializeField] private Transform xAxisEmptyRotatorRightScan = null;

    [Header("Specifications")]
    [SerializeField] private float detectionDistance = 25f;
    public float shootingForce = 7000f;

    [Header("Debugging")]
    [SerializeField] private TargetingStates targetingState = TargetingStates.DoingNothing;
    [SerializeField] private ShootingStates shootingState = ShootingStates.DoingNothing;
    [SerializeField] private ScanningStates scanningState = ScanningStates.Right;

    //Declare privates
    private int hash = 0;

    #endregion

    #region Enumerators

    private enum TargetingStates {
        DoingNothing, Scanning, Targeting, LostTarget
    }

    private enum ShootingStates {
        DoingNothing, Shooting
    }

    private enum ScanningStates {
        Right, Left
    }

    #endregion

    #region Constants

    private const string STATE = "State";

    #endregion

    #region Start and initialization

    private void Start() {
        //Set
        hash = Animator.StringToHash(STATE);
        //Check state
        switch (targetingState) {
            case TargetingStates.DoingNothing:
                break;
            case TargetingStates.Scanning:
                //Change rotation
                xAxisEmptyRotator.rotation = xAxisEmptyRotatorRightScan.rotation;
                break;
            case TargetingStates.Targeting:
                //Check target
                if (target != null) {
                    //Set target
                    SetTarget(target);
                }
                break;
            case TargetingStates.LostTarget:
                break;
        }
    }

    #endregion

    #region Update

    private void Update() {
        //Check state
        switch (targetingState) {
            case TargetingStates.DoingNothing:
                break;
            case TargetingStates.Scanning:
                Scanning();
                break;
            case TargetingStates.Targeting:
                Targeting();
                break;
            case TargetingStates.LostTarget:
                LostTarget();
                break;
        }
    }

    private void SetTarget(Transform targetToBe) {
        //Loop
        for (int i = 0; i < turretManager.lookAtAndLockerScripts.Length; i++) {
            //Set target
            turretManager.lookAtAndLockerScripts[i].target = targetToBe;
        }
    }

    private void Scanning() {
        //Target
        turretManager.Target();
        //Check state
        switch (scanningState) {
            case ScanningStates.Right:
                //Check if matches rotation
                if (xAxisEmptyRotatorRightScan.rotation == turretManager.lookAtAndLockerScripts[1].transform.rotation) {
                    //Change rotation
                    xAxisEmptyRotator.rotation = xAxisEmptyRotatorLeftScan.rotation;
                    //Change state
                    scanningState = ScanningStates.Left;
                }
                break;
            case ScanningStates.Left:
                //Check if matches rotation
                if (xAxisEmptyRotatorLeftScan.rotation == turretManager.lookAtAndLockerScripts[1].transform.rotation) {
                    //Change rotation
                    xAxisEmptyRotator.rotation = xAxisEmptyRotatorRightScan.rotation;
                    //Change state
                    scanningState = ScanningStates.Right;
                }
                break;
        }
        //Check distance
        if (Vector3.Distance(target.position, transform.position) <= detectionDistance) {
            //Change state
            targetingState = TargetingStates.Targeting;
            //Set target
            SetTarget(target);
        }
    }

    private void Targeting() {
        //Target
        turretManager.Target();
        //Raycast
        if (Physics.Raycast(turretMuzzlePoint.position, turretMuzzlePoint.forward, out RaycastHit raycastHit, detectionDistance * 2f)) {
            //Check
            if (turretManager.lookAtAndLockerScripts[0].target != null) {
                //Check if matches
                if (raycastHit.transform == turretManager.lookAtAndLockerScripts[0].target && shootingState == ShootingStates.DoingNothing) {
                    //Shoot
                    Shoot();
                }
            }
        }
        //Check distance
        if (Vector3.Distance(target.position, transform.position) > detectionDistance) {
            //Prepare lost target
            PrepareLostTarget();
        }
    }

    private void PrepareLostTarget() {
        //Change target
        SetTarget(emptyTarget);
        //Set
        xAxisEmptyRotator.rotation = xAxisEmptyRotatorReset.rotation;
        //Change state
        targetingState = TargetingStates.LostTarget;
    }

    private void Shoot() {
        //Set state
        shootingState = ShootingStates.Shooting;
        //Set state for animation
        animator.SetInteger(hash, 1);
        //Shoot bullet
        turretObjectPool.Shoot();
    }

    private void LostTarget() {
        //Target
        turretManager.Target();
        //Check
        if (turretManager.lookAtAndLockerScripts[0].transform.localEulerAngles == Vector3.zero && turretManager.lookAtAndLockerScripts[1].transform.localEulerAngles == Vector3.zero) {
            //Change rotation
            xAxisEmptyRotator.rotation = xAxisEmptyRotatorRightScan.rotation;
            //Set
            scanningState = ScanningStates.Right;
            //Change state
            targetingState = TargetingStates.Scanning;
        }
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