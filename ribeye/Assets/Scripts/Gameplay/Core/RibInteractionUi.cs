using System;
using TMPro;
using UnityEngine;

namespace Gameplay.Core
{
    public class RibInteractionUi : MonoBehaviour
    {
        [SerializeField] private PeterFPSCharacterController _controller;

        private TextMeshProUGUI _text;

        private void Awake()
        {
        }

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _controller = FindObjectOfType<PeterFPSCharacterController>();
        }


        private void Update()
        {
            if (_controller._interactable)
            {
                _text.enabled = true;
                _text.text = "Press 'F' to " + _controller._interactable.interactionText;
            }
            else
            {
                _text.enabled = false;
            }
        }
    }
}