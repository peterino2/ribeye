using System;
using System.Collections;
using Game;
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

        private bool isActive = false;

        [SerializeField] private SmartPistolModes mode = SmartPistolModes.Smart;
        [SerializeField] private int smartShotIndex = 0; // see soundmanager prefab this needs to match
        [SerializeField] private int revolverShotIndex = 1; // see soundmanager prefab this needs to match
        [SerializeField] private float FireRate = 25;
        [SerializeField] private string weaponName = "SmartPistol";
        
        [SerializeField] private bool active = false;
        [SerializeField] private float range = 38;

        [SerializeField] private GameObject muzzle;
        [SerializeField] private GameObject model;
        [SerializeField] private GameObject modelBase;
        
        [SerializeField] private GameObject bulletImpact;
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
            ui.range = range;
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

        public LayerMask curveMask;

        void DoSmartFire()
        {
            fireready = false;
            var target = ui.GetNearestTarget();
            if (target != null)
            {
                var obj = target.gameObject.GetComponent<RibTargetable>();
                Physics.Raycast(transform.position, target.transform.position - transform.position, out RaycastHit r,Mathf.Infinity, curveMask);
                target.TakeDamage(damageSmart);
                ui.Hitmarker();
                bcurveGen.ShowTracer(muzzle.transform, r.point, bulletImpact);
                Instantiate(bulletImpact, r.point, Quaternion.LookRotation(r.normal));
                GameManager._soundManager.PlaySound(0, transform.position, volume:0.05f);
                GameManager.playHitSound(transform.position);
                gunAnimator.Play("PistolSmartModeShoot");
                // StartCoroutine(playFireAnim());
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

            fireready = true;
        }


        private bool revolverReady = true;
        private bool retriggerRevolver = false;
        IEnumerator RevolverHeavy()
        {
            if (revolverReady)
            {
                revolverReady = false;
                GameManager._soundManager.PlaySound(revolverShotIndex, transform.position, volume:0.2f);
                if (ui.GetCenterTarget(out Transform objectHit, out RaycastHit rayhit))
                {
                    EntityBase x = objectHit.gameObject.GetComponent<EntityBase>();
                    if (x != null)
                    {
                        x.TakeDamage(damageRevolver);
                        GameManager.playHitSound(transform.position);
                        ui.Hitmarker();
                    }
                    Instantiate(bulletImpact, rayhit.point, Quaternion.LookRotation(rayhit.normal));
                }
                gunAnimator.Play("PistolShoot");
                yield return new WaitForSeconds(0.20f);
                
                float t = 0f;
                while (t < 0.15f)
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
            if (newMode == SmartPistolModes.Revolver)
            {
                gunAnimator.Play("PistolSmartModeDeactivate");
            }
            else
            {
                gunAnimator.Play("PistolSmartModeActivate");
            }
            yield return null;
        }

        IEnumerator doEquip()
        {
            gunAnimator.Play("PistolEquip");
            if (mode == SmartPistolModes.Smart)
            {
                yield return new WaitForSeconds(0.100f);
                gunAnimator.Play("PistolSmartModeActivate");
            }
        }

        public override void OnReloadPressed()
        {
        }
        
        public override void ActivateWeapon()
        {
            modelBase.SetActive(true);
            ui.gameObject.SetActive(true);
            StartCoroutine(doEquip());
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
            DeactivateWeaponNoAnim();
        }
        
        public override void DeactivateWeaponNoAnim()
        {
            modelBase.SetActive(false);
            ui.gameObject.SetActive(false);
            mode = SmartPistolModes.Revolver;
            ui.SetMode(mode);
        }

        public override string GetWeaponName()
        {
            return weaponName;
        }

        public override void OnAltFire()
        {
            SmartPistolModes newmode =
                mode == SmartPistolModes.Smart ? SmartPistolModes.Revolver : SmartPistolModes.Smart;
            
            if (
                (newmode == SmartPistolModes.Smart && 
                    gunner.upgrades.Contains("PistolSmart")) || 
                (newmode == SmartPistolModes.Revolver)
            ) {
                StartCoroutine(switchModes(newmode));
            }
        }

        public override bool CanActivate()
        {
            return gunner.upgrades.Contains("PistolBasic");
        }

    }
}