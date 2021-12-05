using UnityEngine;

namespace Gameplay.Stats
{
    public abstract class EntityBase : MonoBehaviour
    {
        [SerializeField] public float health = 3f;
        
        public abstract void TakeDamage(float damage);
        public abstract void Heal(float damage);
    }
}