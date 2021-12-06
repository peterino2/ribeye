using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [SerializeField]
    private int weaponIndexStart = 0;

    private int gunIndex;
    private void Awake()
    {
        gunrack = GetComponentsInChildren<RibWeaponBase>();
        Array.Resize(ref guns, 9);
        foreach (var gun in gunrack)
        {
            guns[gun.GetWeaponActivationIndex()] = gun;
            gun.gunAnimator = gunAnimator;
            gun.DeactivateWeaponNoAnim();
        }
    }

    void Start()
    {
        ActivateGun(weaponIndexStart);
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
        if (gunIndex == newGunIndex) return;
        if (guns[newGunIndex])
        {
            ActivateGun(newGunIndex);
        }
    }


    void ActivateGun(int newGunIndex)
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

