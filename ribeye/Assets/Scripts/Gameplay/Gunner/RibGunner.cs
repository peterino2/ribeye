using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Gunner;
using UnityEngine;

public class RibGunner : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private RibWeaponBase[] gunrack;

    [SerializeField]
    public RibWeaponBase[] guns;

    public RibWeaponBase activeGun;
    public LayerMask playermask;

    public ParticleSystem speedLines;
    [SerializeField] private Animator gunAnimator;
    
    private HashSet<string> upgrades = new HashSet<string>();

    public string[] initialUpgrades;

    public bool HasUpgrade(string check)
    {
        return upgrades.Contains(check.ToLower());
    }

    public HashSet<string> GetUpgrades()
    {
        return upgrades;
    }
    
    public int gunIndex;
    private void Awake()
    {
        upgrades = new HashSet<string>();
        upgrades.Clear();
        
        gunrack = GetComponentsInChildren<RibWeaponBase>();
        Array.Resize(ref guns, 9);
        foreach (var gun in gunrack)
        {
            guns[gun.GetWeaponActivationIndex()] = gun;
            gun.gunAnimator = gunAnimator;
            gun.gunner = this;
            gun.DeactivateWeaponNoAnim();
        }
    }

    public void GiveUpgradeInner(string upgrade)
    {
        upgrades.Add(upgrade.ToLower());
    }

    public void GiveUpgrade(string upgrade)
    {
        print("upgrade granted +++" + upgrade);
        GiveUpgradeInner(upgrade);
        if (gunIndex == -1)
        {
            // automatically equip the new weapon if it's smart pistol or sword
            if (upgrade == "PistolBasic")
            {
                gunAnimator.gameObject.SetActive(true);
                TryActivateGun(0); // smart pistol index
            }
            if (upgrade == "Sword")
            {
                gunAnimator.gameObject.SetActive(true);
                TryActivateGun(1); // sword index
            }
        }
    }

    public int WeaponStartIndex = -1;

    void Start()
    {
        gunIndex = WeaponStartIndex;
        foreach(var upgrade in initialUpgrades)
        {
            GiveUpgradeInner(upgrade);
        }

        gunAnimator.Play("StartupSequence");
        if (gunIndex != -1)
        {
            TryActivateGun(gunIndex);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryActivateGun(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryActivateGun(1);
        // if (Input.GetKeyDown(KeyCode.Alpha3)) TryActivateGun(2);
        
        
        if (gunIndex == -1)
        {
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            activeGun.OnFire();
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            activeGun.OnAltFire();
        }
    }

    void TryActivateGun(int newGunIndex)
    {
        if (newGunIndex == -1)
        {
            gunAnimator.Play("DefaultIdle");
                gunIndex = newGunIndex;
            return;
        }
        print(String.Format("{0} -> {1}", gunIndex, newGunIndex));
        
        gunAnimator.gameObject.SetActive(true);
        
        if (gunIndex == newGunIndex) return;
        
        if (guns[newGunIndex].CanActivate())
        {
            ActivateGun(newGunIndex);
        }
    }


    void ActivateGun(int newGunIndex) // do not call directly only call from tryactivate gun
    {
        print(String.Format("Trying activation"));
        if(activeGun) activeGun.DeactivateWeapon();
        
        if (guns[newGunIndex])
        {
            print("doing activation");
            gunIndex = newGunIndex;
            guns[newGunIndex].ActivateWeapon();
            activeGun = guns[newGunIndex];
            gunIndex = newGunIndex;
        }
    }
}

