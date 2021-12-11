using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.Gunner
{
    public class RibWeaponPlasmaCaster : RibWeaponBase
    {
        
        public Vector3 FireRotation;
        public GameObject model;
        public GameObject plasmaBullet;
        public Animator modelAnimator;
        
        private void Awake()
        {
            activationIndex = 2;
        }


        private float equipTime = 0f;
        public override void ActivateWeapon()
        {
            activated = true;
            gunner.inventoryUi.ammoText.enabled = true;
            modelAnimator.Play("LauncherEquip");
            model.SetActive(true);
            gunner.ui.ShowPlasmaCaster();
        }

        public override void DeactivateWeapon()
        {
            gunner.rotationFactor = 0.8f;
            gunner.rotationTarget = Quaternion.Euler(FireRotation);
            model.SetActive(false);
            activated = false;
        }

        public override void DeactivateWeaponNoAnim()
        {
            DeactivateWeapon();
        }

        private bool fireReady = true;
        public float fireDelay = 1.5f;
        private float fireTime = 0f;
        public Transform muzzle;
        public float damage;

        public int Ammo = 3;
        
        public override void OnFire()
        {
            if (fireReady && Ammo >0)
            {
                Ammo -= 1;
                fireTime = fireDelay;
                var b = Instantiate(plasmaBullet, muzzle.position, Quaternion.Euler(muzzle.forward));
                var p = b.GetComponentInChildren<RibPlasmaProjectile>();
                p.SetVelocityDir(gunner.transform.forward);
                modelAnimator.Play("LauncherFire",-1,0);
                p.damage = damage;
                fireReady = false;
                p.team = 0;
                Destroy(b, 10f);
            }
        }

        private void Update()
        {
            if (fireTime > 0)
            {
                fireTime -= Time.deltaTime;
                if (fireTime < 0)
                {
                    fireReady = true;
                }
            }

            if (equipTime > 0)
            {
                equipTime -= Time.deltaTime;
                if (equipTime <= 0)
                {
                    gunner.rotationTarget = Quaternion.identity;
                }

            }
            
            if (activated)
            {
                gunner.inventoryUi.ammoText.text = String.Format("AMMO: {0}", Ammo);
            }
        }

        public override bool CanActivate()
        {
            return gunner.HasUpgrade("plasmacaster");
        }

        public override void OnReloadPressed()
        {
            
        }

        public override void GrantAmmo(int ammo)
        {
            ammo += 1;
        }

        public override string GetWeaponName()
        {
            return "The Deletus";
        }

        public override void OnAltFire()
        {
            
        }
    }
}