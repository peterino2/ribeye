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
    void Update()
    {
        transform.position = new Vector3(transform.position.x, yPos + ((Mathf.Sin(Time.time * 1.2f) + 1 ) * newPos / 2), transform.position.z);
        transform.Rotate(0, 30 * Time.deltaTime, 0);
    }
}