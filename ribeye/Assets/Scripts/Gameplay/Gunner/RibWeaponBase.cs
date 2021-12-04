using UnityEngine; 

namespace Gameplay.Gunner
{
    public abstract class RibWeaponBase : MonoBehaviour
    {
        public int activationIndex = 0;
        public abstract void ActivateWeapon();
        
        public abstract void DeactivateWeapon();

        public int GetWeaponActivationIndex()
        {
            return activationIndex;
        }
    }
}