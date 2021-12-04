using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PeterFPSCharacterController : MonoBehaviour {

    #region Declares
    //Declare serializables
    [Header("Setup")]
    [SerializeField] private PeterFPSInput inputScript = null;
    [SerializeField] private PeterFPSCameraLook cameraLook = null;
    [SerializeField] private PeterFPSGroundCheck groundCheck = null;
    [SerializeField] private Transform Rotator = null;

    [Header("Specifications")]
    [SerializeField] private float RotatorSpeed = 500f;

    private Rigidbody _rigidbody;
    private CapsuleCollider _capsule;

    [Header("Debugging")]
    [SerializeField] private groundStates groundState = groundStates.InAir;
    
    private bool sliding = false;
    private bool slideLock = false;
    [SerializeField] float slideImpulse = 20f;
    [SerializeField] float slideForce = 10f;

    #endregion

    [SerializeField] private Transform cameraTransform;

    #region Enumerators

    private enum groundStates {
        Grounded, InAir
    }

    #endregion

    #region Update
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = true;
        Cursor.lockState = CursorLockMode.Locked;
        _capsule = GetComponent<CapsuleCollider>();
    }

    private bool slideStart = false;
    private void HandleSlideActivate()
    {
        bool lastSliding = sliding;
        sliding = Input.GetKey(KeyCode.LeftControl);
        
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

    private void Update() {
        //Set input
        
        inputScript.SetInput();

        HandleSlideActivate();

        HandleJump();
        
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
    }


    public float mousex = 0f;
    public float mousey = 0f;
    private void UseMouseInput()
    {
        mousex += Input.GetAxis("Mouse X");
        mousey += Input.GetAxis("Mouse Y");
        mousey = Mathf.Clamp(mousey, -90, 90);
        
        _rigidbody.rotation = Quaternion.Euler(0, mousex, 0);
    }

    private void LateUpdate()
    {
        // cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(-mousey, mousex, 0), Time.deltaTime / 0.016f);
        // cameraTransform.position = Vector3.Lerp(cameraTransform.position, transform.position, Time.deltaTime / 0.016f); // 16ms * 60  = 1 s
        
        Transform newTransform = 
        cameraTransform.position = transform.position; // 16ms * 60  = 1 s
        cameraTransform.rotation = Quaternion.Euler(-mousey, mousex, 0);

    }

    [SerializeField] private float speed = 500;
    [SerializeField] private float airForce = 20;
    [SerializeField] private float jumpImpulse = 60;
    [SerializeField] private LayerMask groundLayer;

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
    }

    private void HandleSlidingFixedUpdate(Vector3 trueForward, Vector3 trueRight, Vector3 trueDown)
    {
            if (slideStart)
            {
                slideStart = false;
                 _rigidbody.AddForce(trueDown * 20f, ForceMode.Impulse);
            }

            if (groundState == groundStates.InAir)
            {
                slideLock = true;
            }
            else
            {
                _rigidbody.AddForce(GetGravity()*10f);
                if (_rigidbody.velocity.magnitude < speed * 0.4)
                {
                    _rigidbody.velocity = (0.4f * speed) * trueForward;
                }
                else
                {
                    _rigidbody.AddForce(travelVector * airForce);
                    if (horizontalVelocityVector.magnitude > maxAccelSpeed)
                    {
                        _rigidbody.velocity = (maxAccelSpeed * horizontalVelocityVector.normalized) + Vector3.up * _rigidbody.velocity.y;
                    }
                }
            }
    }

    private bool jumping = false;
    [SerializeField] private float maxAccelSpeed = 20f;
    [SerializeField] private float maxSlideSpeed = 20f;
    
    Vector3 travelVector = Vector3.zero;
    Vector3 horizontalVelocityVector = Vector3.zero;

    private void FixedUpdate()
    {
        GetGroundDirections(out Vector3 trueForward, out Vector3 trueRight, out Vector3 trueDown);
        travelVector = ((inputScript.vertical * trueForward + inputScript.horizontal * trueRight).normalized);
        horizontalVelocityVector = _rigidbody.velocity;
        horizontalVelocityVector.y = 0;
        
        _rigidbody.useGravity = true;
        if (!sliding && !jumping)
        {
            _capsule.height = 2;
            if (groundState == groundStates.Grounded)
            {
                _rigidbody.velocity = speed * (travelVector);
                _rigidbody.useGravity = false;
            }
        }
        else if(sliding)
        {
            _capsule.height = 0.6f;
            HandleSlidingFixedUpdate(trueForward, trueRight, trueDown);
        }
        
        if (groundState == groundStates.InAir)
        {
            _capsule.height = 2;
            _rigidbody.AddForce(travelVector * airForce);
            var innerMaxSpeed = sliding ? maxSlideSpeed : maxAccelSpeed;
            if (horizontalVelocityVector.magnitude > innerMaxSpeed)
            {
                _rigidbody.velocity = (innerMaxSpeed * horizontalVelocityVector.normalized) + Vector3.up * _rigidbody.velocity.y;
            }
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && groundState == groundStates.Grounded)
        {
            StartCoroutine(doJump());
        }
    }

    IEnumerator doJump()
    {
        if (!jumping)
        {
            Vector3 vel = _rigidbody.velocity;
            _rigidbody.velocity = new Vector3(vel.x, jumpImpulse, vel.z);
            jumping = true;
            yield return new WaitForSeconds(0.1f);
            jumping = false;
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