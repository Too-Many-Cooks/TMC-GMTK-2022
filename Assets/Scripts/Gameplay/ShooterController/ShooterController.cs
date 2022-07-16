using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShooterController : MonoBehaviour
{

    int _currentWeaponIndex;
    int _currentAmmo;
    Camera _camera;
    bool _canShoot = true;
    bool _fireHeld;

    //can change this. did this for testing mostly
    [SerializeField] Weapon[] Weapons;

    // Start is called before the first frame update
    void Start()
    {
        _currentWeaponIndex = 0;
        _currentAmmo = this.CurrentWeapon.maxAmmo;
        //TODO change this depedning on show camera is setup
        _camera = Camera.main;
        _fireHeld = false;

    }

    // Update is called once per frame
    void Update()
    {
        //mousePosition = Mouse.current.position.ReadValue()
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
    public void Reload()
    {
        //reloads current weapon
        _currentAmmo = CurrentWeapon.maxAmmo;
        //Debug.Log("Reload ammo is " + _currentAmmo.ToString());
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
            //Debug.Log("Out of ammo");
            //maybe play click sound, throw out of ammo event
            return;
        }
        if (!_canShoot) return;

        //decrease ammo
        _currentAmmo -= CurrentWeapon.ammoUsuage;
        //Debug.Log("current ammo is now" + _currentAmmo);
        //probably need to get center of screen for hitting.
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
                    Debug.Log("hit enemy");
                    //Do enemy hit things
                }

            }
        }
        else
        {
            //do projectile thingies.
            //testing with camera pivot
            GameObject cameraPivot = GameObject.Find("CameraPivot");
            GameObject ball = Instantiate(CurrentWeapon.projectile, transform.position, cameraPivot.transform.rotation);
            ball.GetComponent<Rigidbody>().velocity = transform.forward * CurrentWeapon.speed;
            Physics.IgnoreCollision(ball.GetComponent<Collider>(), this.gameObject.GetComponent<Collider>(), true);
            Physics.IgnoreCollision(ball.GetComponent<Collider>(), this.gameObject.GetComponentInChildren<Collider>(), true);

            //ball.GetComponent<Rigidbody>().AddForce(Vector3.forward * 100);
        }
        //pause until we can shoot again
        StartCoroutine(CanShoot());

    }

    public void NextWeapon()
    {
        //switch weapons
        _currentWeaponIndex++;
        if (_currentWeaponIndex == Weapons.Length)
        {
            _currentWeaponIndex = 0;
        }
        //switching reloads which is probably wrong
        _currentAmmo = CurrentWeapon.maxAmmo;
        _canShoot = true;
        //Debug.Log("Current weapon is: " + CurrentWeapon.weaponName);
        //ToDo Weapon Swap animation here or throw event
    }

    IEnumerator CanShoot()

    {

        _canShoot = false;

        yield return new WaitForSeconds(CurrentWeapon.fireRate);

        _canShoot = true;

    }
}
