using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShooterController : MonoBehaviour
{
    public Weapon currentWeapon;
    int _currentAmmo;


    //can change this. did this for testing mostly
    [SerializeField] Weapon[] Weapons;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //mousePosition = Mouse.current.position.ReadValue()
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
        
    }
    void Fire()
    {
        //shoots the weapon
    }
    void SwitchWeapon()
    {
        //switch weapons
    }
}
