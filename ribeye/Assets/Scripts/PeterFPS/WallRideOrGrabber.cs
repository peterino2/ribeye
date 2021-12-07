using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WallRideOrGrabber : MonoBehaviour
{
    // When this comes into a collision with a wall set the parent controller's wallGrappled to

    private CapsuleCollider _capsule;
    public Collider o;
    public GameObject _wall;
    public Vector3 wallNormal;
    public Vector3 wallDir;
    public Vector3 closestPoint;

    [SerializeField] private TextMeshProUGUI _debugText;

    // Start is called before the first frame update
    void Start()
    {
        _capsule = GetComponent<CapsuleCollider>();
        _wall = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_wall == null && other.gameObject.CompareTag("Wall"))
        {
            o = other;
            _wall = other.gameObject;
            Physics.Raycast(transform.position, closestPoint - transform.position, out RaycastHit hitInfo, 5f, LayerMask.GetMask("Ground"));
            wallNormal = hitInfo.normal;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_wall)
        {
            if (other.gameObject.GetInstanceID() == _wall.GetInstanceID())
            {
                _wall = null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_wall)
        {
            closestPoint = o.ClosestPoint(transform.position);

            wallDir = closestPoint - transform.position;
            Physics.Raycast(transform.position, closestPoint - transform.position, out RaycastHit hitInfo, 5f, LayerMask.GetMask("Ground"));
            wallNormal = hitInfo.normal;

            string x = string.Format("wall at {0} with normal {1}", closestPoint, wallNormal);
            _debugText.text = x;

            if (wallNormal.y > 0.3f)
            {
                _wall = null;
                closestPoint = o.ClosestPoint(transform.position);

                wallDir = closestPoint - transform.position;
                wallNormal = hitInfo.normal;
            }
        }
        else
        {
            _debugText.text = "No Wall";
        }
    }
}