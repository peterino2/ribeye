using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.Gunner
{
    public class RibWeaponPlasmaCaster : RibWeaponBase
    {
        public Vector3 FireRotation;

        public GameObject model;
        private void Awake()
        {
            activationIndex = 2;
        }

        public override void ActivateWeapon()
        {
            gunner.rotationFactor = 0.8f;
            model.SetActive(true);
            
        }

        IEnumerator ActiveWeapon()
        {
            yield return null;
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

        public override void OnFire()
        {
            gunner.rotationFactor = 0.8f;
            gunner.rotationTarget = Quaternion.Euler(FireRotation);
            model.SetActive(true);
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