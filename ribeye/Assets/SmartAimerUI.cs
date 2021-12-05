using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game;
using Gameplay.Gunner;
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


    [SerializeField] public GameObject imagePrefab;
    [SerializeField] private GameObject poolParent;
    [SerializeField] public int targetingPoolSize = 15;
    
    public Image[] positionPool;
    private RectTransform _rect;
    
    void Start()
    {
        _rect = GetComponent<RectTransform>(); // scale this based on FOV and screen size
        if (!mainCamera) mainCamera = FindObjectOfType<Camera>();
        SetMode(mode);
        Array.Resize(ref positionPool, targetingPoolSize);

        for (var i = 0; i < targetingPoolSize; i++)
        {
            var o = Instantiate(imagePrefab, poolParent.transform);
            positionPool[i] = o.GetComponent<Image>();
            positionPool[i].sprite = aimerRevolverEnemy;
            positionPool[i].color = enemyTargetColor;
            positionPool[i].enabled = false;
        }
    }

    private Vector3 cameraCenter;
    void SetMode(RibWeaponSmartPistol.SmartPistolModes m)
    {
        cameraCenter = new Vector3(mainCamera.pixelWidth/2f, mainCamera.pixelHeight/2f, 0f);
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
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            SetMode(mode == RibWeaponSmartPistol.SmartPistolModes.Revolver ? RibWeaponSmartPistol.SmartPistolModes.Smart : RibWeaponSmartPistol.SmartPistolModes.Revolver);
        }

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

    List<GameObject> confirmedTargets = new List<GameObject>();
    List<Vector3> targetPointsOnScreen = new List<Vector3>();
    
    void HandleSmartTargetingVisuals()
    {
        var targets = FindObjectsOfType<Targetable>();
        confirmedTargets.Clear();
        targetPointsOnScreen.Clear();

        foreach(var target in targets)
        {
            Vector3 targetWorldPos;
            Vector3 point;
            
            if (target.targetSet)
            {
                 targetWorldPos = target.targetingLoc.transform.position;
            }
            else
            {
                targetWorldPos = target.targetingLoc.transform.position;
            }
            
            point = mainCamera.WorldToScreenPoint(targetWorldPos);
            
            if (Vector3.Dot(mainCamera.transform.forward, targetWorldPos - mainCamera.transform.position) < 0)
            {
                continue;
            }
            
            var r = _rect.rect;
            var targetPoint = point - cameraCenter;
            
            if (
                targetPoint.x > (-0.5 * r.width) && 
                targetPoint.x < (0.5 * r.width) && 
                targetPoint.y > (-0.5 * r.height) && 
                targetPoint.y < (0.5 * r.height) 
            )
            {
                AddTargetingObject(point);
                confirmedTargets.Add(target.gameObject);
            }
        }
        print(confirmedTargets.Count());
    }

    void AddTargetingObject(Vector3 point)
    {
        // notify the gun here.
        targetPointsOnScreen.Add(point);
    }
}
