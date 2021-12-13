using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Gameplay.Core
{
    public abstract class RibInteractable : MonoBehaviour
    {
        private RibInteractionUi ui;
        protected PeterFPSCharacterController _controller;

        public bool ready = true;
        
        public string interactionText = "Interact with me";

        private void Awake()
        {
            _controller = FindObjectOfType<PeterFPSCharacterController>();
            Assert.IsNotNull(_controller);
        }

        private void Start()
        {
            _controller = FindObjectOfType<PeterFPSCharacterController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ready)
            {
                var x = other.gameObject.GetComponent<PeterFPSCharacterController>();
                if (x)
                {
                    _controller.SetInteractable(this);
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<PeterFPSCharacterController>())
            {
                _controller.ClearInteractable(this);
            }
        }

        public abstract void Activate(PeterFPSCharacterController controller);
    }
}