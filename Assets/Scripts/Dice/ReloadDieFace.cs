using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReloadDieFace", menuName = "Gameplay/Dice/Face/Reload", order = 3)]
public class ReloadDieFace : DieFace
{
    public float ammoMultiplier = 1.0f;
    public float damageMultiplier = 1.0f;
    public float fireRateMultiplier = 1.0f;
    public float projectileSpeedMultiplier = 1.0f;
    public float weaponRangeMultiplier = 1.0f;

    public override void Use(GameObject owner)
    {
        base.Use(owner);
        var shooterController = owner.GetComponent<ShooterController>();
        shooterController.AmmoModifier = ammoMultiplier;
        shooterController.DamageMultiplier = damageMultiplier;
        shooterController.FireRateMultiplier = fireRateMultiplier;
        shooterController.ProjectileSpeedMultiplier = projectileSpeedMultiplier;
        shooterController.WeaponRangeMultiplier = weaponRangeMultiplier;
        shooterController.ReloadWeapon();
    }

}
