using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShooterController : MonoBehaviour
{

    int _currentWeaponIndex;
    int _currentAmmo;
    Camera _camera;
    bool _canShoot;
    bool _fireHeld;
    bool _canSwap;
    bool _reloading;

    //can change this. did this for testing mostly
    [SerializeField] Weapon[] Weapons;
    //how soon player can swap weapons again
    [Header("Weapon Swap Delay")]
    [SerializeField] float WeaponSwapSpeed =0.5f;

    void Start()
    {
        _currentWeaponIndex = 0;
        _currentAmmo = this.CurrentWeapon.maxAmmo;
        _camera = Camera.main;
        _fireHeld = false;
        _canShoot = true;
        _canSwap = true;
        _reloading = false; ;
    }

    void Update()
    {
       //fire called in updates so holding fire works
        if (_fireHeld)
        {
            //if the player has clicked or is holding fire, fire.
            FireWeapon();
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

        if (_reloading) { return; }
        //only perform once per press
        if (context.performed)
        {
            //reloads current weapon
            _currentAmmo = CurrentWeapon.maxAmmo;
            //interupt firering for reloading
            _fireHeld = false;
            //Debug.Log("Reload ammo is " + _currentAmmo.ToString());
            //probably need to get new weapon or change weapon depending dice
            StartCoroutine(CanShoot());
        }
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

    private void FireWeapon()
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
        //Debug.Log("current ammo is now" + _currentAmmo);
        // add - (crosshairImage.width / 2) if we have a crosshair
        int x = (Screen.width / 2);
        int y = (Screen.height / 2);
        Vector2 screenPos = new Vector2(x, y);
        Vector3 worldPos = _camera.ScreenToWorldPoint(screenPos);
        if (CurrentWeapon.hitScan)
        {
            // Create a vector at the center of our camera's viewport
            // Declare a raycast hit to store information about what our raycast has hit
            RaycastHit hit;
            if (Physics.Raycast(worldPos, _camera.transform.forward, out hit, CurrentWeapon.weaponRange))
            {
                
                //hit!
                if (hit.transform.gameObject.GetComponent<Enemy>())
                {
                    Debug.DrawRay(worldPos, _camera.transform.forward, Color.red, 10000f);
                    Debug.Log("hit enemy");
                    //Do enemy hit things
                }

            }
        }
        else
        {
            //do projectile thingies.
            //CameraMovement have accessors for vertical and horizontal rotation
            //Assumes prefab for bullet is kinematic
            Vector3 ballRotation = new Vector3(GetComponent<CameraMovement>().verticalRotation, GetComponent<CameraMovement>().horizontalRotation, 0f);
            GameObject ball = Instantiate(CurrentWeapon.projectile, transform.position, Quaternion.Euler(ballRotation));
            ball.GetComponent<Rigidbody>().velocity = (ball.transform.forward).normalized * CurrentWeapon.speed;
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
            //switch weapons
            _currentWeaponIndex++;
            if (_currentWeaponIndex == Weapons.Length)
            {
                _currentWeaponIndex = 0;
            }
            //switching reloads which is probably wrong
            _currentAmmo = CurrentWeapon.maxAmmo;
            _canShoot = true;
            //interupt firering for weapon switching
            _fireHeld = false;
            //Debug.Log("Current weapon is: " + CurrentWeapon.weaponName);
            //ToDo Weapon Swap animation here or throw event
            StartCoroutine(CanSwapWeapons());
        }

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
