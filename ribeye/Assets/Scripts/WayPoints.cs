using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class WayPoints : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    public WayPoint[] wayPoints = null;

    [Header("Debugging")]
    [SerializeField] private bool drawPath = true;
    [SerializeField] private Color nextTransformsColor = Color.green;
    [SerializeField] private Color previousTransformsColor = Color.grey;

    #endregion

    #region Constants

    //Declare private constants
    private const float SIZE_OFFSET = 0.85f;

    #endregion

    #region On gizmos

    #if UNITY_EDITOR

    private void OnDrawGizmos() {
        //Check to draw path
        if (drawPath) {
            //Loop
            for (int i = 0; i < wayPoints.Length; i++) {
                //Set color
                Handles.color = nextTransformsColor;
                //Loop
                for (int j = 0; j < wayPoints[i].nextTransforms.Length; j++) {
                    //Declare
                    Vector3 direction = wayPoints[i].nextTransforms[j].transform.position - wayPoints[i].transform.position;
                    float size = Vector3.Distance(wayPoints[i].nextTransforms[j].transform.position, wayPoints[i].transform.position) * SIZE_OFFSET;
                    //Draw line
                    Handles.ArrowHandleCap(0, wayPoints[i].transform.position, Quaternion.LookRotation(direction), size, EventType.Repaint);
                }
                //Set color
                Handles.color = previousTransformsColor;
                //Loop
                for (int j = 0; j < wayPoints[i].previousTransforms.Length; j++) {
                    //Declare
                    Vector3 direction = wayPoints[i].previousTransforms[j].transform.position - wayPoints[i].transform.position;
                    float size = Vector3.Distance(wayPoints[i].previousTransforms[j].transform.position, wayPoints[i].transform.position) * SIZE_OFFSET;
                    //Draw line
                    Handles.ArrowHandleCap(0, wayPoints[i].transform.position, Quaternion.LookRotation(direction), size, EventType.Repaint);
                }
            }
        }
    }

    #endif

    #endregion

}