using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType", menuName = "EnemyType/Generic", order = 1)]
public class EnemyType : ScriptableObject
{
    public string enemyName;
    public float preferredDistance;
    public float visionRange = 20f;
    public float health = 100f;

    //public Weapon usedWeapon
}