using System;
using Gameplay.Stats;
using UnityEngine;

namespace Gameplay.Core
{
    public class RibEnemyBase : EntityBase
    {
        private void Start()
        {
            team = 1;
        }

        public override void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0);

            GameManager.playHitSound(transform.position);
            if (health <= 0.01f)
            {
                GameManager.playHeavySound(transform.position);
                Destroy(gameObject);
            }
        }

        public override void Heal(float damage)
        {
            health += damage;
        }
    }
}