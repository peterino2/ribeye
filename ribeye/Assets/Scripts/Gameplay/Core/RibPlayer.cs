using System;
using UnityEditorInternal;
using UnityEngine;

namespace Gameplay.Stats
{
    public class RibPlayer : EntityBase
    {

        [SerializeField] public float maxHp = 100f;

        private PeterFPSCharacterController _controller;
        private void Start()
        {
            _controller = FindObjectOfType<PeterFPSCharacterController>();
        }

        public override void TakeDamage(float damage)
        {
            Mathf.Max(health - damage, 0);

            if (health == 0)
            {
                _controller.enabled = false;
            }
        }

        public override void Heal(float damage)
        {
            health += damage;
            Mathf.Min(maxHp, health);
        }
    }
}