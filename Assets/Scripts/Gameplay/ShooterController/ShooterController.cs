using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShooterController : MonoBehaviour
{
    Weapon _currentWeapon;
    int _weaponIndex;
    int _currentAmmo;
    Camera _camera;


    //can change this. did this for testing mostly
    [SerializeField] Weapon[] Weapons;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentWeapon = Weapons[0];
        //TODO change this depedning on show camera is setup
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //mousePosition = Mouse.current.position.ReadValue()
    }

    public Weapon CurrentWeapon
    {
        get { return _currentWeapon; }
    }
    public int Ammo
    {
        get { return _currentAmmo; }
        set { _currentAmmo = value; }
    }
    void Reload()
    {
        //reloads current weapon
    }

    void OnFire()
    {
        //Message for Fire from Input System
        //Fires current gun.

        //decrease ammo
        _currentAmmo -= _currentWeapon.ammoUsuage;


        //probably need to get center of screen for hitting.
        // add - (crosshairImage.width / 2) if we have a crosshair
        int x = (Screen.width / 2);
        int y = (Screen.height / 2);
        Vector2 screenPos = new Vector2(x, y);
        //Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPos = _camera.ScreenToWorldPoint(screenPos);
        Debug.Log("Hit at: " + worldPos.ToString());
    }

    void OnNextWeapon()
    {
        //switch weapons
        _weaponIndex++;
        if(_weaponIndex == Weapons.Length)
        {
            _currentWeapon = Weapons[0];
            _weaponIndex = 0;
        }
        else
        {
            _currentWeapon = Weapons[_weaponIndex];
        }
        Debug.Log("Current weapon is: " + _currentWeapon.weaponName);
        //ToDo Weapon Swap animation here or throw event
    }
}
