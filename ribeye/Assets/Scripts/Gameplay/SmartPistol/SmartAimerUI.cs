using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game;
using Gameplay.Gunner;
using Gameplay.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class SmartAimerUI : MonoBehaviour
{
    RibWeaponSmartPistol.SmartPistolModes mode = RibWeaponSmartPistol.SmartPistolModes.Revolver;
    
    // Start is called before the first frame update
    
    [SerializeField]
    public Sprite aimerRevolverEnemy;
    
    [SerializeField]
    public Sprite aimerSmartEnemy;
    
    [SerializeField] public Color smartColor;
    [SerializeField] public Color enemyTargetColor;
    [SerializeField] public Color neutralColor;
    
    [SerializeField] public Image imageTl;
    [SerializeField] public Image imageTr;
    [SerializeField] public Image imageBl;
    [SerializeField] public Image imageBr;
    [SerializeField] public TextMeshProUGUI targetingText;
    [SerializeField] public Camera mainCamera;
    
    [SerializeField] public Image reticuleMain;
    [SerializeField] public Image hitmarker;


    private float hitmarkerFade = 0.0f;

    [SerializeField] public GameObject imagePrefab;
    [SerializeField] private GameObject poolParent;
    [SerializeField] public int targetingPoolSize = 15;
    
    public Image[] positionPool;
    private RectTransform _rect;

    public float range;
    
    void Start()
    {
        _rect = GetComponent<RectTransform>(); // scale this based on FOV and screen size
        SetMode(mode);
        Array.Resize(ref positionPool, targetingPoolSize);

        for (var i = 0; i < targetingPoolSize; i++)
        {
            var o = Instantiate(imagePrefab, poolParent.transform);
            positionPool[i] = o.GetComponent<Image>();
            positionPool[i].sprite = aimerSmartEnemy;
            positionPool[i].color = enemyTargetColor;
            positionPool[i].enabled = false;
        }
    }
    
    public void SetMode(RibWeaponSmartPistol.SmartPistolModes m)
    {
        mode = m;
        if (m == RibWeaponSmartPistol.SmartPistolModes.Revolver)
        {
            // todo scale the base canvas position based on 
            imageTl.enabled = false;
            imageTr.enabled = false;
            imageBl.enabled = false;
            imageBr.enabled = false;
            targetingText.enabled = false;
            targetingText.text = "";

            reticuleMain.enabled = true;
            foreach (var pooledImage in positionPool)
            {
                pooledImage.enabled = false;
            }
            confirmedTargets.Clear();
            targetPointsOnScreen.Clear();

            SetColors(neutralColor);
        }

        if (m == RibWeaponSmartPistol.SmartPistolModes.Smart)
        {
            imageTl.enabled = true;
            imageTr.enabled = true;
            imageBl.enabled = true;
            imageBr.enabled = true;
            targetingText.enabled = true;
            targetingText.text = "Targeting mode: Smart";
            reticuleMain.enabled = false;
            SetColors(smartColor);
        }
    }

    void SetColors(Color col)
    {
        imageBl.color = col;
        imageTl.color = col;
        imageBr.color = col;
        imageTr.color = col;
        
        reticuleMain.color = col;
    }

    void Update()
    {
        hitmarkerFade -= Time.deltaTime * 5;
        var c = hitmarker.color;
        hitmarker.color = new Color(c.r, c.g, c.b, hitmarkerFade);
        
        Ray r = mainCamera.ScreenPointToRay(_rect.position);
        
        Debug.DrawLine(r.origin, r.direction*100f + r.origin);
        
        if (mode == RibWeaponSmartPistol.SmartPistolModes.Smart)
        {
            HandleSmartTargetingVisuals();
            
            int i = 0;
            foreach (var poolObj in positionPool)
            {
                if (i < confirmedTargets.Count())
                {
                    poolObj.enabled = true;
                    poolObj.transform.position = new Vector3(targetPointsOnScreen[i].x, targetPointsOnScreen[i].y, 0f);
                }
                else
                {
                    poolObj.enabled = false;
                }
                i += 1;
            }
        }
    }

    public List<GameObject> confirmedTargets = new List<GameObject>();
    public List<Vector3> targetPointsOnScreen = new List<Vector3>();

    [SerializeField] private LayerMask mask;

    [SerializeField] GameObject test;

    public void Hitmarker()
    {
        hitmarkerFade = 1;
    }

    private EntityBase nearest;
    private float nearest_mag;
    void HandleSmartTargetingVisuals()
    {
        var targets = FindObjectsOfType<RibTargetable>();
        confirmedTargets.Clear();
        targetPointsOnScreen.Clear();

        nearest_mag = 100000f; 
        nearest = null;
        
        print(targets.Length);
        
        foreach(var target in targets)
        {
            Vector3 targetWorldPos;
            Vector3 point;
            if (!target.target.alive)
            {
                continue;
            }
            
            targetWorldPos = target.targetingLoc.transform.position;
            if ((targetWorldPos - mainCamera.transform.position).magnitude > range)
            {
                continue;
            }

            var castv = targetWorldPos - mainCamera.transform.position;
            if (
                Physics.Raycast(
                    mainCamera.transform.position, 
                    castv,
                    out RaycastHit hit, castv.magnitude, mask
                    )
                )
            {
                continue;
            }

            point = mainCamera.WorldToScreenPoint(targetWorldPos);
            
            if (Vector3.Dot(mainCamera.transform.forward, targetWorldPos - mainCamera.transform.position) < 0)
            {
                continue;
            }
            
            var r = _rect.rect;
            var targetPoint = point - _rect.position;
            var close = targetPoint.magnitude;
            
            if (
                targetPoint.x > (-0.5 * r.width) && 
                targetPoint.x < (0.5 * r.width) && 
                targetPoint.y > (-0.5 * r.height) && 
                targetPoint.y < (0.5 * r.height) 
            ) {
                AddTargetingObject(point);
                confirmedTargets.Add(target.gameObject);
                if (close < nearest_mag)
                {
                    nearest_mag = close;
                    nearest = target.target;
                }
            }
        }
    }


    public LayerMask playermask;
    public bool GetCenterTarget(out Transform objectHit, out RaycastHit rayhit)
    {
        Ray ray = mainCamera.ScreenPointToRay(_rect.position);
        objectHit = transform;
        
        if (Physics.Raycast(ray, out rayhit, Mathf.Infinity, playermask)) {
            objectHit = rayhit.transform;
            return true;
        }

        return false;
    }

    public EntityBase GetNearestTarget()
    {
        if (confirmedTargets.Count() > 0)
        {
            return nearest;
        }
        else
        {
            return null;
        }
    }

    void AddTargetingObject(Vector3 point)
    {
        // notify the gun here.
        targetPointsOnScreen.Add(point);
    }
}
