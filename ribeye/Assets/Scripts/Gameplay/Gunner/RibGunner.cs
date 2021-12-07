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
    private RibWeaponBase[] guns;

    public RibWeaponBase activeGun;
    
    [SerializeField] private Animator gunAnimator;
    
    private int weaponIndexStart = -1;

    public HashSet<string> upgrades = new HashSet<string>();

    public string[] initialUpgrades;

    public int gunIndex;
    private void Awake()
    {
        upgrades = new HashSet<string>();
        upgrades.Clear();
        foreach(var upgrade in initialUpgrades)
        {
            GiveUpgrade(upgrade);
        }
        
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

    public void GiveUpgrade(string upgrade)
    {
        upgrades.Add(upgrade);
        print("upgrade granted +++" + upgrade);
        if (gunIndex == -1)
        {
            gunAnimator.gameObject.SetActive(true);
            // automatically equip the new weapon if it's smart pistol or sword
            if (upgrade == "PistolBasic") TryActivateGun(0); // smart pistol index
            if (upgrade == "Sword") TryActivateGun(1); // smart pistol index
        }
    }

    void Start()
    {
        gunIndex = weaponIndexStart;
        TryActivateGun(weaponIndexStart);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gunAnimator.Play("BladeSwingSeq2");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            gunAnimator.Play("BladeSwingSeq1");
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            print("hello");
            gunAnimator.Play("Armature|HookToss");
        }

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

        if (Input.GetKeyDown(KeyCode.Alpha1)) TryActivateGun(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryActivateGun(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryActivateGun(2);
    }

    void TryActivateGun(int newGunIndex)
    {
        if (newGunIndex == -1)
        {
            gunAnimator.gameObject.SetActive(false);
            gunIndex = newGunIndex;
            return;
        }
        
        if (gunIndex == newGunIndex) return;
        if (guns[newGunIndex].CanActivate())
        {
            ActivateGun(newGunIndex);
        }
    }


    void ActivateGun(int newGunIndex) // do not call directly only call from tryactivate gun
    {
        if(activeGun) activeGun.DeactivateWeapon();
        if (guns[newGunIndex])
        {
            gunIndex = newGunIndex;
            guns[newGunIndex].ActivateWeapon();
            activeGun = guns[newGunIndex];
        }
    }
}

