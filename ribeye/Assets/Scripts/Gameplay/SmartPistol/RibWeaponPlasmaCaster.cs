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
        
        private void Awake()
        {
            activationIndex = 2;
        }

        public override void ActivateWeapon()
        {
            gunner.rotationFactor = 0.8f;
            gunner.rotationTarget = Quaternion.Euler(FireRotation);
            model.SetActive(true);
        }

        public override void DeactivateWeapon()
        {
            gunner.rotationFactor = 0.8f;
            gunner.rotationTarget = Quaternion.Euler(FireRotation);
            model.SetActive(false);
        }

        public override void DeactivateWeaponNoAnim()
        {
            DeactivateWeapon();
        }

        private bool fireReady = true;
        private float fireTime = 0f;
        public Transform muzzle;
        public float damage;
        
        public override void OnFire()
        {
            if (fireReady)
            {
                if (Physics.Raycast(transform.position, transform.forward, out RaycastHit r, Mathf.Infinity, ~gunner.playermask))
                {
                    fireTime = 0.9f;
                    var b = Instantiate(plasmaBullet, muzzle.position, Quaternion.Euler(muzzle.forward));
                    var p = b.GetComponentInChildren<RibPlasmaProjectile>();
                    p.SetVelocityDir(gunner.transform.forward);
                    p.damage = damage;
                    fireReady = false;
                    p.team = 0;
                    Destroy(b, 10f);
                }
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
        }

        public override bool CanActivate()
        {
            return gunner.HasUpgrade("plasmacaster");
        }

        public override void OnReloadPressed()
        {
            
        }

        public override string GetWeaponName()
        {
            return "Plasma Caster";
        }

        public override void OnAltFire()
        {
            
        }
    }
}