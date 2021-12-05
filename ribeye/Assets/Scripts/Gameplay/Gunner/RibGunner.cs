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
    
    [SerializeField]
    private int weaponIndexStart = 0;
    
    void Start()
    {
        gunrack = GetComponentsInChildren<RibWeaponBase>();
        Array.Resize(ref guns, 9);
        foreach (var gun in gunrack)
        {
            guns[gun.GetWeaponActivationIndex()] = gun;
            gun.DeactivateWeapon();
        }
        ActivateGun(weaponIndexStart);
    }

    // Update is called once per frame
    void Update()
    {
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

    void TryActivateGun(int gunIndex)
    {
        if (guns[gunIndex])
        {
            ActivateGun(gunIndex);
        }
    }

    void ActivateGun(int gunIndex)
    {
        if(activeGun) activeGun.DeactivateWeapon();
        if (guns[gunIndex])
        {
            guns[gunIndex].ActivateWeapon();
            activeGun = guns[gunIndex];
        }
    }
}

