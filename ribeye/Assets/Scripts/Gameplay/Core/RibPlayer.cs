using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.PeterFPS;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

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
        
        IEnumerator filmGrainDeath()
        {
            float t = 0;
            vhs.profile.TryGet(out VHSPro vhsInner);
            while (t < 20f)
            {
                t += Time.deltaTime;
                var f = deathFilmCurve.Evaluate(t);

                vhsInner.filmGrainAmount.value = f;
                vhsInner.bleedAmount.value = f*5;
                vhsInner.signalNoiseAmount.value = f;
                if (t > 2f)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                yield return null;
            }
        }

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

        private bool dead = false;
        public void  DoFilmGrainDeath()
        {
            filmGrainAnimating = false;
            if (!dead)
            {
                dead = true;
                StartCoroutine(filmGrainDeath());
            }
        }
        
        public AnimationCurve deathFilmCurve;
        public AnimationCurve filmGrainCurve;
        public Volume vhs;
        public HurtIndicatorUi hurtUi;

        public override void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0);

            if (health == 0)
            {
                _controller.Die();
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