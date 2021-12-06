using System;
using UnityEngine;

namespace Gameplay.Gunner
{
    public class RibWeaponBlade : RibWeaponBase
    {
        private void Awake()
        {
            activationIndex = 1;
        }

        [SerializeField] private GameObject model;
        public override void ActivateWeapon()
        {
            model.SetActive(true);
            gunAnimator.Play("BladeEquip");
        }

        public override void DeactivateWeapon()
        {
            model.SetActive(false);
        }

        public override void OnAltFire()
        {
            gunAnimator.Play("Armature|HookToss");
        }

        public override void OnFire()
        {
            gunAnimator.Play("BladeSwingSeq1");
        }
        
        public override void OnReloadPressed()
        {
        }

        public override void DeactivateWeaponNoAnim()
        {
        }

        public override string GetWeaponName()
        {
            return "Blade and tackle";
        }
    }
}