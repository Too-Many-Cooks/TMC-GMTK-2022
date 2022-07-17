using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BarrelDieFace", menuName = "Gameplay/Dice/Face/Barrel", order = 3)]
public class BarrelDieFace : DieFace
{
    [SerializeField]
    GameObject OverrideBullet;

    public override void Use(GameObject owner)
    {
        base.Use(owner);
        var shooterController = owner.GetComponent<ShooterController>();
        shooterController.WeaponSlots[shooterController.CurrentWeaponIndex].overideBullet = OverrideBullet;
        shooterController.ReloadWeapon();
    }
}
