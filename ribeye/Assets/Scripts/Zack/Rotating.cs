using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : MonoBehaviour {

    [SerializeField] private Vector3 axisAndSpeed = Vector3.right;

    private void Update() {
        transform.localRotation *= Quaternion.Euler(axisAndSpeed * Time.deltaTime);
    }

}