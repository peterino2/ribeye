using UnityEngine;

namespace Gameplay.Core
{
    public class RibForceField : RibInteractable
    {
        public override void Activate(PeterFPSCharacterController controller)
        {
            Destroy(gameObject);
        }
    }
}