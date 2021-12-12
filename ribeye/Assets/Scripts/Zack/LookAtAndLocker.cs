using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LookAtAndLocker : MonoBehaviour {

    #region Declares

    //Declare serializables
    [Header("Setup")]
    public Transform target = default;

    [Header("Specifications")]
    public float Speed = 100f;
    [SerializeField] public enumAxis Axis = enumAxis.X;

    [Header("Locking")]
    [SerializeField] public bool CanLock = false;
    [Range(0f, 180f)] public float LowerBoundGreen = 90f;
    [Range(0f, 180f)] public float UpperBoundBlue = 90f;

    [Header("Handles")]
    public float SphereSize = 0.25f;
    public float DrawDistance = 2f;
    public Color SphereColor = Color.white;
    public Color LineToSphereColor = Color.white;
    public Color ArcColor = Color.red;
    public Color LowerBoundGreenColor = Color.green;
    public Color UpperBoundBlueColor = Color.blue;

    //Declare privates
    private Transform trnDummy = null;
    private float fltAngleCache = 0f;

    #endregion

    #region Enumerators

    public enum enumAxis {
        X, Y, Z
    }

    private enum enumHalfWayAngleTypes {
        Middle, Left, Right
    }

    private enum enumRotationAxes {
        X, Y, Z
    }

    #endregion

    #region Constants

    //Declare constants
    private const float fltHALF_ROTATION = 180f;
    private const float fltFULL_ROTATION = 360f;
    private const string strSPACE_DUMMY = " Dummy";

    #endregion

    #region Start and initialize

    private void Start() {
        //Create
        trnDummy = new GameObject(gameObject.name + strSPACE_DUMMY).transform;
        //Parent
        trnDummy.SetParent(transform.parent, false);
        //Match scale
        trnDummy.localScale = transform.localScale;
        //Set position
        trnDummy.SetPositionAndRotation(transform.position, transform.rotation);
        //Disable
        trnDummy.gameObject.SetActive(false);
    }

    #endregion

    #region Update

    public Vector3 GetAngleDirection() {
        //Check
        switch (Axis) {
            case enumAxis.X:
                //Return
                return trnGetTransform().forward;
            case enumAxis.Y:
                //Return
                return trnGetTransform().forward;
            default:
                //Return
                return trnGetTransform().up;
        }
    }

    private Transform trnGetTransform() {
#if UNITY_EDITOR
        //Check
        if (Application.isEditor && !EditorApplication.isPlaying) {
            //Return
            return transform;
        } else {
            //Return
            return trnDummy;
        }
#else
            //Return
            return trnDummy;
#endif
    }

    public Vector3 GetAxisMatching() {
        //Return
        return Vector3.ProjectOnPlane(target.position - transform.position, GetTransformDirection(transform)) + transform.position;
    }

    public void RotateTowardsTarget(float speedChange, bool change = false) {
        //Declare
        float fltAngle = Vector3.SignedAngle(GetAngleDirection(), GetAxisMatching() - transform.position, GetTransformDirection(transform));
        //Check if can lock
        if (CanLock) {
            //Declare
            float fltUpperBound = -UpperBoundBlue;
            float fltHalfWayAngle = ((fltFULL_ROTATION - LowerBoundGreen - UpperBoundBlue) * 0.5f);
            float fltHalfWayAnglePlus = fltHalfWayAngle + LowerBoundGreen;
            enumHalfWayAngleTypes enumHalfWayAngleType = enumGetHalfWayAngle(fltHalfWayAnglePlus);
            //Check type
            switch (enumHalfWayAngleType) {
                case enumHalfWayAngleTypes.Middle:
                    //Check angles
                    if (fltAngle >= LowerBoundGreen && fltAngle <= fltHALF_ROTATION) {
                        //Lock
                        fltAngle = LowerBoundGreen;
                    } else if (fltAngle <= fltUpperBound && fltAngle >= -fltHALF_ROTATION) {
                        //Lock
                        fltAngle = fltUpperBound;
                    }
                    break;
                case enumHalfWayAngleTypes.Left:
                    //Declare
                    float fltNegativeHalf = fltUpperBound - fltHalfWayAngle;
                    //Check angles
                    if (fltAngle <= fltUpperBound && fltAngle >= fltNegativeHalf) {
                        //Lock
                        fltAngle = fltUpperBound;
                    } else if ((fltAngle <= fltNegativeHalf && fltAngle >= -fltHALF_ROTATION) || (fltAngle >= LowerBoundGreen && fltAngle <= fltHALF_ROTATION)) {
                        //Lock
                        fltAngle = LowerBoundGreen;
                    }
                    break;
                default:
                    //Declare
                    float fltPositionHalf = LowerBoundGreen + fltHalfWayAngle;
                    //Check angles
                    if ((fltAngle <= fltUpperBound && fltAngle >= -fltHALF_ROTATION) || (fltAngle <= fltHALF_ROTATION && fltAngle >= fltPositionHalf)) {
                        //Lock
                        fltAngle = fltUpperBound;
                    } else if (fltAngle <= fltPositionHalf && fltAngle >= LowerBoundGreen) {
                        //Lock
                        fltAngle = LowerBoundGreen;
                    }
                    break;
            }
            //Rotate towards
            AngleRotation(transform, ref fltAngleCache, fltAngle, fltGetSpeed(speedChange, change), (enumRotationAxes)Axis, false);
        } else {
            //Rotate towards
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, quaGetRotation(fltAngle), fltGetSpeed(speedChange, change));
        }
    }

    private bool AngleRotation(Transform trnTransform, ref float fltCurrentRotation, float fltRotationAmount, float fltRotationSpeed,
                               enumRotationAxes enumLocalEulerAngleRotationAxis, bool blnWorld) {
        //Set current rotation
        fltCurrentRotation = Mathf.MoveTowards(fltCurrentRotation, fltRotationAmount, fltRotationSpeed);
        //Check if world or local
        if (blnWorld) {
            //Check axis
            switch (enumLocalEulerAngleRotationAxis) {
                case enumRotationAxes.X:
                    //Rotate
                    trnTransform.rotation = Quaternion.Euler(fltCurrentRotation, 0f, 0f);
                    break;
                case enumRotationAxes.Y:
                    //Rotate
                    trnTransform.rotation = Quaternion.Euler(0f, fltCurrentRotation, 0f);
                    break;
                case enumRotationAxes.Z:
                    //Rotate
                    trnTransform.rotation = Quaternion.Euler(0f, 0, fltCurrentRotation);
                    break;
            }
        } else {
            //Check axis
            switch (enumLocalEulerAngleRotationAxis) {
                case enumRotationAxes.X:
                    //Rotate
                    trnTransform.localRotation = Quaternion.Euler(fltCurrentRotation, 0f, 0f);
                    break;
                case enumRotationAxes.Y:
                    //Rotate
                    trnTransform.localRotation = Quaternion.Euler(0f, fltCurrentRotation, 0f);
                    break;
                case enumRotationAxes.Z:
                    //Rotate
                    trnTransform.localRotation = Quaternion.Euler(0f, 0, fltCurrentRotation);
                    break;
            }
        }
        //Return
        return fltCurrentRotation == fltRotationAmount;
    }

    private enumHalfWayAngleTypes enumGetHalfWayAngle(float fltHalfWayAngle) {
        //Check
        if (fltHalfWayAngle == fltHALF_ROTATION) {
            //Return
            return enumHalfWayAngleTypes.Middle;
        } else if (fltHalfWayAngle > fltHALF_ROTATION) {
            //Return
            return enumHalfWayAngleTypes.Left;
        } else {
            //Return
            return enumHalfWayAngleTypes.Right;
        }
    }

    public Vector3 GetAnglePosition(float fltAngle) {
        //Declare
        Quaternion quaRotation = Quaternion.AngleAxis(fltAngle, GetTransformDirection(trnGetTransform()));
        Vector3 v3Direction = (quaRotation * GetAngleDirection()).normalized;
        //Return
        return transform.position + (v3Direction * DrawDistance);
    }

    public Vector3 GetTransformDirection(Transform trnTransformToUse) {
        //Check
        switch (Axis) {
            case enumAxis.X:
                //Return
                return trnTransformToUse.right; //x
            case enumAxis.Y:
                //Return
                return trnTransformToUse.up; //y
            default:
                //Return
                return trnTransformToUse.forward; //z
        }
    }

    private Quaternion quaGetRotation(float fltAngle) {
        //Check
        switch (Axis) {
            case enumAxis.X:
                //Return
                return Quaternion.Euler(fltAngle, 0f, 0f);
            case enumAxis.Y:
                //Return
                return Quaternion.Euler(0f, fltAngle, 0f);
            default:
                //Return
                return Quaternion.Euler(0f, 0f, fltAngle);
        }
    }

    private float fltGetSpeed(float speed, bool change) {
        //Check for speed change
        if (change) {
            //Return
            return speed * Time.deltaTime;
        } else {
            //Return
            return Speed * Time.deltaTime;
        }
    }

    #endregion

}

#if UNITY_EDITOR

[CustomEditor(typeof(LookAtAndLocker))]
public class DrawWireArc : Editor {

    #region Constants

    //Declare constants
    private const string strFORMAT_ONE = "F1";
    private const string strDEGREES = "\u00b0";

    #endregion

    #region On scene GUI

    private void OnSceneGUI() {
        //Declare
        LookAtAndLocker udcLookAtAndLocker = (LookAtAndLocker)target;
        //Check if can lock
        if (udcLookAtAndLocker.CanLock) {
            //Set color
            Handles.color = udcLookAtAndLocker.SphereColor;
            //Declare
            Vector3 v3AxisMatching = udcLookAtAndLocker.GetAxisMatching();
            //Draw sphere
            Handles.SphereHandleCap(0, v3AxisMatching, Quaternion.identity, udcLookAtAndLocker.SphereSize, EventType.Repaint);
            //Change color
            Handles.color = udcLookAtAndLocker.LineToSphereColor;
            //Draw line
            Handles.DrawLine(udcLookAtAndLocker.transform.position, v3AxisMatching);
            //Declare
            float fltTotalAngle = udcLookAtAndLocker.UpperBoundBlue + udcLookAtAndLocker.LowerBoundGreen;
            float fltRadius = Handles.ScaleValueHandle(udcLookAtAndLocker.DrawDistance, udcLookAtAndLocker.transform.position + udcLookAtAndLocker.transform.forward * udcLookAtAndLocker.DrawDistance,
                              udcLookAtAndLocker.transform.rotation, 0f, Handles.DotHandleCap, 1f);
            Vector3 v3Start = v3DrawLineAngle(udcLookAtAndLocker.UpperBoundBlueColor, -udcLookAtAndLocker.UpperBoundBlue, udcLookAtAndLocker);
            //Label
            Handles.Label(v3DrawLineAngle(udcLookAtAndLocker.LowerBoundGreenColor, udcLookAtAndLocker.LowerBoundGreen, udcLookAtAndLocker), udcLookAtAndLocker.LowerBoundGreen.ToString(strFORMAT_ONE) + strDEGREES);
            //Label
            Handles.Label(v3Start, udcLookAtAndLocker.UpperBoundBlue.ToString(strFORMAT_ONE) + strDEGREES);
            //Change color
            Handles.color = udcLookAtAndLocker.ArcColor;
            //Draw wire arc
            Handles.DrawWireArc(udcLookAtAndLocker.transform.position, v3GetNormal(udcLookAtAndLocker), v3Start - udcLookAtAndLocker.transform.position, fltTotalAngle, fltRadius);
        }
    }

    private Vector3 v3GetNormal(LookAtAndLocker udcLookAtAndLocker) {
        //Check
        switch (udcLookAtAndLocker.Axis) {
            case LookAtAndLocker.enumAxis.X:
                //Return
                return udcLookAtAndLocker.transform.right;
            case LookAtAndLocker.enumAxis.Y:
                //Return
                return udcLookAtAndLocker.transform.up;
            default:
                //Return
                return udcLookAtAndLocker.transform.forward;
        }
    }

    private Vector3 v3DrawLineAngle(Color colColor, float fltAngleReversed, LookAtAndLocker udcLookAtAndLocker) {
        //Set color
        Handles.color = colColor;
        //Declare
        Vector3 v3AnglePosition = udcLookAtAndLocker.GetAnglePosition(fltAngleReversed);
        //Draw line
        Handles.DrawLine(udcLookAtAndLocker.transform.position, v3AnglePosition);
        //Set color
        Handles.color = udcLookAtAndLocker.ArcColor;
        //Draw cone handle cap
        Handles.SphereHandleCap(0, v3AnglePosition, Quaternion.identity, 0.05f, EventType.Repaint);
        //Return
        return v3AnglePosition;
    }

    #endregion

}

#endif
