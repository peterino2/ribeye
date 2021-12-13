using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private TurretManager turretManager = null;
    [SerializeField] private Transform turretMuzzleTargeting = null;
    [SerializeField] private Transform[] turretMuzzlePoints = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private TurretObjectPool turretObjectPool = null;
    [SerializeField] private Transform target = null;
    [SerializeField] private string Tag = "CharacterControllerTag";
    [SerializeField] private Transform emptyTarget = null;
    [SerializeField] private Transform xAxisEmptyRotator = null;
    [SerializeField] private Transform xAxisEmptyRotatorReset = null;
    [SerializeField] private Transform xAxisEmptyRotatorLeftScan = null;
    [SerializeField] private Transform xAxisEmptyRotatorRightScan = null;

    [Header("Specifications")]
    [SerializeField] private LineRenderer lineRenderer = null;
    [SerializeField] private float detectionDistance = 25f;
    public float shootingForce = 1f;
    [SerializeField] private float speedWhenScanning = 5f;
    [SerializeField] private float animatorSpeedForShooting = 1f;

    [Header("Debugging")]
    [SerializeField] private TargetingStates targetingState = TargetingStates.DoingNothing;
    [SerializeField] private ShootingStates shootingState = ShootingStates.DoingNothing;
    [SerializeField] private ScanningStates scanningState = ScanningStates.Right;
    [SerializeField] private LayerMask TheLayerMask = new LayerMask();

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
        //Check if no target
        if (!target) {
            //Set target
            target = GameObject.Find("CharacterController").transform;
        }
        //Set
        hash = Animator.StringToHash(STATE);
        //Set animator speed
        animator.speed = animatorSpeedForShooting;
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
        //Check
        if (turretObjectPool != null && lineRenderer == null) {
            //Run lifetimes
            turretObjectPool.CheckLifetimes(TheLayerMask);
        }
        //Check
        if (lineRenderer != null) {
            //Set state
            shootingState = ShootingStates.DoingNothing;
            //Reset
            lineRenderer.enabled = false;
        }
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
        turretManager.Target(speedWhenScanning, true);
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
        turretManager.Target(0f);
        //Check
        if (Physics.Raycast(turretMuzzleTargeting.position, turretMuzzleTargeting.forward, out RaycastHit raycastHit, detectionDistance * 2f, TheLayerMask, QueryTriggerInteraction.Ignore)) {
            //Check if player
            if (raycastHit.transform.gameObject.CompareTag(Tag)) {
                //Check
                if (lineRenderer == null) {
                    //Check
                    if (shootingState == ShootingStates.DoingNothing) {
                        //Shoot
                        Shoot();
                    }
                } else {
                    //Laser
                    Laser(raycastHit.point);
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
        //Loop
        for (int i = 0; i < turretMuzzlePoints.Length; i++) {
            //Shoot bullet
            turretObjectPool.Shoot(turretMuzzlePoints[i]);
            GameManager._soundManager.PlaySound(22, transform.position, volume:0.05f);
        }
    }

    private void Laser(Vector3 position) {
        //Set
        lineRenderer.enabled = true;
        //Set state
        shootingState = ShootingStates.Shooting;
        //Set line
        lineRenderer.SetPosition(0, turretMuzzleTargeting.position);
        lineRenderer.SetPosition(1, position);

        //HIT PLAYER
        Debug.Log("HIT");
    }

    private void LostTarget()
    {
        //Target
        turretManager.Target(0f);
        //Check
        if ((turretManager.lookAtAndLockerScripts[0].transform.rotation == xAxisEmptyRotatorReset.rotation)
        && (turretManager.lookAtAndLockerScripts[1].transform.rotation == xAxisEmptyRotatorReset.rotation))
        {
            //Change rotation
            xAxisEmptyRotator.rotation = xAxisEmptyRotatorRightScan.rotation;
            //Set
            scanningState = ScanningStates.Right;
            //Change state
            targetingState = TargetingStates.Scanning;
        }
        
        //Target
        //turretManager.Target(0f);
        // //Check
        // if (Vector3Approximately(turretManager.lookAtAndLockerScripts[0].transform.localEulerAngles, Vector3.zero)
        // && Vector3Approximately(turretManager.lookAtAndLockerScripts[1].transform.localEulerAngles, Vector3.zero))
        // {
        //     //Change rotation
        //     xAxisEmptyRotator.rotation = xAxisEmptyRotatorRightScan.rotation;
        //     //Set
        //     scanningState = ScanningStates.Right;
        //     //Change state
        //     targetingState = TargetingStates.Scanning;
        // }
    }

    private bool Vector3Approximately(Vector3 v0, Vector3 v1)
    {
        return Mathf.Approximately(v0.x, v1.x) && Mathf.Approximately(v0.y, v1.y) && Mathf.Approximately(v0.z, v1.z);
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