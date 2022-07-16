using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class ShooterController : MonoBehaviour
{
    bool _isPlayer;
    int _currentWeaponIndex;
    public int _currentAmmo;
    Camera _camera;
    bool _canShoot;
    bool _fireHeld;
    bool _canSwap;
    bool _reloading;

    AudioSource _audioSource;
    //can change this. did this for testing mostly
    public Weapon[] Weapons;
    //Only gets updated and called when switching weapons to avoid swap-reloading
    public Dictionary<int, int> weaponCurrentAmmo = new Dictionary<int, int>();
    //how soon player can swap weapons again
    //this could be weapon specific
    [Header("Weapon Swap Delay")]
    [SerializeField] float WeaponSwapSpeed =0.5f;
    [SerializeField] Transform aimOrientation;

    public class WeaponChangeEvent : UnityEvent<Weapon> { }
    public WeaponChangeEvent OnWeaponChanged = new WeaponChangeEvent();

    public class AmmoChangedEvent : UnityEvent<int, int> { }
    public AmmoChangedEvent OnAmmoChanged = new AmmoChangedEvent();


    void Start()
    {
        _isPlayer = GetComponent<PlayerMovement>() != null;
        _currentWeaponIndex = 0;
        _currentAmmo = this.CurrentWeapon.maxAmmo;
        OnWeaponChanged.Invoke(CurrentWeapon);
        OnAmmoChanged.Invoke(_currentAmmo, CurrentWeapon.maxAmmo);
        if(_isPlayer)
            _camera = Camera.main;
        _fireHeld = false;
        _canShoot = true;
        _canSwap = true;
        _reloading = false; ;
        _audioSource = this.GetComponent<AudioSource>();
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
        get { return Weapons[_currentWeaponIndex]; }
    }
    public int Ammo
    {
        get { return _currentAmmo; }
        set { _currentAmmo = value; }
    }
    public void Reload(InputAction.CallbackContext context)
    {

        //only perform once per press
        if (context.performed)
        {
            ReloadWeapon();
        }
    }

    public void ReloadWeapon()
    {
        if (_reloading || !_canShoot) { return; }

        //reloads current weapon
        _currentAmmo = CurrentWeapon.maxAmmo;
        OnAmmoChanged.Invoke(_currentAmmo, CurrentWeapon.maxAmmo);
        //interupt firering for reloading
        _fireHeld = false;
        //Debug.Log("Reload ammo is " + _currentAmmo.ToString());
        _audioSource.clip = CurrentWeapon.weaponReloadSound;
        _audioSource.Play();
        //probably need to get new weapon or change weapon depending dice
        StartCoroutine(Reloading());
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
        if (_currentAmmo <= 0)
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
        _currentAmmo -= CurrentWeapon.ammoUsuage;
        OnAmmoChanged.Invoke(_currentAmmo, CurrentWeapon.maxAmmo);
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
                        hit.transform.gameObject.GetComponent<Enemy>()?.DamageHealth(CurrentWeapon.damage);
                    }
                    else
                    {
                        Debug.Log("hit player");
                        hit.transform.gameObject.GetComponent<PlayerStatus>()?.DamageHealth(CurrentWeapon.damage);
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
            ball.GetComponent<Rigidbody>().velocity = (ball.transform.forward).normalized * CurrentWeapon.projectileSpeed;
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
            weaponCurrentAmmo[_currentWeaponIndex] = _currentAmmo;
            //switch weapons
            _currentWeaponIndex++;
            if (_currentWeaponIndex == Weapons.Length)
            {
                _currentWeaponIndex = 0;
            }
            //load ammo of new weapon (if available)
            _currentAmmo = weaponCurrentAmmo.ContainsKey(_currentWeaponIndex) ? weaponCurrentAmmo[_currentWeaponIndex] : CurrentWeapon.maxAmmo;
            OnAmmoChanged.Invoke(_currentAmmo, CurrentWeapon.maxAmmo);
            OnWeaponChanged.Invoke(CurrentWeapon);
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
        return _currentAmmo <= 0;
    }

    IEnumerator CanShoot()

    {

        _canShoot = false;

        yield return new WaitForSeconds(CurrentWeapon.fireRate);

        _canShoot = true;

    }
    IEnumerator CanSwapWeapons()

    {

        _canSwap = false;

        yield return new WaitForSeconds(WeaponSwapSpeed);

        _canSwap = true;

    }
    IEnumerator Reloading()

    {

        _reloading = true;

        yield return new WaitForSeconds(CurrentWeapon.reloadSpeed);

        _reloading = false;

    }


}
