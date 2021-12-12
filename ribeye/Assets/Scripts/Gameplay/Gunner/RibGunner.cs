using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Gunner;
using UnityEngine;
using UnityEngine.Assertions;

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

    public Vector3 positionTarget;
    public Quaternion rotationTarget;
    public GameObject cameraBoom;
    
    [SerializeField] public SmartAimerUI ui;

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

        inventoryUi.ammoText.text = "";
        gunrack = GetComponentsInChildren<RibWeaponBase>();
        Array.Resize(ref guns, 9);
        foreach (var gun in gunrack)
        {
            guns[gun.GetWeaponActivationIndex()] = gun;
            gun.gunAnimator = gunAnimator;
            gun.gunner = this;
            inventoryUi.SetSelector(-1);
            inventoryUi.HideIcon(gun.GetWeaponActivationIndex());
            gun.DeactivateWeaponNoAnim();
        }
        
        Assert.IsTrue(ui != null);
        
        if(ui == null) ui = FindObjectOfType<SmartAimerUI>();
    }

    public void GiveUpgradeInner(string upgrade)
    {
        upgrades.Add(upgrade.ToLower());
    }

    public RibGunnerInventory inventoryUi;
    public void GiveUpgrade(string upgrade)
    {
        print("upgrade granted +++" + upgrade);
        GiveUpgradeInner(upgrade);
        
        // automatically equip the new weapon if it's smart pistol or sword
        if (upgrade.ToLower() == "PistolBasic".ToLower())
        {
            inventoryUi.ShowIcon(0);
            TryActivateGun(0); // smart pistol index
        }
        if (upgrade.ToLower() == "Sword".ToLower())
        {
            inventoryUi.ShowIcon(1);
            TryActivateGun(1); // sword index
        }
        if (upgrade.ToLower() == "plasmacaster".ToLower())
        {
            inventoryUi.ShowIcon(2);
            TryActivateGun(2); // plasma caster index
        }
        inventoryUi.SetSelector(gunIndex);
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

    public float rotationFactor = 0.8f;

    void HandleCameraTransforms()
    {
        cameraBoom.transform.localPosition = Vector3.Lerp(positionTarget, cameraBoom.transform.localPosition,   rotationFactor);
        cameraBoom.transform.localRotation = Quaternion.Lerp(rotationTarget, cameraBoom.transform.localRotation, rotationFactor);
    }
    // Update is called once per frame
    void Update()
    {
        HandleCameraTransforms();
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryActivateGun(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryActivateGun(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryActivateGun(2);
        
        
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
            ui.ShowNone();
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


    void ActivateGun(int newGunIndex) // do not call directly only call from try activate gun
    {
        print(String.Format("Trying activation"));
        if(activeGun) activeGun.DeactivateWeapon();
        ui.ShowNone();

        inventoryUi.weaponText.text = guns[newGunIndex].GetWeaponName();
        print("doing activation");
        inventoryUi.SetSelector(newGunIndex);
        inventoryUi.ShowIcon(newGunIndex);
        gunIndex = newGunIndex;
        guns[newGunIndex].ActivateWeapon();
        activeGun = guns[newGunIndex];
        gunIndex = newGunIndex;
    }
}

