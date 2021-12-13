using Gameplay.Core;
using UnityEngine;

namespace Gameplay.Stats
{
    public abstract class EntityBase : MonoBehaviour
    {
        [SerializeField] public float health = 3f;
        [SerializeField] public float team = 0;
        [SerializeField] public EventDirector owner;

        public bool alive = true;
        
        public abstract void TakeDamage(float damage);
        public abstract void Heal(float damage);
    }
}