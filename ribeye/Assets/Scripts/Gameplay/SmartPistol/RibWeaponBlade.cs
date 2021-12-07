using System;
using System.Collections.Generic;
using Game;
using Gameplay.Stats;
using UnityEngine;

namespace Gameplay.Gunner
{
    public class RibWeaponBlade : RibWeaponBase
    {
        public float damage = 5;
        private Collider hurtbox;
        private bool activated = false;
        [SerializeField] private GameObject impactEffect;
        private void Awake()
        {
            activationIndex = 1;
        }

        private void Start()
        {
            hurtbox = GetComponent<Collider>();
        }

        [SerializeField] private GameObject model;
        public override void ActivateWeapon()
        {
            activated = true;
            model.SetActive(true);
            gunAnimator.Play("BladeEquip");
        }

        public override void DeactivateWeapon()
        {
            model.SetActive(false);
        }

        public override void OnAltFire()
        {
            gunAnimator.Play("Armature|HookToss");
        }

        [SerializeField]
        private List<EntityBase> targets = new List<EntityBase>();

        public LayerMask playermask;

        private void OnTriggerEnter(Collider other)
        {
            print("nigga");
            if (other.gameObject.GetComponent<EntityBase>()
                && ((( 1 << other.gameObject.layer) | playermask.value) > 0)
            )
            {
                targets.Add(other.gameObject.GetComponent<EntityBase>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var e = other.GetComponent<EntityBase>();
            if (e == null)
            {
                targets.Remove(e);
            }
            if (targets.Contains(e))
            {
                targets.Remove(e);
            }
        }

        private float swingDamageDelay = 0;
        private void Update()
        {
            if(swingDamageDelay > 0)
            {
                swingDamageDelay -= Time.deltaTime;
                if (swingDamageDelay < 0)
                {
                    foreach (var target in targets)
                    {
                        if (target != null)
                        {
                            target.TakeDamage(damage);
                            Physics.Raycast(
                                gunner.transform.position,
                                target.transform.position - gunner.transform.position,
                                out RaycastHit r, Mathf.Infinity, ~playermask);
                            
                            Instantiate(impactEffect, r.point, Quaternion.LookRotation(r.normal));
                        }
                    }
                }
            }
            

            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
                if (cooldown < 0)
                {
                    swingReady = true;
                }
            }
        }

        private bool swingReady = true;
        private float cooldown = 0f;
        public override void OnFire()
        {
            if (swingReady)
            {
                swingReady = false;
                gunAnimator.Play("BladeSwingSeq1");
                swingDamageDelay = 0.1f;
                cooldown = 0.3f;
            }
        }
        
        public override void OnReloadPressed()
        {
        }

        public override void DeactivateWeaponNoAnim()
        {
            activated = false;
            model.SetActive(false);
        }
        
        public override bool CanActivate()
        {
            return gunner.upgrades.Contains("Sword");
        }

        public override string GetWeaponName()
        {
            return "Blade and tackle";
        }
        
        
    }
}