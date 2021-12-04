using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeterFPSCameraLook : MonoBehaviour
{
    private Transform cameraTransform;
    [SerializeField] private PeterFPSCharacterController _controller;
    private void Start()
    {
        cameraTransform = transform;
    }

    private void FixedUpdate()
    {
    }

    public float sensitivity = 60f;

    private void LateUpdate()
    {
        // cameraTransform.position = _controller.transform.position;
        // cameraTransform.Rotate(Input.GetAxis("Mouse Y")*Time.deltaTime * sensitivity, 0, 0);// += Quaternion.Euler(-_controller.mousey * Time.deltaTime * 60f, _controller.mousex * Time.deltaTime* 60f, 0);
        
        // cameraTransform.Rotate(0, Input.GetAxis("Mouse X")*Time.deltaTime * sensitivity, 0);// += Quaternion.Euler(-_controller.mousey * Time.deltaTime * 60f, _controller.mousex * Time.deltaTime* 60f, 0);
        
        // cameraTransform.rotation()
        // Input.GetAxis("Mouse X")*Time.deltaTime * sensitivity
        
        // cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(-_controller.mousey, _controller.mousex, 0), Time.deltaTime / 0.016f);
        // cameraTransform.position = Vector3.Lerp(cameraTransform.position, _controller.transform.position, Time.deltaTime / 0.016f); // 16ms * 60  = 1 s
        
        // cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, Quaternion.Euler(-_controller.mousey, _controller.mousex, 0), Time.deltaTime / 0.016f);
        //cameraTransform.position = Vector3.Lerp(cameraTransform.position, _controller.transform.position, Time.deltaTime / 0.016f); // 16ms * 60  = 1 s
        
        // cameraTransform.position = _controller.transform.position; // 16ms * 60  = 1 s
        // cameraTransform.rotation = Quaternion.Euler(-_controller.mousey, _controller.mousex, 0);
    }
}