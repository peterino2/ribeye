using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletBezierFollow : MonoBehaviour {

    #region Declares

    //Declare serializables
    [SerializeField] private LineRenderer lineRenderer = null;
    [SerializeField] private GameObject bullet = null;

    //Declare publics
    [NonSerialized] public bool run = false;
    [NonSerialized] public int index = 0;

    #endregion

    private void Update() {
        //Check
        if (run) {
            //Move
            bullet.transform.position = lineRenderer.GetPosition(index);
            //Check index
            if (index == 0) {
                //Enable
                lineRenderer.gameObject.SetActive(true);
                //Enable
                bullet.SetActive(true);
            }
            //Increase
            index++;
        }
        //Check
        if (index >= lineRenderer.positionCount) {
            //Disable
            run = false;
            //Disable
            lineRenderer.gameObject.SetActive(false);
            //Disable
            bullet.SetActive(false);
        }
    }

}