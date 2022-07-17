using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatsUpdater : MonoBehaviour
{
   

    ShooterController shooterController;
    [SerializeField] TextMeshProUGUI currentAmmo;
    [SerializeField] TextMeshProUGUI maxAmmo;
    [SerializeField] Image weaponIcon;
    [SerializeField] Image jamIcon;
    [SerializeField] Image crosshair;

    void Start()
    {
        shooterController = FindObjectOfType<PlayerMovement>().GetComponent<ShooterController>();
        shooterController.OnAmmoChanged.AddListener(HandleAmmoChanged);
        shooterController.OnWeaponChanged.AddListener(HandleWeaponChanged);
        
    }

    private void Update()
    {
        jamIcon.enabled = shooterController.CurrentWeaponIsJammed;
    }

    private void HandleWeaponChanged(Weapon newWeapon)
    {   
        weaponIcon.sprite = newWeapon.weaponSprite;
        crosshair.sprite = newWeapon.crosshair;
    }

    private void HandleAmmoChanged(int currentAmmo, int maxAmmo)
    {
        this.currentAmmo.text = currentAmmo.ToString();
        this.maxAmmo.text = maxAmmo.ToString();
    }
}
