using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class ShooterController : MonoBehaviour
{
    bool _isPlayer;
    int _currentWeaponIndex;
    [SerializeField] int _currentReloadDieIndex;
    public int AmmoCount { get { return WeaponSlots[_currentWeaponIndex].ammoCount; } set { WeaponSlots[_currentWeaponIndex].ammoCount = value; } }
    Camera _camera;
    bool _canShoot;
    bool _fireHeld;
    bool _canSwap;
    bool _reloading;

    public float AmmoModifier { get { return WeaponSlots[_currentWeaponIndex].ammoModifier; } set { WeaponSlots[_currentWeaponIndex].ammoModifier = value; } }
    public float DamageMultiplier { get { return WeaponSlots[_currentWeaponIndex].damageMultiplier; } set { WeaponSlots[_currentWeaponIndex].damageMultiplier = value; } }
    public float FireRateMultiplier { get { return WeaponSlots[_currentWeaponIndex].fireRateMultiplier; } set { WeaponSlots[_currentWeaponIndex].fireRateMultiplier = value; } }
    public float ProjectileSpeedMultiplier { get { return WeaponSlots[_currentWeaponIndex].projectileSpeedMultiplier; } set { WeaponSlots[_currentWeaponIndex].projectileSpeedMultiplier = value; } }
    public float WeaponRangeMultiplier { get { return WeaponSlots[_currentWeaponIndex].weaponRangeMultiplier; } set { WeaponSlots[_currentWeaponIndex].weaponRangeMultiplier = value; } }

    public int LastReloadIndex { get; private set; }

    AudioSource _audioSource;
    //can change this. did this for testing mostly
    public WeaponSlot[] WeaponSlots;

    public Die[] ReloadDice;
    public bool HasReloadDie { get { return ReloadDice.Length > 0; } }
    //Only gets updated and called when switching weapons to avoid swap-reloading
    public Dictionary<int, int> weaponCurrentAmmo = new Dictionary<int, int>();
    //how soon player can swap weapons again
    //this could be weapon specific
    [Header("Weapon Swap Delay")]
    [SerializeField] float WeaponSwapSpeed =0.5f;
    [SerializeField] Transform aimOrientation;

    void Start()
    {
        _isPlayer = GetComponent<PlayerMovement>() != null;
        _currentWeaponIndex = 0;
        if(_isPlayer)
            _camera = Camera.main;
        _fireHeld = false;
        _canShoot = true;
        _canSwap = true;
        _reloading = false;
        _audioSource = this.GetComponent<AudioSource>();
        for (int i = 0; i < WeaponSlots.Length; i++)
        {
            ReloadWeaponInstant(i);
        }
    }

    void Update()
    {
        //fire called in updates so holding fire works
        if (_isPlayer && _fireHeld)
        {
            //if the player has clicked or is holding fire, fire.
            int x = (Screen.width / 2);
            int y = (Screen.height / 2);
            Vector2 screenPos = new Vector2(x, y);
            Vector3 worldPos = _camera.ScreenToWorldPoint(screenPos);
            FireWeapon(worldPos, _camera.transform.rotation);
        }
    }

    public Weapon CurrentWeapon
    {
        get { return WeaponSlots[_currentWeaponIndex].weapon; }
    }

    public Die CurrentReloadDie
    {
        get { return ReloadDice[_currentReloadDieIndex]; }
    }
    public int Ammo
    {
        get { return AmmoCount; }
        set { AmmoCount = value; }
    }
    public void Reload(InputAction.CallbackContext context)
    {

        //only perform once per press
        if (context.performed)
        {
            if(HasReloadDie)
            {
                int reloadDieFaceIndex;
                var reloadDieFace = CurrentReloadDie.Roll(out reloadDieFaceIndex);
                LastReloadIndex = reloadDieFaceIndex;
                reloadDieFace.Use(gameObject);
            } else
            {
                ReloadWeapon();
            }
        }
    }

    public void ReloadWeapon(int weaponIndex = -1, bool instant = false)
    {
        if (weaponIndex < 0)
            weaponIndex = _currentWeaponIndex;
        if (_reloading || !_canShoot) { return; }

        if(!instant)
        {
            _audioSource.clip = WeaponSlots[weaponIndex].weapon.weaponReloadSound;
            _audioSource.Play();
        }
        //probably need to get new weapon or change weapon depending dice
        StartCoroutine(Reloading(_currentWeaponIndex, instant));
    }

    public void ReloadWeaponInstant(int weaponIndex = -1)
    {
        ReloadWeapon(weaponIndex, true);
    }

    public void Fire(InputAction.CallbackContext context)
    {
        //check if fire was hit and then held
        if (context.performed)
        {
            _fireHeld = true;
        }
        else if (context.canceled)
        {
            _fireHeld = false;
        }
    }

    public void FireWeapon(Vector3 shotOriginPositionInWorldCoords, Quaternion shotOrientation)
    {
        //Message for Fire from Input System
        //Fires current gun.
        if (AmmoCount <= 0)
        {
            //No ammo
            //can't shoot
            //Debug.Log("Out of ammo");
            //maybe play click sound, throw out of ammo event
            return;
        }
        if (!_canShoot) return;
        if (_reloading) return;
        //decrease ammo
        AmmoCount -= CurrentWeapon.ammoUsuage;
        //play weapon sound
        _audioSource.clip = CurrentWeapon.weaponShotSound;
        _audioSource.Play();

        //Debug.Log("current ammo is now" + _currentAmmo);
        // add - (crosshairImage.width / 2) if we have a crosshair
        if (CurrentWeapon.hitScan)
        {
            Vector3 shotDirection = shotOrientation * Vector3.forward;
            // Create a vector at the center of our camera's viewport
            // Declare a raycast hit to store information about what our raycast has hit
            RaycastHit hit;
            if (Physics.Raycast(shotOriginPositionInWorldCoords, shotDirection, out hit, CurrentWeapon.weaponRange))
            {
                
                //hit!
                if (hit.transform.gameObject.GetComponent<Enemy>() || hit.transform.gameObject.GetComponent<PlayerStatus>())
                {
                    Debug.DrawRay(shotOriginPositionInWorldCoords, shotDirection, Color.red, 10000f);
                    //Do enemy hit things
                    //damage enemy
                    if (_isPlayer)
                    {
                        Debug.Log("hit enemy");
                        hit.transform.gameObject.GetComponent<Enemy>()?.DamageHealth(CurrentWeapon.damage * DamageMultiplier);
                    }
                    else
                    {
                        Debug.Log("hit player");
                        hit.transform.gameObject.GetComponent<PlayerStatus>()?.DamageHealth(CurrentWeapon.damage * DamageMultiplier);
                    }
                }

            }
        }
        else
        {
            //do projectile thingies.
            //CameraMovement have accessors for vertical and horizontal rotation
            //Assumes prefab for bullet is kinematic
            //need to update origin to end of gun or w/e

            // TODO: Check with Victor if accurate
            GameObject ball = Instantiate(CurrentWeapon.projectile, shotOriginPositionInWorldCoords, shotOrientation);//Quaternion.Euler(ballRotation));
            ball.GetComponent<Rigidbody>().velocity = (ball.transform.forward).normalized * CurrentWeapon.projectileSpeed * ProjectileSpeedMultiplier;
            //rely on bullets to do hit detection
        }
        //pause until we can shoot again
        StartCoroutine(CanShoot());

    }

    public void NextWeapon(InputAction.CallbackContext context)
    {
        if (!_canSwap) return;
        //no swapping while reloading
        if (_reloading) return;
        //only perform once per press
        if (context.performed)
        {
            Debug.Log("Switch weapon");

            //store ammo of current weapon
            weaponCurrentAmmo[_currentWeaponIndex] = AmmoCount;
            //switch weapons
            _currentWeaponIndex++;
            if (_currentWeaponIndex == WeaponSlots.Length)
            {
                _currentWeaponIndex = 0;
            }
            //load ammo of new weapon (if available)
            AmmoCount = weaponCurrentAmmo.ContainsKey(_currentWeaponIndex) ? weaponCurrentAmmo[_currentWeaponIndex] : CurrentWeapon.maxAmmo;
            _canShoot = true;
            //interupt firering for weapon switching
            _fireHeld = false;
            //Debug.Log("Current weapon is: " + CurrentWeapon.weaponName);
            //ToDo Weapon Swap animation here or throw event
            StartCoroutine(CanSwapWeapons());
        }

    }

    public bool IsOutOfAmmo()
    {
        return AmmoCount <= 0;
    }

    IEnumerator CanShoot()

    {

        _canShoot = false;

        yield return new WaitForSeconds(CurrentWeapon.fireRate / FireRateMultiplier);

        _canShoot = true;

    }
    IEnumerator CanSwapWeapons()

    {

        _canSwap = false;

        yield return new WaitForSeconds(WeaponSwapSpeed);

        _canSwap = true;

    }
    IEnumerator Reloading(int weaponIndex, bool instant = false)
    {

        _reloading = true;

        if(!instant)
        {
            yield return new WaitForSeconds(WeaponSlots[weaponIndex].weapon.reloadSpeed);
        }

        //reloads current weapon
        AmmoCount = Mathf.FloorToInt(WeaponSlots[weaponIndex].weapon.maxAmmo * AmmoModifier);
        //interupt firering for reloading
        _fireHeld = false;

        _reloading = false;

    }

    [Serializable]
    public struct WeaponSlot
    {
        public WeaponSlot(Weapon weapon)
        {
            this.weapon = weapon;
            ammoCount = weapon.maxAmmo;
            ammoModifier = 1.0f;
            damageMultiplier = 1.0f;
            fireRateMultiplier = 1.0f;
            projectileSpeedMultiplier = 1.0f;
            weaponRangeMultiplier = 1.0f;
        }

        public Weapon weapon;
        public int ammoCount;
        public float ammoModifier;
        public float damageMultiplier;
        public float fireRateMultiplier;
        public float projectileSpeedMultiplier;
        public float weaponRangeMultiplier;
    }
}
