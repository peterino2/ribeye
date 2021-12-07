using System;
using UnityEngine;

namespace Gameplay.Gunner
{
    public class RibWeaponUpgrade : MonoBehaviour
    {
        public string[] upgrades;
        public string name;

        private void OnTriggerEnter(Collider other)
        {
            PeterFPSCharacterController go = other.gameObject.GetComponent<PeterFPSCharacterController>();
            if (go)
            {
                foreach (var upgrade in upgrades)
                {
                    go.gunner.GiveUpgrade(upgrade);
                }
                Destroy(gameObject);
            }
        }
    }
}