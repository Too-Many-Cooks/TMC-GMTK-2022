using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon", order = 2)]
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
    //projectile gameobject
    [SerializeField]
    public GameObject projectile;
    //projectile speed
    public float projectileSpeed;
    //how long reload takes
    public float reloadSpeed;
    //audio clip
    public AudioClip weaponShotSound;
    //audio clip
    public AudioClip weaponReloadSound;
}