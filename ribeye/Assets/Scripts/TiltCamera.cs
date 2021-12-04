using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltCamera : MonoBehaviour
{
    public GameObject camera;
    public Vector3 rotationLeft = new Vector3(0f, 0f, 2f);
    public Vector3 rotationRight = new Vector3(0f, 0f, -2f);
    public float speed = 10;

    void Update()
    {    
        /*
        if (Input.GetKey(KeyCode.A))
        {
            camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, Quaternion.Euler(rotationLeft), speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, Quaternion.Euler(rotationRight), speed * Time.deltaTime);
        }
        else
        {
            camera.transform.rotation = Quaternion.RotateTowards(camera.transform.rotation, Quaternion.identity, speed * Time.deltaTime);
        }
        Debug.Log(camera.transform.eulerAngles);
        */
        
        if (Input.GetKey(KeyCode.A))
        {
            camera.transform.Rotate(0.0f, 0.0f, 2.0f, Space.Self);
        }
        else
        {
            camera.transform.Rotate(0.0f, 0.0f, 0.0f, Space.Self);
        }

        if (Input.GetKey(KeyCode.D))
        {
            camera.transform.Rotate(0.0f, 0.0f, -2.0f, Space.Self);
        }
        else
        {
            camera.transform.Rotate(0.0f, 0.0f, 0.0f, Space.Self);
        }
    }
}
