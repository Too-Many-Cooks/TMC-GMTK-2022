using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatsUpdater : MonoBehaviour
{
    [SerializeField] ShooterController shooterController;

    [SerializeField] TextMeshProUGUI currentAmmo;
    [SerializeField] TextMeshProUGUI maxAmmo;
    [SerializeField] Image weaponIcon;
    [SerializeField] Image crosshair;

    void Start()
    {
        shooterController.OnAmmoChanged.AddListener(HandleAmmoChanged);
        shooterController.OnWeaponChanged.AddListener(HandleWeaponChanged);
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
