using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "JamDieFace", menuName = "Gameplay/Dice/Face/Jam", order = 3)]
public class JamDieFace : DieFace
{
    public float duration;

    public override void Use(GameObject owner)
    {
        base.Use(owner);
        var shooterController = owner.GetComponent<ShooterController>();
        shooterController.JamGun(duration);
    }
}
