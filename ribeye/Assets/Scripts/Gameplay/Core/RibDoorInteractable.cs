﻿using UnityEngine;

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
            lever.OpenDoor();
            this.enabled = false;
        }
    }
}