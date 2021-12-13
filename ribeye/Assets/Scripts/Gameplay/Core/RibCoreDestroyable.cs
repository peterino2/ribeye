using Gameplay.Stats;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.Core
{
    public class RibCoreDestroyable : EntityBase
    {
        [SerializeField] private GameObject deathExplosionParticle;
        [SerializeField] private Transform deathExplosionCenterPoint;
        public override void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0);

            GameManager.playHitSound(transform.position);
            if (health <= 0.01f)
            {
                if (owner) {
                    owner.EnemyKilled(this);
                }
                GameManager.playHeavySound(transform.position);
                var x = Instantiate(deathExplosionParticle, deathExplosionCenterPoint.position, Quaternion.identity);
                Destroy(x, 2f);
                Destroy(gameObject);
                SceneManager.LoadScene("Thanks For playing");
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
