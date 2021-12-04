using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunLean : MonoBehaviour
{
    public Transform cam;
    //[SerializeField] private Transform player;
    [SerializeField] private AnimationCurve animCurve;
    public IEnumerator Lean(float dir)
    {
        float pos = 0;
        Vector3 ogPos = cam.localPosition;
        while (pos < 1)
        {
            pos += Time.deltaTime * 0.6f;
            cam.localRotation = Quaternion.Euler(new Vector3(0, 0, animCurve.Evaluate(pos) * 10 * dir));
            cam.localPosition = new Vector3(0, ogPos.y + animCurve.Evaluate(pos), 0);
            yield return null;
        }
        StartCoroutine(ResetLean());
    }
    public IEnumerator ResetLean()
    {
        float w = 0;
        while (w < 1)
        {
            w += Time.deltaTime;
            cam.localPosition = Vector3.Lerp(cam.localPosition, Vector3.zero, Time.deltaTime * 10);
            cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.identity, Time.deltaTime * 10);
            yield return null;
        }
        cam.localPosition = Vector3.zero;
        cam.localRotation = cam.localRotation;
    }
}