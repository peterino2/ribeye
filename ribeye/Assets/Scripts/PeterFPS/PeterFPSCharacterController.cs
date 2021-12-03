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
    }

    private void Update() {
        //Set input
        
        inputScript.SetInput();
        
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

        transform.localRotation = Quaternion.Euler(0, mousex, 0);
    }

    private void LateUpdate()
    {
        cameraTransform.rotation = Quaternion.Euler(-mousey, mousex, 0);
    }


    [SerializeField] private float speed = 500;
    [SerializeField] private LayerMask groundLayer;

    private void GetGroundVelocityFromInput(out Vector3 trueForward, out Vector3 trueRight)
    {
        Physics.Raycast(transform.position, -transform.up, out RaycastHit hitInfo, 5f, groundLayer);
        
        if (groundState == groundStates.Grounded)
        {
            trueForward = Vector3.ProjectOnPlane(transform.forward, hitInfo.normal).normalized;
            trueRight = Vector3.ProjectOnPlane(transform.right, hitInfo.normal).normalized;
        }
        else
        {
            trueForward = transform.forward;
            trueRight = transform.right;
        }
    }
    
    private void FixedUpdate()
    {
        GetGroundVelocityFromInput(out Vector3 trueForward, out Vector3 trueRight);
        // Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
        // Debug.DrawLine(transform.position, transform.position + trueForward, Color.red);
        
        // Debug.DrawLine(transform.position, transform.position + trueRight);

        // project
        // isolate forward vector
        _rigidbody.velocity = speed * ((inputScript.vertical * trueForward + inputScript.horizontal * trueRight).normalized) ;
        _rigidbody.velocity += GetGravity();
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