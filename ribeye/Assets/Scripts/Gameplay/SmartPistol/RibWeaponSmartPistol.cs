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
        [SerializeField] private float damageRevolver = 2.5f;


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
        }

        private BezierCurveTracer bcurveGen;
        public GameObject RevolverShotPrefab;
        private void Start()
        {
            //StartCoroutine(switchModes(mode));
            mode = SmartPistolModes.Revolver;
            gunner.ui.SetSmartPistolMode(mode);
            gunner.ui.range = range;
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
            print("firing inner");
            var target = gunner.ui.GetNearestTarget();
            if (target != null)
            {
                smartAmmo -= 1;
                print("firing innerinner");
                var obj = target.gameObject.GetComponent<RibTargetable>();
                Physics.Raycast(transform.position, target.transform.position - transform.position, out RaycastHit r,
                    Mathf.Infinity, curveMask);
                var wrather = r.transform.gameObject.GetComponent<RibHumanoidEnemy>();
                if (wrather)
                {
                    wrather.TakeHeadShotDamage(damageSmart);
                }
                else
                {
                    target.TakeDamage(damageSmart);
                }
                gunner.ui.Hitmarker();
                bcurveGen.ShowTracer(muzzle.transform, r.point, bulletImpact);
                Instantiate(bulletImpact, r.point, Quaternion.LookRotation(r.normal));
                GameManager._soundManager.PlaySound(0, transform.position, volume:0.2f);
                gunAnimator.Play("SmartPistolShoot");
                // StartCoroutine(playFireAnim());
            } 
            fireready = true;
        }

        IEnumerator FullAutoSmart()
        {
            while (Input.GetKey(KeyCode.Mouse0) && fireready && (smartAmmo > 0))
            {
                print("smartFiring");
                DoSmartFire();
                yield return new WaitForSeconds(1 / FireRate);
            }

            if (smartAmmo <= 0)
            {
                StartCoroutine(switchModes(SmartPistolModes.Revolver));
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
                GameManager._soundManager.PlaySound(revolverShotIndex, transform.position, volume:0.6f);
                Vector3[] Positions = {muzzle.transform.position, muzzle.transform.position + transform.forward * 3200f};
                
                if (gunner.ui.GetCenterTarget(out Transform objectHit, out RaycastHit rayhit))
                {
                    Positions[1] = rayhit.point;
                    
                    EntityBase x = objectHit.gameObject.GetComponent<EntityBase>();

                    var wrather = objectHit.gameObject.GetComponent<RibHumanoidEnemy>();
                    if (wrather)
                    {
                        if (((wrather.head.transform.position + wrather.head.transform.up * 0.06f) - rayhit.point).magnitude < 0.55f)
                        {
                            wrather.TakeHeadShotDamage(damageRevolver);
                            smartAmmo = smartAmmo + 5 > smartAmmoMax? smartAmmoMax: smartAmmo + 5;
                            gunner.ui.Hitmarker();
                        }
                        else
                        {
                            smartAmmo = smartAmmo + 1 > smartAmmoMax? smartAmmoMax: smartAmmo + 1;
                            x.TakeDamage(damageRevolver);
                            gunner.ui.Hitmarker();
                        }
                    }
                    else if (x != null)
                    {
                        x.TakeDamage(damageRevolver);
                        smartAmmo = smartAmmo + 1 > smartAmmoMax? smartAmmoMax: smartAmmo + 1;
                        gunner.ui.Hitmarker();
                    }
                
                    Instantiate(bulletImpact, rayhit.point, Quaternion.LookRotation(rayhit.normal));
                }
                var line = Instantiate(RevolverShotPrefab, transform.position, Quaternion.identity);
                var linec = line.GetComponent<LineRenderer>();
                line.GetComponent<CurvedLine>().Fade(true);
                linec.SetPositions(Positions);
                
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

            if (newMode == SmartPistolModes.Revolver)
            {
                mode = newMode;
                gunAnimator.Play("SmartPistolDeactivate");
                gunner.inventoryUi.ammoText.text = "";
            }
            else
            {
                if (smartAmmo > 0)
                {
                    print("Switching");
                    mode = newMode;
                    gunAnimator.Play("SmartPistolActivate");
                }
            }
            gunner.ui.SetSmartPistolMode(mode);
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
            gunner.ui.SetSmartPistolMode(mode);
            StartCoroutine(doEquip());
        }

        public int smartAmmoMax = 30;
        public int smartAmmo = 30;
        public float smartAmmoRegenTime = 2f; // gain one ammo every 2 seconds
        private float saRegenTime;

        public override void GrantAmmo(int ammo)
        {
            smartAmmo += ammo;
            if (smartAmmo > smartAmmoMax) smartAmmo = smartAmmo;
        }

        private void Update()
        {
            if (saRegenTime>0)
            {
                saRegenTime -= Time.deltaTime;
                if (saRegenTime <= 0)
                {
                    smartAmmo += smartAmmo + 1 > smartAmmoMax ? smartAmmoMax: smartAmmo + 1;
                    saRegenTime = smartAmmoRegenTime;
                }
            }
            if (retriggerRevolver && revolverReady)
            {
                retriggerRevolver = false;
                if (mode == SmartPistolModes.Revolver && activated)
                {
                    StartCoroutine(RevolverHeavy());
                }
            }

            if (activated)
            {
                if (gunner.HasUpgrade("PistolSmart"))
                {
                    gunner.inventoryUi.ammoText.text = String.Format("SMART AMMO: {0}/{1}", smartAmmo, smartAmmoMax);
                }
                else
                {
                    gunner.inventoryUi.ammoText.text = String.Format("");
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
            // ui.gameObject.SetActive(false);
            mode = SmartPistolModes.Revolver;
            gunner.ui.SetSmartPistolMode(mode);
        }

        public override string GetWeaponName()
        {
            if (gunner.HasUpgrade("pistolSmart"))
            {
                return "SMART Pistol";
            }
            else
            {
                return "Hand Cannon";
            }
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