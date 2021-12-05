using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveTracer : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    [SerializeField] private GameObject tracerPrefab = null;
    [SerializeField] private int numberOfTracers = 30;

    [Header("Specifications")]
    [SerializeField] private int numberOfPoints = 5;
    [SerializeField] private float multiplerCurving = 2f;
    [SerializeField] private float multiplerForwarding = 2f;

    //Declare privates
    private TracerClass[] tracers = null;
    private LinePositions linePositions = null;
    private int nextTracer = 0;
    private int lengthCalculated = 0;

    #endregion

    #region Classes

    private class TracerClass {
        //Declare
        public GameObject TheGameObject = null;
        public LineRenderer TheLineRenderer = null;
        //Constructor
        public TracerClass(GameObject theGameObject, int numberOfPoints) {
            //Set
            TheGameObject = theGameObject;
            //Set
            TheLineRenderer = TheGameObject.GetComponent<LineRenderer>();
            //Disable
            TheGameObject.SetActive(false);
            //Set positions count
            TheLineRenderer.positionCount = numberOfPoints;
        }
    }

    private class LinePositions {
        //Declare
        public float[] TimePositions = null;
        public Vector3[] Positions = null;
        //Constructor
        public LinePositions(int numberOfPoints) {
            //Expand
            TimePositions = new float[numberOfPoints];
            Positions = new Vector3[numberOfPoints];
            //Loop to create time
            for (int i = 0; i < TimePositions.Length; i++) {
                //Set
                TimePositions[i] = (i / (float)(numberOfPoints - 1));
            }
        }
    }

    #endregion

    #region Start and initialization

    private void Start() {
        //Create object pool
        CreateObjectPool();
        //Create line positions
        linePositions = new LinePositions(numberOfPoints);
        //Set
        lengthCalculated = linePositions.Positions.Length - 1;
    }

    private void CreateObjectPool() {
        //Expand
        tracers = new TracerClass[numberOfTracers];
        //Loop
        for (int i = 0; i < tracers.Length; i++) {
            //Create
            tracers[i] = new TracerClass(Instantiate(tracerPrefab, transform), numberOfPoints);
        }
    }

    #endregion

    #region Public functions

    [SerializeField] private Transform Gun = null;
    [SerializeField] private Transform Enemy = null;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            ShowTracer(Gun, Enemy.position);
        }
    }

    public void ShowTracer(Transform gunMuzzlePoint, Vector3 endingPosition) {
        //Declare
        float m = multiplerCurving;

        m = Random.Range(-6, 6);
        float u = Random.Range(-4, 2);
        Vector3 startingCurve = gunMuzzlePoint.position + (gunMuzzlePoint.right * m) + (gunMuzzlePoint.forward * multiplerForwarding) + (-gunMuzzlePoint.up * u);
        
        // Vector3 startingCurve = gunMuzzlePoint.position + ()
        Vector3 endingCurve = endingPosition + (-gunMuzzlePoint.right * m) + (-gunMuzzlePoint.forward * multiplerForwarding) + (-gunMuzzlePoint.up * u);
        //Loop
        for (int i = 0; i < lengthCalculated; i++) {
            //Set
            linePositions.Positions[i] = GetPositionWithTwoPointQuadraticBezierCurve(linePositions.TimePositions[i], gunMuzzlePoint.position, startingCurve, endingCurve, endingPosition);
        }
        //Set last
        linePositions.Positions[lengthCalculated] = endingPosition;
        //Set positions
        tracers[nextTracer].TheLineRenderer.SetPositions(linePositions.Positions);
        //Enable
        tracers[nextTracer].TheGameObject.SetActive(true);
        //Increase
        nextTracer = (nextTracer + 1) % numberOfPoints;
    }

    #endregion

    #region Math functions

    private Vector3 GetPositionWithTwoPointQuadraticBezierCurve(float time, Vector3 position0, Vector3 position1, Vector3 position2, Vector3 position3) {
        //Return
        return (Mathf.Pow(1 - time, 3) * position0) + (3 * Mathf.Pow(1 - time, 2) * time * position1) + (3 * (1 - time) * Mathf.Pow(time, 2) * position2) + (Mathf.Pow(time, 3) * position3);
    }

    #endregion

}