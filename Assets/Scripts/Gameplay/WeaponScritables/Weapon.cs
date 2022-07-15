using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 50)]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int damage;
    public int maxAmmo;
    public Vector3[] spawnPoints;
}