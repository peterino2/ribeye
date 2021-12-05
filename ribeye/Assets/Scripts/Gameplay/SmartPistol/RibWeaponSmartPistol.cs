﻿using System;
using System.Collections;
using Gameplay.Stats;
using UnityEngine;
using UnityEngine.Assertions;

namespace Gameplay.Gunner
{
    public class RibWeaponSmartPistol : RibWeaponBase
    {
        public enum SmartPistolModes
        {
            Smart,
            Revolver
        }

        [SerializeField] private SmartPistolModes mode = SmartPistolModes.Smart;
        [SerializeField] private int smartShotIndex = 0; // see soundmanager prefab this needs to match
        [SerializeField] private int revolverShotIndex = 1; // see soundmanager prefab this needs to match
        [SerializeField] private float FireRate = 25;
        [SerializeField] private GameObject AimerPrefab;
        [SerializeField] private string weaponName = "SmartPistol";
        
        [SerializeField] private bool active = false;

        [SerializeField] private GameObject model;
        [SerializeField] private GameObject modelBase;
        [SerializeField] private AnimationCurve smartPistolFire;
        [SerializeField] private AnimationCurve smartPistolBasePosition;
        
        [SerializeField] private float damageSmart = 1.0f;
        [SerializeField] private float damageRevolver = 1.5f;
        
        [SerializeField] private SmartAimerUI ui;


        private void Awake()
        {
            if (!active)
            {
                modelBase.SetActive(false);
            }

            bcurveGen = GetComponent<BezierCurveTracer>();
            Assert.IsTrue(bcurveGen != null);
            Assert.IsTrue(ui != null);
            
            if(ui == null) ui = FindObjectOfType<SmartAimerUI>();
        }

        private BezierCurveTracer bcurveGen;
        private void Start()
        {
            StartCoroutine(switchModes(mode));
        }
        
        private bool fireready = true;
        
        public override void OnFire()
        {
            if (mode == SmartPistolModes.Smart)
            {
                StartCoroutine(FullAutoSmart());
            }
            else if (mode == SmartPistolModes.Revolver)
            {
                if (revolverReady)
                {
                    StartCoroutine(RevolverHeavy());
                }
            }
        }

        void DoSmartFire()
        {
            fireready = false;
            var target = ui.GetNearestTarget();
            if (target != null)
            {
                target.TakeDamage(damageSmart);
                bcurveGen.ShowTracer(model.transform, target.transform.position);
                GameManager._soundManager.PlaySound(0, transform.position, volume:0.3f);
                StartCoroutine(playFireAnim());
            } 
            fireready = true;
        }

        IEnumerator FullAutoSmart()
        {
            while (Input.GetKey(KeyCode.Mouse0) && fireready)
            {
                DoSmartFire();
                yield return new WaitForSeconds(1 / FireRate);
            }
        }


        private bool revolverReady = true;
        private bool retriggerRevolver = false;
        IEnumerator RevolverHeavy()
        {
            if (revolverReady)
            {
                revolverReady = false;
                GameManager._soundManager.PlaySound(revolverShotIndex, transform.position, volume:0.3f);
                if (ui.GetCenterTarget(out Transform objectHit, out RaycastHit rayhit))
                {
                    EntityBase x = objectHit.gameObject.GetComponent<EntityBase>();
                    if (x != null)
                    {
                        x.TakeDamage(damageRevolver);
                    }
                }
                StartCoroutine(playFireAnim());
                yield return new WaitForSeconds(0.20f);
                
                float t = 0f;
                while (t < 0.10f)
                {
                    t += Time.deltaTime;
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        retriggerRevolver = true;
                    }

                    yield return null;
                }
                revolverReady = true;
            }
        }

        IEnumerator switchModes(SmartPistolModes newMode)
        {
            mode = newMode;
            ui.SetMode(mode);
            yield return null;
        }

        IEnumerator playFireAnim()
        {
            float time = 0;
            while (time < smartPistolFire.length)
            {
                float yoffset = smartPistolFire.Evaluate(time);
                time += Time.deltaTime;
                model.transform.localRotation = Quaternion.Euler(0, 0, yoffset * 15f);
                yield return null;
            }
            
            model.transform.localRotation = Quaternion.Euler(0, 0, 0);
        } 

        public override void ActivateWeapon()
        {
            modelBase.SetActive(true);
            ui.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (retriggerRevolver && revolverReady)
            {
                retriggerRevolver = false;
                if (mode == SmartPistolModes.Revolver)
                {
                    StartCoroutine(RevolverHeavy());
                }
            }
        }

        public override void DeactivateWeapon()
        {
            modelBase.SetActive(false);
            ui.gameObject.SetActive(false);
        }

        public override string GetWeaponName()
        {
            return weaponName;
        }

        public override void OnAltFire()
        {
            SmartPistolModes newmode =
                mode == SmartPistolModes.Smart ? SmartPistolModes.Revolver : SmartPistolModes.Smart;
            StartCoroutine(switchModes(newmode));
        }

    }
}