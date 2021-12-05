﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using Cursor = UnityEngine.Cursor;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PeterFPSCharacterController : MonoBehaviour {

    #region Declares
    //Declare serializables
    [Header("Setup")]
    [SerializeField] private PeterFPSInput inputScript = null;
    [SerializeField] private PeterFPSCameraLook cameraLook = null;
    [SerializeField] private PeterFPSGroundCheck groundCheck = null;
    [SerializeField] private WallRideOrGrabber wallGrappler = null;
    [SerializeField] private Transform Rotator = null;


    [Header("Specifications")]
    [SerializeField] private float RotatorSpeed = 500f;

    [HideInInspector] public Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _capsule;
    private RigidbodyConstraints originalConstraints;

    [Header("Polish")]
    [SerializeField] private AnimationCurve fallCurve;
    [SerializeField] private Transform cam;
    [SerializeField] private WallRunLean lean;
    [SerializeField] private Transform camParent;
    [SerializeField] private Transform cameraPos;
    [SerializeField] private Transform cameraPosP;
    //[SerializeField] private Transform cameraParent;
    
    [SerializeField] private TextMeshProUGUI dbgui;

    [Header("Rigidbody Stuff")]
    private Vector3 cachedvelocity;
    private Vector3 f;
    private bool pounding = false;

    [Header("Debugging")]
    public groundStates groundState = groundStates.InAir;

    private bool sliding = false;
    private bool slideLock = false;
    [SerializeField] float slideImpulse = 20f;
    [SerializeField] float slideForce = 10f;
 
    #endregion

    [SerializeField] private Transform cameraTransform;

    #region Enumerators

    public enum groundStates {
        Grounded, InAir
    }

    #endregion

    #region Update

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = true;
        originalConstraints = _rigidbody.constraints;
        Cursor.lockState = CursorLockMode.Locked;
        //_capsule = GetComponent<CapsuleCollider>();
    }

    private bool slideStart = false;
    private bool postPound = false;
    private void HandleSlideActivate()
    {
        bool lastSliding = sliding;
        sliding = Input.GetKey(KeyCode.LeftControl);
        if (postPound)
        {
            sliding = false;
            lastSliding = false;
        }
        
        if(!sliding)
            pounding = false;

        if (slideLock)
        {
            sliding = true;
            if (groundState == groundStates.Grounded)
            {
                slideLock = false;
                sliding = false;
            }
        }

        if (lastSliding != sliding)
        {
            slideStart = true;
        }

    }

    private bool dashReady = true;
    private bool dashing = false;
    [SerializeField] private float dashSpeed = 10f;

    IEnumerator doDash()
    {
        if (dashReady && !sliding)
        {
            dashReady = false;
            dashing = true;
            //Vector3 cachedVelocity = _rigidbody.velocity;
            f = travelVector;
            if (f.magnitude < 0.5f)
            {
                f = m_trueForward;
            }
            //_rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
            _rigidbody.useGravity = false;
            _rigidbody.velocity = (f.normalized * dashSpeed);
            yield return new WaitForSeconds(0.3f);
            
            EndDashing();

            //_rigidbody.velocity = cachedVelocity;
            yield return new WaitForSeconds(0.7f);
            dashReady = true;
        }
    }

    private void HandleWallGrabberUpdate()
    {
        if (wallGrappler._wall && groundState == groundStates.InAir)
        {
            StartCoroutine(doWallGrab());
            //if (Vector3.Dot(travelVector, wallGrappler.wallDir.normalized) > 0f)
            //{
            //    StartCoroutine(doWallGrab());
            //}
        }
    }
    private void Lean()
    {
        float x;
        x = Input.GetAxisRaw("Horizontal");
        camParent.localRotation = Quaternion.Slerp(camParent.localRotation, Quaternion.Euler(0, 0, x * -2), Time.deltaTime * 4);
    }

    private bool wallgrabready = true;
    private bool wallgrabbed = false;
    IEnumerator doWallGrab()
    {
        if (wallgrabready && !wallgrabbed && !wallrunning && !jumping)
        {
            //print(wallgrabbed);
            wallgrabready = false;
            wallgrabbed = true;
            cachedvelocity = _rigidbody.velocity;
            float dir = Vector3.Dot(-cameraTransform.right, wallGrappler.wallNormal);
            lean.isLeaning = false;
            lean.StartCoroutine(lean.Lean(dir));
            doubleJump = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.useGravity = false;
            yield return new WaitForSeconds(1f);
            _rigidbody.useGravity = true;
            wallgrabbed = false;
        }
        yield return null;
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(doDash());
        }
    }

    private bool spacePressed = false;
    private void Update() {
        //Set input
        spacePressed = Input.GetKeyDown(KeyCode.Space);
        Lean();
        inputScript.SetInput();
        Debug.DrawLine(transform.position, transform.position + m_trueForward);

        HandleSlideActivate();
        HandleJump();
        HandleDash();

        HandleWallGrabberUpdate();

        //Set state
        if (groundCheck.OnGround()) {
            //Change state
            groundState = groundStates.Grounded;
        } else {
            //Change state
            groundState = groundStates.InAir;
        }
        //Check
        switch (inputScript.inputType) {
            case PeterFPSInput.inputTypes.KeyboardAndMouse:
                UseMouseInput();
                break;
            case PeterFPSInput.inputTypes.Controller:
                UseControllerInput();
                break;
        }

        if (!wallGrappler._wall)
        {
            wallgrabbed = false;
        }


        int layer = 1 << 3;
        layer = ~layer;
        underObject = Physics.Raycast(transform.position + new Vector3(0, -0.5f, 0), Vector3.up, 1, layer);
        Input.GetKeyDown(KeyCode.Space);
        if (wallrunning)
        {
            if (!wallGrappler._wall || spacePressed)
            {
                if (spacePressed)
                {
                    _rigidbody.AddForce(wallGrappler.wallNormal * 10, ForceMode.Impulse);
                    wallgrabready = true;
                }
                EndWallRun();
            }
        }
        
        if (dashing)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                print("jumping during dash");
                EndDashing();
                
                Vector3 vel = _rigidbody.velocity;
                _rigidbody.velocity = new Vector3(vel.x, jumpImpulse, vel.z);
            }
        }

        if (!Input.GetKey(KeyCode.LeftControl))
        {
            postPound = false;
        }

        HandleDebugUi();
    }

    private void HandleDebugUi()
    {
        string dbgString = string.Format("jumping: {0}, sliding{1}, dashing{2}, doublejump{3}\n", jumping, sliding, dashing, doubleJump);
        dbgString += string.Format("wallgrab: {0}, wallgrab ready{1} wallrunning{2}", wallgrabbed, wallgrabready, wallrunning);
        dbgui.text = dbgString;
    }

    private void EndDashing()
    {
        dashing = false;
        _rigidbody.useGravity = true;

        _rigidbody.constraints = originalConstraints;
        //_rigidbody.velocity = f * 10;
    }

    private bool wallrunning = false;
    private Vector3 Wallrunning_vect;
    [SerializeField] private float wallrunningInitialSpeed = 8;
    IEnumerator DoWallRun()
    {
        if (!wallrunning)
        {
            print("wallrunning started!");
            wallgrabbed = false;
            Vector3 base_vect = Vector3.ProjectOnPlane(transform.forward, wallGrappler.wallNormal);
            Vector3 start_normal = wallGrappler.wallNormal;
            //base_vect.y += 0.10f;
            base_vect = base_vect.normalized;
            print(base_vect);
            Wallrunning_vect = base_vect * wallrunningInitialSpeed;
            wallrunning = true;
            yield return new WaitForSeconds(1.5f);
            wallrunning = false;
            
            // jump off if we're still on the wall.
            // if (wallGrappler._wall)
            // {
                 //_rigidbody.AddForce(wallGrappler.wallNormal * (jumpImpulse * 100));
                 // wallrunning = false;
            // }
        }
    }
    void EndWallRun()
    {
        wallrunning = false;
        _rigidbody.AddForce(Vector3.up * 1.5f, ForceMode.Impulse);

        // lean.StopAllCoroutines();
        lean.isLeaning = false;
        //lean.StartCoroutine(lean.ResetLean());

        //_rigidbody.velocity += wallGrappler.wallNormal *  (jumpImpulse * 10);
        doubleJump = false;
    }

    public float mousex = 0f;
    public float mousey = 0f;
    private void UseMouseInput()
    {
        mousex += Input.GetAxis("Mouse X");
        mousey += Input.GetAxis("Mouse Y");
        mousey = Mathf.Clamp(mousey, -90, 90);
    }

    private void LateUpdate()
    {
        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(-mousey, mousex, 0), 0.5f);
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraPos.position, 0.5f); // 16ms * 60  = 1 s
    }

    [SerializeField] private float speed = 500;
    [SerializeField] private float airForce = 20;
    [SerializeField] private float jumpImpulse = 60;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 m_trueForward;

    private void GetGroundDirections(out Vector3 trueForward, out Vector3 trueRight, out Vector3 trueDown)
    {
        Physics.Raycast(transform.position, -transform.up, out RaycastHit hitInfo, 5f, groundLayer);

        if (groundState == groundStates.Grounded)
        {
            trueForward = Vector3.ProjectOnPlane(transform.forward, hitInfo.normal).normalized;
            trueRight = Vector3.ProjectOnPlane(transform.right, hitInfo.normal).normalized;
            trueDown = Vector3.ProjectOnPlane(-transform.up, hitInfo.normal).normalized;
        }
        else
        {
            trueForward = transform.forward;
            trueRight = transform.right;
            trueDown = -transform.up;
        }

        m_trueForward = trueForward;
    }

    private void HandleSlidingFixedUpdate(Vector3 trueForward, Vector3 trueRight, Vector3 trueDown)
    {
            if (slideStart)
            {
                slideStart = false;
                if (doubleJump)
                {
                    _rigidbody.AddForce(trueDown * 35f, ForceMode.Impulse);
                    pounding = true;
                }

                if (groundState == groundStates.Grounded)
                {
                    _rigidbody.AddForce(trueForward * 10f, ForceMode.Impulse);
                }
            }

            if (groundState == groundStates.InAir)
            {
                slideLock = true;
            }
            else
            {
                _rigidbody.AddForce(GetGravity()*10f);

                // _rigidbody.AddForce(travelVector * airForce);

                _rigidbody.velocity = (1 - 0.95f * Time.deltaTime) * _rigidbody.velocity;

                //if (horizontalVelocityVector.magnitude < maxAccelSpeed * 0.4f)
                //{
                //    _rigidbody.velocity = maxAccelSpeed * 0.4f * travelVector;
                //}

                if ((maxAccelSpeed * 0.4f * travelVector).magnitude > _rigidbody.velocity.magnitude)
                {
                    _rigidbody.velocity = maxAccelSpeed * 0.4f * travelVector;
                }

                // if (horizontalVelocityVector.magnitude > maxSlideSpeed)
                // {
                //     _rigidbody.velocity = (maxSlideSpeed * horizontalVelocityVector.normalized) + Vector3.up * _rigidbody.velocity.y;
                // }
            }
    }

    private bool jumping = false;
    private bool underObject = false;
    [SerializeField] private float maxAccelSpeed = 20f;
    [SerializeField] private float maxSlideSpeed = 20f;

    Vector3 travelVector = Vector3.zero;
    Vector3 horizontalVelocityVector = Vector3.zero;

    private void FixedUpdate()
    {
        _rigidbody.rotation = Quaternion.Euler(0, mousex, 0);
        

        GetGroundDirections(out Vector3 trueForward, out Vector3 trueRight, out Vector3 trueDown);
        travelVector = ((inputScript.vertical * trueForward + inputScript.horizontal * trueRight).normalized);
        horizontalVelocityVector = _rigidbody.velocity;
        horizontalVelocityVector.y = 0;

        if (groundState == groundStates.Grounded)
        {
            doubleJump = false;
            wallgrabready = true;
        }
        _rigidbody.useGravity = true;


        if (groundState == groundStates.InAir && dashing)
        {
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
        }
        if (!sliding && !jumping && !dashing && !underObject)
        {
            _capsule.center = new Vector3(0, -0.25f, 0);
            _capsule.height = 1.5f;
            cameraPosP.localPosition = Vector3.Lerp(cameraPosP.localPosition, new Vector3(0, 0.5f, 0), Time.deltaTime * 8);
            cam.localRotation = Quaternion.Slerp(cam.localRotation, Quaternion.identity, Time.deltaTime * 10);
            if (groundState == groundStates.Grounded)
            {
                if (travelVector.magnitude > 0)
                    _rigidbody.velocity = speed * (travelVector);
                else
                    _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, Time.deltaTime * 10);
                _rigidbody.useGravity = false;
            }
        }
        else if(sliding)
        {
            if(groundState == groundStates.Grounded && !pounding)
            {
                _capsule.center = new Vector3(0, -0.5f, 0);
                _capsule.height = 0.75f;
                cameraPosP.localPosition = Vector3.Lerp(cameraPosP.localPosition, new Vector3(0, -0.45f, 0), Time.deltaTime * 8);
                cam.localRotation = Quaternion.Slerp(cam.localRotation, Quaternion.Euler(-10, 0, 0), Time.deltaTime * 2);
            }
            HandleSlidingFixedUpdate(trueForward, trueRight, trueDown);
        }

        if (pounding)
        {
            if (groundState == groundStates.Grounded)
            {
                _rigidbody.velocity = Vector3.zero;
                pounding = false;
                postPound = true;
            }
        }

        if (wallgrabbed)
        {
            _rigidbody.useGravity = false;
            
            if (wallgrabbed)
            {
                if (Vector3.Dot(travelVector, wallGrappler.wallDir.normalized)  > 0.5f)
                {
                    StartCoroutine(DoWallRun());
                }
            }
            
            if (spacePressed)
            {
                StartCoroutine(doWallGrabJump());
            }
            //_rigidbody.velocity = -Vector3.up * 0.2f;
        }

        if (groundState == groundStates.InAir && !dashing && !wallgrabbed)
        {
            //_capsule.center = new Vector3(0, 0.09f, 0);
            //cameraPos.localPosition = Vector3.Lerp(cameraPos.localPosition, new Vector3(0, -0.5f, 0), Time.deltaTime * 8);
            _rigidbody.AddForce(travelVector * airForce);
            var innerMaxSpeed = sliding ? maxSlideSpeed : maxAccelSpeed;
            if (horizontalVelocityVector.magnitude > innerMaxSpeed)
            {
                _rigidbody.velocity = (innerMaxSpeed * horizontalVelocityVector.normalized) + Vector3.up * _rigidbody.velocity.y;
            }
        }

        if (wallrunning)
        {
            _rigidbody.velocity = Wallrunning_vect;
        }
    }

    private bool wallJumpDebounce = true;
    IEnumerator doWallGrabJump()
    {
        if (wallJumpDebounce)
        {
            wallgrabbed = false;
            _rigidbody.AddForce(wallGrappler.wallNormal * 10, ForceMode.Impulse);
            _rigidbody.AddForce(transform.up * 10, ForceMode.Impulse);
            yield return new WaitForSeconds(0.1f);
            wallJumpDebounce = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f) && !(groundState == groundStates.Grounded))
        {
            if (collision.relativeVelocity.magnitude > 0.3f)
            {
                StartCoroutine(LerpCamDown());
            }
        }
    }
    IEnumerator LerpCamDown()
    {
        float pos = 0;
        while (pos < 1)
        {
            pos += Time.deltaTime * 2.5f;
            cameraPos.localPosition = new Vector3(cameraPos.localPosition.x, fallCurve.Evaluate(pos), cameraPos.localPosition.z);
            yield return null;
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(doJump());
        }
    }

    private bool doubleJump = false;

    IEnumerator doJump()
    {
        if (!jumping)
        {
            if (groundState == groundStates.Grounded)
            {
                Vector3 vel = _rigidbody.velocity;
                _rigidbody.velocity = new Vector3(vel.x, jumpImpulse, vel.z);
                jumping = true;
                yield return new WaitForSeconds(0.1f);
                jumping = false;
            }
            else
            {
                if (groundState == groundStates.InAir && !doubleJump)
                {
                    Vector3 vel = _rigidbody.velocity;
                    _rigidbody.velocity = new Vector3(vel.x, jumpImpulse, vel.z);
                    doubleJump = true;
                    jumping = true;
                    yield return new WaitForSeconds(0.1f);
                    jumping = false;
                }
            }
        }
    }

    private Vector3 GetGravity()
    {
        Vector3 rv = Vector3.zero;

        if (groundState == groundStates.InAir)
        {
            return Physics.gravity;
        }
        return rv;
    }


    private void HandleGravity()
    {

    }

    private void UseControllerInput() {

    }

    #endregion

}
