using Gameplay.Stats;
using UnityEngine;

namespace Gameplay.Core
{
    public abstract class EventDirector : MonoBehaviour
    {
        public abstract void EnemyKilled(EntityBase entity);
    }
}