using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.UI;
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

    [Header("Debugging")]
    [SerializeField] private groundStates groundState = groundStates.InAir;

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


    private float mousex = 0f;
    private float mousey = 0f;
    private void UseMouseInput()
    {
        mousex += Input.GetAxis("Mouse X");
        mousey += Input.GetAxis("Mouse Y");
        mousey = Mathf.Clamp(mousey, -90, 90);

        // transform.localRotation *= Quaternion.Euler(0, mousex * sensitivity, 0);
        // transform.localRotation *= Quaternion.Euler(mousey * sensitivity, 0, 0);
        
        _rigidbody.rotation = Quaternion.Euler(0, mousex, 0);
    }

    private void LateUpdate()
    {
        cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(-mousey, mousex, 0), Time.deltaTime/0.016f);
        if (sliding)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, transform.position - new Vector3(0.0f, 0.4f, 0.0f), Time.deltaTime/0.016f); // 16ms * 60  = 1 s
        }
        else
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, transform.position, Time.deltaTime/0.016f); // 16ms * 60  = 1 s
        }
    }

    [SerializeField] private float speed = 500;
    [SerializeField] private LayerMask groundLayer;


    private void GetGroundVelocityFromInput(out Vector3 trueForward, out Vector3 trueRight, out Vector3 trueDown)
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
    
    private bool sliding = false;
    private bool slideLock = false;
    [SerializeField] float slideImpulse = 20f;
    [SerializeField] float slideForce = 10f;
    
    private void FixedUpdate()
    {
        GetGroundVelocityFromInput(out Vector3 trueForward, out Vector3 trueRight, out Vector3 trueDown);
        
        // Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
        // Debug.DrawLine(transform.position, transform.position + trueForward, Color.red);
        // Debug.DrawLine(transform.position, transform.position + trueRight);

        // project
        // isolate forward vector
        if (!sliding)
        {
            if (groundState == groundStates.Grounded)
            {
                _rigidbody.velocity = speed * ((inputScript.vertical * trueForward + inputScript.horizontal * trueRight).normalized);
            }
            else if (groundState == groundStates.InAir)
            {
                _rigidbody.AddForce(trueForward * slideForce);
                _rigidbody.AddForce(GetGravity());
            }
        }
        else
        {
            if (slideStart)
            {
                slideStart = false;
                // _rigidbody.AddForce(trueForward* slideImpulse, ForceMode.Impulse);
            }

            if (groundState == groundStates.InAir)
            {
                slideLock = true;
            }
            else
            {
                _rigidbody.AddForce(GetGravity());
                if (_rigidbody.velocity.magnitude < speed * 0.4)
                {
                    _rigidbody.velocity = (0.4f * speed) * trueForward;
                }
                else
                {
                    _rigidbody.AddForce(trueForward * slideForce);
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