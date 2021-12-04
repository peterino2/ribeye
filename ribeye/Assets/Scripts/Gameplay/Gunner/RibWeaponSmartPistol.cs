using System;
using UnityEngine;

namespace Gameplay.Gunner
{
    public class RibWeaponSmartPistol : RibWeaponBase
    {
        [SerializeField] private int soundIndex = 0; // see soundmanager prefab this needs to match
        [SerializeField]
        private bool active = false;
        private void Start()
        {
            active = false;
        }

        private void Update()
        {
        }

        public override void ActivateWeapon()
        {
        }

        public override void DeactivateWeapon()
        {
        }
        
    }
}