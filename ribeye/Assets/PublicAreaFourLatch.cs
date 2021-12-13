using Gameplay.Core;
using UnityEngine;

namespace DefaultNamespace
{
    public class PublicAreaFourLatch : RibInteractable
    {
        static PublicAreaFourLatch latch;

        public RibInteractable door;
        private int lockCount = 4;

        public override void Activate(PeterFPSCharacterController controller)
        {
            lockCount -= 1;
            
            if (lockCount == 0)
            {
                door.Activate(controller);
            }
            else
            {
                PublicAreaDirector.director.NextWave();
            }
        }
    }
}