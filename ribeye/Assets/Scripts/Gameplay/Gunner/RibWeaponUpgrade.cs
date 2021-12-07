using System;
using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Gunner
{
    public class RibWeaponUpgrade : RibInteractable
    {
        public string[] upgrades;
        public string name;

        private void Awake()
        {
            FindObjectOfType<RibGunnerUi>();
        }

        public override void Activate(PeterFPSCharacterController controller)
        {
            foreach (var upgrade in upgrades)
            {
                controller.gunner.GiveUpgrade(upgrade);
            }
            Destroy(gameObject);
        }
    }
}