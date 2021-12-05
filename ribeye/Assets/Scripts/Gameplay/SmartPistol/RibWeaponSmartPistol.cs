using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.Gunner
{
    public class RibWeaponSmartPistol : RibWeaponBase
    {
        public enum SmartPistolModes
        {
            Smart,
            Revolver
        }

        [SerializeField] private SmartPistolModes mode = SmartPistolModes.Smart;
        [SerializeField] private int gunshotIndex = 0; // see soundmanager prefab this needs to match
        [SerializeField] private float FireRate = 25; // see soundmanager prefab this needs to match
        [SerializeField] private GameObject AimerPrefab;
        [SerializeField] private string weaponName = "SmartPistol";
        
        [SerializeField]
        private bool active = false;

        [SerializeField] private GameObject model;
        [SerializeField] private GameObject modelBase;
        [SerializeField] private AnimationCurve smartPistolFire;
        [SerializeField] private AnimationCurve smartPistolBasePosition;
        private void Start()
        {
            if (!active)
            {
                modelBase.SetActive(false);
            }
            
        }

        private bool fireready = true;
        private void Update()
        {
        }
        
        public override void OnFire()
        {
            if (mode == SmartPistolModes.Smart)
            {
                StartCoroutine(FullAutoSmart());
            }
        }

        void DoFire()
        {
            fireready = false;
            GameManager._soundManager.PlaySound(0, transform.position, volume:0.3f);
            StartCoroutine(playFireAnim());
            fireready = true;
        }

        IEnumerator FullAutoSmart()
        {
            while (Input.GetKey(KeyCode.Mouse0) && fireready)
            {
                DoFire();
                yield return new WaitForSeconds(1/FireRate);
            }
        }
        IEnumerator switchModes(SmartPistolModes newMode)
        {
            yield return null;
        }

        IEnumerator playFireAnim()
        {
            float time = 0;
            while (time < smartPistolFire.length)
            {
                float yoffset = smartPistolFire.Evaluate(time);
                time += Time.deltaTime;
                model.transform.localRotation = Quaternion.Euler(0, 0, yoffset * 15f);
                yield return null;
            }
        } 

        public override void ActivateWeapon()
        {
            modelBase.SetActive(true);
        }

        public override void DeactivateWeapon()
        {
            modelBase.SetActive(false);
        }

        public override string GetWeaponName()
        {
            return weaponName;
        }

        public override void OnAltFire()
        {
            SmartPistolModes newmode =
                mode == SmartPistolModes.Smart ? SmartPistolModes.Revolver : SmartPistolModes.Smart;
            switchModes(newmode);
        }

    }
}