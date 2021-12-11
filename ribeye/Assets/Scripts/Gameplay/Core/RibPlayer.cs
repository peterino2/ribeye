using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gameplay.Stats
{
    public class RibPlayer : EntityBase
    {

        [SerializeField] public float maxHp = 100f;

        private PeterFPSCharacterController _controller;
        private void Start()
        {
            _controller = FindObjectOfType<PeterFPSCharacterController>();
        }
        
        private bool filmGrainAnimating = false;

        IEnumerator filmGrainExplosion()
        {
            float t = 0;
            vhs.profile.TryGet(out VHSPro vhsInner);
            while (t < 0.7f)
            {
                t += Time.deltaTime;
                var f = filmGrainCurve.Evaluate(t);

                vhsInner.filmGrainAmount.value = f;
                vhsInner.bleedAmount.value = f*5;
                vhsInner.signalNoiseAmount.value = f;
                yield return null;
            }
        }
        public void  DoFilmGrainExplosion()
        {
            filmGrainAnimating = false;
            StopCoroutine(filmGrainExplosion());
            StartCoroutine(filmGrainExplosion());
        }
        
        public AnimationCurve filmGrainCurve;
        public Volume vhs;

        public override void TakeDamage(float damage)
        {
            Mathf.Max(health - damage, 0);

            if (health == 0)
            {
                _controller.enabled = false;
            }
        }

        public override void Heal(float damage)
        {
            health += damage;
            Mathf.Min(maxHp, health);
        }
    }
}