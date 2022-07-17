using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using EZCameraShake;

public class PlayerStatus : MonoBehaviour
{
    float _health;
    public float maxHealth;
    bool _isDead = false;


    [SerializeField]
    public bool hitMode = false;

    public class HealthChangedEvent : UnityEvent<float> { }
    public HealthChangedEvent OnHealthChanged = new HealthChangedEvent();

    public float Health
    {
        get
        {
            return _health;
        }
    }

    public bool Dead
    {
        get
        {
            return _isDead;
        }
    }

    private void Start()
    {
        if(hitMode)
        {
            maxHealth = 10;
        }
        _health = maxHealth;
        OnHealthChanged.Invoke(_health);
    }

    public void DamageHealth(float damage)
    {
        if (_isDead)
            return;
        
        if (!hitMode)
        {
            _health -= damage;
        }
        else
            _health--;
        OnHealthChanged.Invoke(_health);

        // Taking damage shake.
        CameraShaker.Instance.ShakeOnce(1, 2, 0.5f, 1.5f);

        if (_health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        //maybe throw event if other stuff needs to know about enemy death
        //do death animation
        Debug.Log("You died");
        GetComponent<CameraMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<ShooterController>().enabled = false;

        StartCoroutine(SimpleDeathAnim());
        _isDead = true;
    }

    private IEnumerator SimpleDeathAnim()
    {
        float duration = 2f;
        float progress = 0f;
        Quaternion initialRot = transform.rotation;
        Vector3 initialPos = transform.position;
        Quaternion targetRot = initialRot * Quaternion.Euler(0, 0, 90);
        Vector3 targetPos = initialPos - new Vector3(0, 0.5f, 0);

        while (progress < 1f)
        {
            transform.rotation = Quaternion.Slerp(initialRot, targetRot, progress);
            transform.position = Vector3.Slerp(initialPos, targetPos, progress);
            yield return null;
            progress += Time.deltaTime / duration;
        }
    }

    private void Update()
    {
        if(hitMode ? Input.GetKeyDown(KeyCode.K) : Input.GetKey(KeyCode.K))
        {
            _health -= 1f;
            OnHealthChanged.Invoke(_health);
        }
    }
}
