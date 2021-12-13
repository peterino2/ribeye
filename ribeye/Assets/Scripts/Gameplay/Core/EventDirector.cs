using System;
using System.Collections;
using Gameplay.Stats;
using UnityEngine;

namespace Gameplay.Core
{
    public abstract class EventDirector : MonoBehaviour
    {
        public abstract void EnemyKilled(EntityBase entity);

        public abstract void OnSlowTick(float dt);

        public bool ticking = true;

        IEnumerator DoSlowTick()
        {
            while (ticking)
            {
                OnSlowTick(1f);
                yield return new WaitForSeconds(1f);
            }
        }

        public void Init()
        {
            ticking = true;
            StartCoroutine(DoSlowTick());
        }
    }
}