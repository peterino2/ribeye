using System;
using System.Collections;
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
        private PeterFPSCharacterController _character;
        private void Awake()
        {
            activationIndex = 1;
            _character = FindObjectOfType<PeterFPSCharacterController>();
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

        private GameObject hookTarget;
        private bool hookready = true;
        public override void OnAltFire()
        {
            if (hooked)
            {
                // yang yourself to the hooked target
                hooked = false;
                print("Yanking target to you");
                _character.HookTargetToMe(hookTarget);
            }
            
            if (!hooked && hookready)
            {
                gunAnimator.Play("HookToss");
                hookTossTime = 0.22f;
            }
        }

        [SerializeField]
        private List<EntityBase> targets = new List<EntityBase>();

        public LayerMask playermask;

        private void OnTriggerEnter(Collider other)
        {
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
        private void HandleSwingDamage()
        {
            if(swingDamageDelay > 0)
            {
                swingDamageDelay -= Time.deltaTime;
                if (swingDamageDelay <= 0)
                {
                    bool hit = false;
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
                            hit = true;
                        }
                    }

                    if (hit)
                    {
                        
                    }
                }
            }
        }
        
        private float hookTossTime = 0;

        private void HandleHookToss()
        {
            if (hookTossTime > 0)
            {
                hookTossTime -= Time.deltaTime;
                if (hookTossTime <= 0)
                {
                    TossHook();
                }
            }
        }

        private void HandleHooking()
        {
            if (hookingTime > 0)
            {
                hookingTime -= Time.deltaTime;
            }
        }

        private void HandleSwingCooldown()
        {
            if (swingCooldown > 0)
            {
                swingCooldown -= Time.deltaTime;
                if (swingCooldown < 0)
                {
                    swingReady = true;
                }
            }
        }
        
        private float hookingTime = 0;
        private void Update()
        {
            HandleSwingDamage();
            HandleHookToss();
            HandleHooking();
            HandleSwingDamage();
            HandleSwingCooldown();
        }

        public float hookRange;
        public GameObject hookPrefab;
        public Transform hookStartTransform;
        void TossHook()
        {
            print("TossingHook");
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit r, hookRange, ~gunner.playermask))
            {
                Instantiate(hookPrefab, r.point, Quaternion.LookRotation(r.normal));
                _character.HookToTarget(r.point, r.point);
            }
        }

        private bool hooked;

        private bool swingReady = true;
        private float swingCooldown = 0f;
        public override void OnFire()
        {
            if (hooked)
            {
                // yang yourself to the hooked target
                hooked = false;
                print("Yanking to target");
                var v = hookTarget.transform.position;
                _character.HookToTarget(v, v);
            }
            
            if (swingReady && !hooked)
            {
                swingReady = false;
                gunAnimator.Play("BladeSlash_1");
                swingDamageDelay = 0.1f;
                swingCooldown = 0.3f;
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
            return gunner.HasUpgrade("Sword");
        }

        public override string GetWeaponName()
        {
            return "Blade and tackle";
        }
        
        
    }
}