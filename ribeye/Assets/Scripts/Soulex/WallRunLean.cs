using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunLean : MonoBehaviour
{
    public Transform cam;
    public Transform camP;
    //[SerializeField] private Transform player;
    [SerializeField] private AnimationCurve animCurve;
    public bool isLeaning = false;
    public IEnumerator Lean(float dir)
    {
        isLeaning = true;
        float pos = 0;
        Vector3 ogPos = camP.localPosition;
        while (pos < 1)
        {
            pos += Time.deltaTime * 0.65f;
            cam.localRotation = Quaternion.Euler(new Vector3(0, 0, animCurve.Evaluate(pos) * 12 * dir));
            camP.localPosition = new Vector3(0, ogPos.y + animCurve.Evaluate(pos) * 1.25f, 0);
            yield return null;
        }
        //StartCoroutine(ResetLean());
    }
    private void Update()
    {
        if (!isLeaning)
        {
            camP.localPosition = Vector3.Lerp(camP.localPosition, Vector3.zero, Time.deltaTime * 5);
            cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.identity, Time.deltaTime * 5);
        }
    }
    //public IEnumerator ResetLean()
    //{
    //    float w = 0;
    //    while (w < 2)
    //    {
    //        w += Time.deltaTime;
    //        camP.localPosition = Vector3.Lerp(camP.localPosition, Vector3.zero, Time.deltaTime * 5);
    //        cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.identity, Time.deltaTime * 5);
    //        yield return null;
    //    }
    //    camP.localPosition = Vector3.zero;
    //    cam.localRotation = Quaternion.identity;
    //}
}