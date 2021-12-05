using UnityEngine; 

namespace Gameplay.Gunner
{
    public abstract class RibWeaponBase : MonoBehaviour
    {
        public int activationIndex = 0;
        [SerializeField] private float sprayDeg = 25; // see soundmanager prefab this needs to match
        public abstract void ActivateWeapon();
        
        public abstract void DeactivateWeapon();
        public abstract void OnFire();
        
        public abstract string GetWeaponName();
        
        public abstract void OnAltFire();
        public int GetWeaponActivationIndex()
        {
            return activationIndex;
        }
    }
}