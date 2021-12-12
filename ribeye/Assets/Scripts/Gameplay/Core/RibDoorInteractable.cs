using UnityEngine;

namespace Gameplay.Core
{
    public class RibDoorInteractable : RibInteractable
    {
        [SerializeField]
        private Lever lever; 
        void Awake()
        {
            lever = GetComponent<Lever>();
        }
        public override void Activate(PeterFPSCharacterController controller)
        {
            print("Activating door");
            GameManager._soundManager.PlaySound(26, transform.position);
            lever.OpenDoor();
            this.enabled = false;
        }
    }
}