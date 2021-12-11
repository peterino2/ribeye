using System;
using System.Collections;
using Game;
using Gameplay.Core;
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
        [SerializeField] private string weaponName = "SmartPistol";
        
        [SerializeField] private bool active = false;
        [SerializeField] private float range = 38;

        [SerializeField] private GameObject muzzle;
        [SerializeField] private GameObject modelBase;
        [SerializeField] private GameObject modelBase1;
        [SerializeField] private GameObject modelBase2;
        
        [SerializeField] private GameObject bulletImpact;
        [SerializeField] private float damageSmart = 1.0f;
        [SerializeField] private float damageRevolver = 1.5f;
        
        [SerializeField] private SmartAimerUI ui;


        private void Awake()
        {
            if (!active)
            {
                modelBase.SetActive(false);
                modelBase1.SetActive(false);
                modelBase2.SetActive(false);
            }

            bcurveGen = GetComponent<BezierCurveTracer>();
            Assert.IsTrue(bcurveGen != null);
            Assert.IsTrue(ui != null);
            
            if(ui == null) ui = FindObjectOfType<SmartAimerUI>();
        }

        private BezierCurveTracer bcurveGen;
        private void Start()
        {
            //StartCoroutine(switchModes(mode));
            mode = SmartPistolModes.Revolver;
            ui.SetMode(mode);
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
                gunAnimator.Play("SmartPistolShoot");
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

        [SerializeField] Vector3 revolverRecoilEuler = new Vector3(0,5, -5);
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

                    var wrather = objectHit.gameObject.GetComponent<RibHumanoidEnemy>();
                    if (wrather)
                    {
                        print("wrather shot");
                        if (((wrather.head.transform.position + wrather.head.transform.up * 0.06f) - rayhit.point).magnitude < 0.25f)
                        {
                            print("Taking Headshot Damage");
                            wrather.TakeHeadShotDamage(damageRevolver);
                            ui.Hitmarker();
                        }
                        else
                        {
                            x.TakeDamage(damageRevolver);
                            ui.Hitmarker();
                        }
                    }
                    else if (x != null)
                    {
                        x.TakeDamage(damageRevolver);
                        ui.Hitmarker();
                    }
                
                    Instantiate(bulletImpact, rayhit.point, Quaternion.LookRotation(rayhit.normal));
                }
                gunAnimator.Play("RevolverShot");
                gunner.rotationTarget = Quaternion.Euler(revolverRecoilEuler);
                yield return new WaitForSeconds(0.10f);
                
                gunner.rotationTarget = Quaternion.identity;
                yield return new WaitForSeconds(0.10f);
                
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
                gunAnimator.Play("SmartPistolDeactivate");
            }
            else
            {
                gunAnimator.Play("SmartPistolActivate");
            }
            yield return null;
        }

        IEnumerator doEquip()
        {
            gunAnimator.Play("RevolverEquip");
            if (mode == SmartPistolModes.Smart)
            {
                yield return new WaitForSeconds(0.100f);
                gunAnimator.Play("SmartPistolActivate");
            }
        }

        public override void OnReloadPressed()
        {
        }
        
        public override void ActivateWeapon()
        {
            activated = true;
            modelBase.SetActive(true);
            modelBase1.SetActive(true);
            modelBase2.SetActive(true);
            ui.gameObject.SetActive(true);
            StartCoroutine(doEquip());
        }

        private void Update()
        {
            if (retriggerRevolver && revolverReady)
            {
                retriggerRevolver = false;
                if (mode == SmartPistolModes.Revolver && activated)
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
            activated = false;
            modelBase.SetActive(false);
            modelBase1.SetActive(false);
            modelBase2.SetActive(false);
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
                    gunner.HasUpgrade("PistolSmart")) || 
                (newmode == SmartPistolModes.Revolver)
            ) {
                StartCoroutine(switchModes(newmode));
            }
        }

        public override bool CanActivate()
        {
            return gunner.HasUpgrade("PistolBasic");
        }

    }
}