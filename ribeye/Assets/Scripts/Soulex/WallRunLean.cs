using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunLean : MonoBehaviour
{
    public Transform cam;
    public Transform camP;
    [SerializeField] private AnimationCurve animCurve;
    public bool isLeaning = false;
    Vector3 ogPos;
    private void Start()
    {
        ogPos = camP.localPosition;
    }
    private void Update()
    {
        if (!isLeaning)
        {
            camP.localPosition = Vector3.Lerp(camP.localPosition, Vector3.zero, Time.deltaTime * 5);
            cam.localRotation = Quaternion.Slerp(cam.localRotation, Quaternion.identity, Time.deltaTime * 5);
        }
    }
    public IEnumerator Lean(float dir)
    {
        isLeaning = true;
        float pos = 0;
        while (pos < 1)
        {
            pos += Time.deltaTime * 0.65f;
            cam.localRotation = Quaternion.Slerp(cam.localRotation, Quaternion.Euler(new Vector3(0, 0, animCurve.Evaluate(pos) * 12 * dir)), Time.deltaTime * 8);
            camP.localPosition = Vector3.Lerp(camP.localPosition, new Vector3(0, ogPos.y + animCurve.Evaluate(pos) * 1.25f, 0), Time.deltaTime * 8);
            yield return null;
        }
        isLeaning = false;
    }
}