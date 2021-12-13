using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHover : MonoBehaviour
{
    private float yPos;
    private float newPos;
    private void Start()
    {
        yPos = transform.position.y;
        newPos = 0.35f;
    }

    public float yrot = 30;
    public float xrot = 0;
    public float zrot = 0;
    void Update()
    {
        transform.position = new Vector3(transform.position.x, yPos + ((Mathf.Sin(Time.time * 1.2f) + 1 ) * newPos / 2), transform.position.z);
        transform.Rotate(xrot * Time.deltaTime, yrot * Time.deltaTime, zrot * Time.deltaTime);
    }
}