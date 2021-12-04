using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WallRideOrGrabber : MonoBehaviour
{
    // When this comes into a collision with a wall set the parent controller's wallGrappled to  

    private CapsuleCollider _capsule;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            wallDir = other.gameObject.transform.position - transform.position;
            Physics.Raycast(transform.position, other.gameObject.transform.position- transform.position, out RaycastHit hitInfo, 5f, LayerMask.GetMask("Ground"));
            wallNormal = hitInfo.normal;
            closestPoint = hitInfo.point;
            _wall = other.gameObject;
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
            string x = string.Format("wall at {0} with normal {1}", closestPoint, wallNormal);
            _debugText.text = x;
        }
        else
        {
            _debugText.text = "No Wall";
        }
    }
}
