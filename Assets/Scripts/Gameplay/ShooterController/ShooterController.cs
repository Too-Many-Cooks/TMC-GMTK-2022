using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

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
    bool _canSwapReloadDie;
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
    //how soon player can swap weapons again
    //this could be weapon specific
    [Header("Weapon Swap Delay")]
    [SerializeField] float WeaponSwapSpeed =0.5f;
    [SerializeField] float ReloadDieSwapSpeed = 0.5f;
    [SerializeField] Transform aimOrientation;

    IObjectPool<Projectile> _pool;
    [SerializeField] int projectilePoolCapacity = 25;


    [Header("Weapon GameObjects")]
    [SerializeField] Animator revolverAnimator;
    [SerializeField] Animator shotgunAnimator;


    public class WeaponChangeEvent : UnityEvent<Weapon> { }
    public WeaponChangeEvent OnWeaponChanged = new WeaponChangeEvent();

    public class ReloadDieChangeEvent : UnityEvent<Die, int> { }
    public ReloadDieChangeEvent OnReloadDieChanged = new ReloadDieChangeEvent();

    public class AmmoChangedEvent : UnityEvent<int, int> { }
    public AmmoChangedEvent OnAmmoChanged = new AmmoChangedEvent();



    void Start()
    {
        _isPlayer = GetComponent<PlayerMovement>() != null;
        _currentWeaponIndex = 0;
        if(_isPlayer)
            _camera = Camera.main;
        _fireHeld = false;
        _canShoot = true;
        _canSwap = true;
        _canSwapReloadDie = true;
        _reloading = false;
        _audioSource = this.GetComponent<AudioSource>();
        for (int i = 0; i < WeaponSlots.Length; i++)
        {
            ReloadWeaponInstant(i);
        }
        OnWeaponChanged.Invoke(CurrentWeapon);
        OnAmmoChanged.Invoke(AmmoCount, CurrentWeapon.maxAmmo);
        OnReloadDieChanged.Invoke(CurrentReloadDie, CurrentReloadDieIndex);
    }

    void Update()
    {
        UpdateWeaponSlots();
        //fire called in updates so holding fire works
        if (_isPlayer && _fireHeld &&_canSwap)
        {
            //if the player has clicked or is holding fire, fire.
            int x = (Screen.width / 2);
            int y = (Screen.height / 2);
            Vector2 screenPos = new Vector2(x, y);
            Vector3 worldPos = _camera.ScreenToWorldPoint(screenPos);
            FireWeapon(worldPos, _camera.transform.rotation);
        }
    }

    public void PickUp(InputAction.CallbackContext context)
    {
        RaycastHit newHit;

        LayerMask layerMask = LayerMask.GetMask("PickUp");
        bool didHit = Physics.Raycast(_camera.transform.position, _camera.transform.forward, out newHit, 5f, layerMask);

        if (!didHit)
            return;

        Die newDie = newHit.collider.gameObject.GetComponent<DieDisplay>().Die;

        ReloadDice[_currentReloadDieIndex] = newDie;
        OnReloadDieChanged.Invoke(newDie, _currentReloadDieIndex);
        
        Destroy(newHit.collider.gameObject);

        Debug.Log("Picked up");
    }

    void UpdateWeaponSlots()
    {
        for (int i = 0; i < WeaponSlots.Length; i++) {
            UpdateWeaponSlot(i);
        }
    }

    void UpdateWeaponSlot(int i)
    {
        if(WeaponSlots[i].jamTimer > 0.0f)
        {
            WeaponSlots[i].jamTimer -= Mathf.Min(Time.deltaTime, WeaponSlots[i].jamTimer);
        }
    }

    public int CurrentWeaponIndex
    {
        get { return _currentWeaponIndex; }
    }

    public WeaponSlot CurrentWeaponSlot
    {
        get { return WeaponSlots[_currentWeaponIndex]; }
    }

    public Weapon CurrentWeapon
    {
        get { return WeaponSlots[_currentWeaponIndex].weapon; }
    }

    public Die CurrentReloadDie
    {
        get { return HasReloadDie ? ReloadDice[_currentReloadDieIndex] : null; }
    }

    public int CurrentReloadDieIndex
    {
        get { return _currentReloadDieIndex; }
    }

    public int Ammo
    {
        get { return AmmoCount; }
        set { AmmoCount = value; }
    }

    public bool CurrentWeaponIsJammed
    {
        get { return CurrentWeaponSlot.IsJammed; }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        if(CurrentWeaponIsJammed)
        {
            return;
        }
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
        if (!instant && (_reloading || !_canShoot)) { return; }

        if(!instant)
        {
            if (_isPlayer)
            {
                if (CurrentWeapon.weaponName == "Shotgun")
                    shotgunAnimator?.SetTrigger("Reload");
                else if (CurrentWeapon.weaponName == "Pistol")
                    revolverAnimator?.SetTrigger("Reload");
                else
                    Debug.LogError("Couldn't find weapon with name: " + CurrentWeapon.weaponName);
            }
                _audioSource.clip = WeaponSlots[weaponIndex].weapon.weaponReloadSound;
                _audioSource.Play();
            
        }


        //probably need to get new weapon or change weapon depending dice
        StartCoroutine(Reloading(weaponIndex, instant));
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
        if (CurrentWeaponIsJammed) return;
        if (!_canShoot) return;
        if (_reloading) return;
        //decrease ammo
        AmmoCount -= CurrentWeapon.ammoUsuage;
        OnAmmoChanged.Invoke(AmmoCount, CurrentWeapon.maxAmmo);

        // Animation triggers.



        if (CurrentWeapon.weaponName == "Shotgun")
            shotgunAnimator?.SetTrigger("Fire");
        else if (CurrentWeapon?.weaponName == "Pistol")
            revolverAnimator?.SetTrigger("Fire");
        else
            Debug.LogError("Couldn't find weapon with name: " + CurrentWeapon.weaponName);

        
       
        //Debug.Log("current ammo is now" + _currentAmmo);
        // add - (crosshairImage.width / 2) if we have a crosshair
        if (CurrentWeapon.hitScan)
        {
            FireHitScans(shotOriginPositionInWorldCoords, shotOrientation);
        }
        else
        {
            //do projectile thingies.
            //CameraMovement have accessors for vertical and horizontal rotation
            //Assumes prefab for bullet is kinematic
            //need to update origin to end of gun or w/e
            GameObject ball = Instantiate(CurrentWeapon.projectile, shotOriginPositionInWorldCoords, shotOrientation);//Quaternion.Euler(ballRotation));
            var projectileComponent = ball.GetComponent<Projectile>();
            projectileComponent.damagesEnemy = true;
            projectileComponent.damagesPlayer = true;
            projectileComponent.owner = gameObject;
            ball.GetComponent<Rigidbody>().velocity = (ball.transform.forward).normalized * CurrentWeapon.projectileSpeed * ProjectileSpeedMultiplier;
            ball.GetComponent<Projectile>().Damage = (CurrentWeapon.damage* DamageMultiplier);
            //rely on bullets to do hit detection
        }
        //pause until we can shoot again
        StartCoroutine(CanShoot());

    }

    private void FireHitScans(Vector3 shotOriginPositionInWorldCoords, Quaternion shotOrientation)
    {
        //Logic for fireing hitscan weapons
        Vector3 shotDirection = shotOrientation * Vector3.forward;
        // Create a vector at the center of our camera's viewport
        // Declare a raycast hit to store information about what our raycast has hit
        //for larger reticles throw a bunch of raycasts
        //this isnt a cone shot more like a cylindar
        //Build a list of origin points
        float reticleStep = .1f;
        List<Vector3> origins = new List<Vector3>();
        for (float x = 0; x <= CurrentWeapon.reticleRadius; x += reticleStep)
        {
            for (float y = 0; y <= CurrentWeapon.reticleRadius; y += reticleStep)
            {
                origins.Add(shotOriginPositionInWorldCoords + transform.right * x + transform.up * y);
                origins.Add(shotOriginPositionInWorldCoords - transform.right * x + transform.up * y);
                origins.Add(shotOriginPositionInWorldCoords - transform.right * x - transform.up * y);
                origins.Add(shotOriginPositionInWorldCoords + transform.right * x - transform.up * y);
            }
        }
        List<RaycastHit> hits = new List<RaycastHit>();
        foreach (Vector3 origin in origins)
        {
            RaycastHit newHit;
            bool didHit = Physics.Raycast(origin, shotDirection, out newHit, CurrentWeapon.weaponRange* WeaponRangeMultiplier);
            if (didHit)
            {
                if (newHit.transform.gameObject.GetComponent<Enemy>() || newHit.transform.gameObject.GetComponent<PlayerStatus>())
                {
                    hits.Add(newHit);
                }
            }
            //Debug.DrawRay(origin, shotDirection, Color.red, 10000f);

        }
        List<GameObject> peopleHit = new List<GameObject>();
        foreach (RaycastHit hit in hits)
        {
            
            if (_isPlayer)
            {
                //Debug.Log("hit enemy");
                
                if (!peopleHit.Contains(hit.transform.gameObject))
                {
                    peopleHit.Add(hit.transform.gameObject);
                    hit.transform.gameObject.GetComponent<Enemy>()?.DamageHealth(CurrentWeapon.damage*DamageMultiplier);
                   
                    //don't break so other rays can hit other people.
                    Debug.Log("I hit enemy!");
                }  
                
            }
            else
            {
                //Debug.Log("hit player");
                if (!peopleHit.Contains(hit.transform.gameObject))
                {
                    peopleHit.Add(hit.transform.gameObject);
                    hit.transform.gameObject.GetComponent<PlayerStatus>()?.DamageHealth(CurrentWeapon.damage);
                    Debug.Log("I got hit");
                }
            }

        }
    }

    public void NextWeapon(InputAction.CallbackContext context)
    {
        //no swapping while swapping
        if (!_canSwap) return;
        //no swapping while reloading
        if (_reloading) return;
        //only perform once per press
        if (context.performed)
        {
            Debug.Log("Switch weapon");

            //store ammo of current weapon
            //switch weapons
            _currentWeaponIndex++;
            if (_currentWeaponIndex == WeaponSlots.Length)
            {
                _currentWeaponIndex = 0;
            }
            OnAmmoChanged.Invoke(AmmoCount, CurrentWeapon.maxAmmo);
            OnWeaponChanged.Invoke(CurrentWeapon);
            _canShoot = false;
            //interupt firering for weapon switching
            //this coroutine ensures no firing or reloading during CanSwapWeaponSpeed
            StartCoroutine(CanSwapWeapons());
            
        }

    }

    public void NextReloadDie(InputAction.CallbackContext context)
    {
        if (!_canSwap) return;
        if (!_canSwapReloadDie) return;
        //no swapping while reloading
        if (_reloading) return;
        //only perform once per press
        if (context.performed)
        {
            Debug.Log("Switch reload die");

            //store ammo of current weapon
            //switch weapons
            _currentReloadDieIndex++;
            if (_currentReloadDieIndex == ReloadDice.Length)
            {
                _currentReloadDieIndex = 0;
            }
            OnReloadDieChanged?.Invoke(CurrentReloadDie, CurrentReloadDieIndex);
            //ToDo Die Swap animation here or throw event
            StartCoroutine(CanSwapReloadDie());
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
        _fireHeld = false;
        _canShoot = false;
        _canSwap = false;

        yield return new WaitForSeconds(WeaponSwapSpeed);

        _fireHeld = false;
        _canShoot = true;
        _canSwap = true;

    }
    IEnumerator CanSwapReloadDie()

    {

        _canSwapReloadDie = false;

        yield return new WaitForSeconds(ReloadDieSwapSpeed);

        _canSwapReloadDie = true;

    }
    IEnumerator Reloading(int weaponIndex, bool instant = false)
    {

        _reloading = true;

        if(!instant)
        {
            yield return new WaitForSeconds(WeaponSlots[weaponIndex].weapon.reloadSpeed);
        }

        //reloads current weapon
        WeaponSlots[weaponIndex].ammoCount = Mathf.FloorToInt(WeaponSlots[weaponIndex].weapon.maxAmmo * WeaponSlots[weaponIndex].ammoModifier);
        if(weaponIndex == _currentWeaponIndex)
		{
			OnAmmoChanged.Invoke(WeaponSlots[weaponIndex].ammoCount, CurrentWeapon.maxAmmo);
		}

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
            jamTimer = 0.0f;
        }

        public Weapon weapon;
        public int ammoCount;
        public float jamTimer;
        public bool IsJammed { get { return jamTimer > 0.0f; } }
        public float ammoModifier;
        public float damageMultiplier;
        public float fireRateMultiplier;
        public float projectileSpeedMultiplier;
        public float weaponRangeMultiplier;
    }
}
