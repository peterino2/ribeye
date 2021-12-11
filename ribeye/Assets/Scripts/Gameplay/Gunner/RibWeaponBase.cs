using UnityEngine; 

namespace Gameplay.Gunner
{
    public abstract class RibWeaponBase : MonoBehaviour
    {
        public int activationIndex = 0;
        private float sprayDeg = 25;
        protected bool activated;

        public RibGunner gunner;
        
        public Animator gunAnimator;
        public abstract void ActivateWeapon();
        
        public abstract void DeactivateWeapon();
        
        public abstract void DeactivateWeaponNoAnim();
        public abstract void OnFire();
        
        public abstract bool CanActivate();
        
        public abstract void OnReloadPressed();
        public abstract string GetWeaponName();
        
        public abstract void OnAltFire();
        public int GetWeaponActivationIndex()
        {
            return activationIndex;
        }
    }
}