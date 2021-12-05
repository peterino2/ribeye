﻿using Gameplay.Stats;
using UnityEngine;

namespace Gameplay.Core
{
    public class RibEnemyBase : EntityBase
    {
        public override void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0);

            if (health == 0)
            {
                Destroy(gameObject);
            }
        }

        public override void Heal(float damage)
        {
            health += damage;
        }
    }
}