using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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

    [SerializeField] private Transform cameraPos;
    [SerializeField] private Transform cameraPosP;

    [Header("Specifications")]
    [SerializeField] private float RotatorSpeed = 500f;

    [HideInInspector] public Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _capsule;

    [Header("Polish")]
    [SerializeField] private AnimationCurve fallCurve;
    [SerializeField] private Camera cam;
    [SerializeField] private WallRunLean lean;

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
        //_capsule = GetComponent<CapsuleCollider>();
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

    private bool dashReady = true;
    private bool dashing = false;
    [SerializeField] private float dashSpeed = 10f;

    IEnumerator doDash()
    {
        if (dashReady && !sliding)
        {
            print("Dashing");
            dashReady = false;
            dashing = true;
            Vector3 cachedVelocity = _rigidbody.velocity;
            Vector3 f = travelVector;
            if (f.magnitude < 0.5f)
            {
                f = transform.forward;
            }
            _rigidbody.velocity = f * dashSpeed;
            yield return new WaitForSeconds(0.3f);
            dashing = false;

            _rigidbody.velocity = cachedVelocity;
            yield return new WaitForSeconds(0.7f);
            dashReady = true;
        }
    }

    private void HandleWallGrabberUpdate()
    {
        if (wallGrappler._wall && groundState == groundStates.InAir)
        {
            if (Vector3.Dot(travelVector, wallGrappler.wallDir.normalized) > 0.5f)
            {
                StartCoroutine(doWallGrab());
            }
        }
    }

    private bool wallgrabready = true;
    private bool wallgrabbed = false;
    IEnumerator doWallGrab()
    {
        if (wallgrabready)
        {
            wallgrabbed = true;
            wallgrabready = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.useGravity = false;
            lean.StartCoroutine(lean.Lean());
            yield return new WaitForSeconds(0.4f);
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
    private void Update() {
        //Set input

        inputScript.SetInput();

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

        if (!sliding && !jumping && !dashing && !underObject)
        {
            _capsule.center = new Vector3(0, 0.09f, 0);
            cameraPosP.localPosition = Vector3.Lerp(cameraPosP.localPosition, new Vector3(0, 0.5f, 0), Time.deltaTime * 8);
            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, Quaternion.identity, Time.deltaTime * 10);
            if (groundState == groundStates.Grounded)
            {
                _rigidbody.velocity = speed * (travelVector);
                _rigidbody.useGravity = false;
            }
        }
        else if(sliding)
        {
            _capsule.center = new Vector3(0,  -0.67f, 0);
            cameraPosP.localPosition = Vector3.Lerp(cameraPosP.localPosition, new Vector3(0, -0.75f, 0), Time.deltaTime * 8);
            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, Quaternion.Euler(-10, 0, 0), Time.deltaTime * 2);
            HandleSlidingFixedUpdate(trueForward, trueRight, trueDown);
        }

        if (wallgrabbed)
        {
            _rigidbody.useGravity = false;
            _rigidbody.velocity = -Vector3.up * 0.2f;
        }

        if (groundState == groundStates.InAir && !dashing)
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
