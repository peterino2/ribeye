using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Game;
using Gameplay.Core;
using Gameplay.Stats;
using UnityEngine;

namespace Gameplay.Gunner
{
    public class RibWeaponBlade : RibWeaponBase
    {
        public float damage = 5;
        private Collider hurtbox;
        [SerializeField] private GameObject impactEffect;
        private PeterFPSCharacterController _character;
        private void Awake()
        {
            activationIndex = 1;
            _character = FindObjectOfType<PeterFPSCharacterController>();
        }

        public String[] SlashAnims;

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
                    List<EntityBase> shouldRemove = new List<EntityBase>();
                    bool hit = false;
                    int max_hits = 3;
                    foreach (var target in targets)
                    {
                        if (target != null && max_hits > 0 && target.alive)
                        {
                            var h = target.GetComponent<RibHumanoidEnemy>();
                            if (h != null)
                            {
                                h.TakeSwordDamage(damage);
                            }
                            else
                            {
                                target.TakeDamage(damage);
                            }

                            Physics.Raycast(
                                gunner.transform.position,
                                target.transform.position - gunner.transform.position,
                                out RaycastHit r, Mathf.Infinity, ~playermask);
                            Instantiate(impactEffect, r.point, Quaternion.LookRotation(r.normal));
                            hit = true;
                            max_hits = -1;
                        }
                        else {
                            shouldRemove.Append(target);
                        }

                        if (!target.alive)
                        {
                            shouldRemove.Append(target);
                        }
                    }

                    foreach (var t in shouldRemove)
                    {
                        targets.Remove(t);
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

        private bool retriggerSlash = false;

        private void HandleSwingCooldown()
        {
            if (swingCooldown > 0)
            {
                swingCooldown -= Time.deltaTime;
                if (swingCooldown < 0)
                {
                    swingReady = true;
                    if (retriggerSlash)
                    {
                        DoSlash();
                        retriggerSlash = false;
                    }
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
            HandleSlashReset();
            if (modelresetTimeout > 0)
            {
                modelresetTimeout -= Time.deltaTime;
                if (modelresetTimeout <= 0)
                {
                    model.SetActive(true);
                }
            }

            if (swingRecoilResetTime > 0)
            {
                swingRecoilResetTime -= Time.deltaTime;

                if (swingRecoilResetTime <= 0)
                {
                    gunner.rotationFactor = 0.8f;
                    gunner.rotationTarget = Quaternion.identity;
                }
            }
        }


        private void HandleSlashReset()
        {
            if (slashResetTime > 0)
            {
                slashResetTime -= Time.deltaTime;
                if (slashResetTime <= 0)
                {
                    slashIndex = 0;
                }
            }
        }

        public float hookRange;
        public GameObject hookPrefab;

        private float modelresetTimeout = 0;
        public Transform hookStartTransform;
        void TossHook()
        {
            print("TossingHook");
            model.SetActive(false);
            modelresetTimeout = 0.2f;
            
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit r, hookRange, ~gunner.playermask))
            {
                Instantiate(hookPrefab, r.point, Quaternion.LookRotation(r.normal));
                _character.HookToTarget(r.point, r.point);
                gunAnimator.Play("HookPull");
                modelresetTimeout = 0.12f;
            }
        }

        private bool hooked;
        private int slashIndex = 0;
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
                DoSlash();
            }

            if (!swingReady && !hooked)
            {
                if (swingCooldown < 0.3f)
                {
                    retriggerSlash = true;
                }
            }
        }

        [SerializeField] 
        private Vector3[] slashRotations_v3 = { };

        private float swingRecoilResetTime = 0.1f;

        private void DoSlash()
        {
            swingReady = false;
            gunAnimator.Play(SlashAnims[slashIndex]);
            
            gunner.rotationFactor = 0.8f;
            gunner.rotationTarget = Quaternion.Euler(slashRotations_v3[slashIndex]);
            incrementSlash();
            swingDamageDelay = 0.1f;
            swingCooldown = 0.3f;
            swingRecoilResetTime = 0.100f;
        }

        private float slashResetTime = 0f;

        private void incrementSlash()
        {
            slashIndex = slashIndex + 1 < SlashAnims.Length ? slashIndex + 1 : 0;
            slashResetTime = 1.0f;
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