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

    private RibWeaponBase activeGun;
    
    void Start()
    {
        gunrack = GetComponentsInChildren<RibWeaponBase>();
        Array.Resize(ref guns, 9);
        foreach (var gun in gunrack)
        {
            guns[gun.GetWeaponActivationIndex()] = gun;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameManager._soundManager.PlaySound(0, transform.position, transform);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) ActivateGun(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActivateGun(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ActivateGun(2);
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

