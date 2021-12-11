using System;
using Gameplay.Stats;
using UnityEngine;

namespace Gameplay.Core
{
    public class RibHumanoidEnemy : EntityBase
    {
        public RagDoll ragDoll;

        [SerializeField]
        public GameObject head;
        
        [SerializeField]
        public GameObject bloodPrefab;
        
        public GameObject bloodsprayPrefab;
        public GameObject upperRagdollPrefab;
        public GameObject lowerRagdollPrefab;
        
        private void Start()
        {
            team = 1;
        }
        
        public void TakeSwordDamage(float damage)
        {
            TakeDamage(damage);
            if (health <= 0.01f)
            {
                head.transform.localScale = Vector3.zero;
                // Destroy(Instantiate(upperRagdollPrefab, transform.position, transform.rotation), 2.5f);
                // Destroy(Instantiate(lowerRagdollPrefab, transform.position, transform.rotation), 2.5f);
                Destroy(Instantiate(bloodsprayPrefab, transform.position, Quaternion.identity),  1.8f);
            }
        }

        public void TakeHeadShotDamage(float damage)
        {
            TakeDamage(damage * 4);
            if (health <= 0.01f)
            {
                head.transform.localScale = Vector3.zero;
                Destroy(Instantiate(bloodPrefab, head.transform.position, Quaternion.identity), 2.5f);
            }
        }

        private void Awake()
        {
            ragDoll = GetComponent<RagDoll>();
        }

        public override void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0);

            GameManager.playHitSound(transform.position);
            if (health <= 0.01f)
            {
                GameManager.playHeavySound(transform.position);
                ragDoll.EnableRagDoll();
                alive = false;
                Destroy(gameObject, 5f);
            }
        }

        public override void Heal(float damage)
        {
            health += damage;
        }
    }
}