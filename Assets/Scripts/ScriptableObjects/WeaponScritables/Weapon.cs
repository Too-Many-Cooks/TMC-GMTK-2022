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
    //how big the reticle for the weapon is
    //E.G shotgun is easier to hit with
    //0 is normal point reticle
    //.25 seems good for shotgun
    //otherwise many how many unit is the radius of the reticle
    //only applicable for hitscan weapons
    public float reticleRadius;
    //can the weapon hit multiple targets
    //ie the shotgun
    //only applicable for hitscan
    public bool multiHit;
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
    //weapon sprite
    public Sprite weaponSprite;
    //weapon crosshair
    public Sprite crosshair;
}