using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.PeterFPS;
using UnityEngine;
using UnityEngine.Rendering;

namespace Gameplay.Stats
{
    public class RibPlayer : EntityBase
    {

        [SerializeField] public float maxHp = 10f;

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
        public HurtIndicatorUi hurtUi;

        public override void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0);

            if (health == 0)
            {
                _controller.enabled = false;
            }
        }

        public void TakeDamageFromSource(float damage, GameObject source)
        {
            TakeDamage(damage);
            print(String.Format("eating damage: new hp: {0}", health));
            hurtUi.ShowHurtDirection(source.transform.position);
        }

        public override void Heal(float damage)
        {
            health += damage;
            Mathf.Min(maxHp, health);
        }
    }
}