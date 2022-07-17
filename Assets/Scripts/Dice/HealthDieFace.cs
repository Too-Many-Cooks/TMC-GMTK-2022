using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthDieFace", menuName = "Gameplay/Dice/Face/Health", order = 5)]
public class HealthDieFace : DieFace
{
    public float healthEffect;

    public override void Use(GameObject owner)
    {
        base.Use(owner);
        if(owner.CompareTag("Player"))
        {
            var shooterController = owner.GetComponent<ShooterController>();
            if (healthEffect < 0)
            {
                shooterController.StartCoroutine(DamagePlayer(owner, -healthEffect, shooterController.CurrentWeapon.reloadSpeed));
            }
            if (healthEffect > 0)
            {
                shooterController.StartCoroutine(HealPlayer(owner, healthEffect, shooterController.CurrentWeapon.reloadSpeed));
            }
        }
    }

    public IEnumerator DamagePlayer(GameObject owner, float damage, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        PlayerStatus playerStatus = owner.GetComponent<PlayerStatus>();
        playerStatus.DamageHealth(damage);
    }

    public IEnumerator HealPlayer(GameObject owner, float healAmount, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        PlayerStatus playerStatus = owner.GetComponent<PlayerStatus>();
        playerStatus.Heal(healAmount);
    }
}
