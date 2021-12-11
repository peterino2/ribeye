using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Stats;
using UnityEngine;

namespace DefaultNamespace
{
    public class RibPlasmaExplosionHurtbox : MonoBehaviour
    {
        [SerializeField]
        private List<EntityBase> targets = new List<EntityBase>();
        
        private void OnCollisionEnter(Collision other)
        {
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.GetComponent<EntityBase>())
            {
                targets.Add(other.gameObject.GetComponent<EntityBase>());
            }
        }

        public void Detonate(float damage, int team)
        {
            List<EntityBase> shouldRemove = new List<EntityBase>();
            bool hit = false;
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.TakeDamage(damage);
                }
                else {
                    shouldRemove.Append(target);
                }
            }

            foreach (var t in shouldRemove)
            {
                targets.Remove(t);
            }
        }
    }
}