using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 50)]
public class Weapon : ScriptableObject
{
    //name of the weapon
    public string weaponName;
    //damage of the weapon
    public int damage;
    //max/starting ammo
    public int maxAmmo;
    //whether or not this is a hitscan usuage
    public bool hitScan;
    //how much ammow goes down when firing
    public int ammoUsuage;
    //range of the weapon
    public float weaponRange;
    //fire rate of weapon
    public float fireRate;
    //project weapon shoots

}