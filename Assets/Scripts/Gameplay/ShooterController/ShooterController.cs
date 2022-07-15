using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterController : MonoBehaviour
{


    public Weapon currentWeapon;
    _currentAmmo;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int Ammo
    {
        get { return _currentAmmo; }
        set { _ammo = value; }
    }
    void Reload()
    {
        //reloads current weapon
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
