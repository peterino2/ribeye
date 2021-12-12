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
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<EntityBase>())
            {
                targets.Add(other.gameObject.GetComponent<EntityBase>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var e = other.gameObject.GetComponent<EntityBase>();
            if (e == null)
            {
                targets.Remove(e);
            }
            if (targets.Contains(e))
            {
                targets.Remove(e);
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
                    if (target.GetComponent<RibPlayer>())
                    {
                        var r =target.GetComponent<PeterFPSCharacterController>()._rigidbody;
                        r.AddExplosionForce(20f, transform.position, GetComponent<SphereCollider>().radius, 1.0f, ForceMode.Impulse);
                        target.GetComponent<RibPlayer>().DoFilmGrainExplosion();
                    }
                    else
                    {
                        var damageFactor = damage * (1 - (target.transform.position - transform.position).magnitude / GetComponent<SphereCollider>().radius);
                        print(damageFactor);
                        target.TakeDamage(damageFactor);
                        var rag = target.GetComponent<RagDoll>();
                        if (rag)
                        {
                            rag.rigidbodies[0].AddExplosionForce(200f, transform.position, GetComponent<SphereCollider>().radius, 1.0f, ForceMode.Impulse);
                        }
                    }
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