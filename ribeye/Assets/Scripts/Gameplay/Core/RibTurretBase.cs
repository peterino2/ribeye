using Gameplay.Stats;
using UnityEngine;

namespace Gameplay.Core
{
    public class RibTurretBase : EntityBase
    {
        [SerializeField] private GameObject deathExplosionParticle;
        [SerializeField] private Transform deathExplosionCenterPoint;
        public override void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0);

            GameManager.playHitSound(transform.position);
            if (health <= 0.01f)
            {
                GameManager.playHeavySound(transform.position);
                var x = Instantiate(deathExplosionParticle, deathExplosionCenterPoint.position, Quaternion.identity);
                Destroy(x, 2f);
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            team = 1;
        }

        public override void Heal(float damage)
        {
            health += damage;
        }
    }
}