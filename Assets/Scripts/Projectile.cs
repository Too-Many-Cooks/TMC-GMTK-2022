using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;
    public float damage = 0f;
    public bool damagesPlayer = true;
    public bool damagesEnemy = true;
    public GameObject owner;
    public ParticleSystem hitParticles;

    private Quaternion _initialRotation;
        
    public bool Released { get; set; }

    public float Damage
    {
        get
        {
            return damage;
        }
        set{
            damage = value;
        }
    }

    private void OnEnable()
    {
        if (hitParticles != null)
            _initialRotation = hitParticles.transform.localRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Released || other.gameObject == owner || other.isTrigger) return;

        if (damagesEnemy && other.gameObject.CompareTag("Enemy")) 
        {
            other.gameObject.GetComponent<Enemy>().DamageHealth(damage);
        }
        
        if (damagesPlayer && other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerStatus>().DamageHealth(damage);
        }

        if (hitParticles != null)
        {
            hitParticles.Play();
            hitParticles.transform.SetParent(null);
            hitParticles.transform.position = transform.position;
            hitParticles.transform.rotation = transform.rotation * _initialRotation;
        }

        Released = true;
    }

    private void OnDestroy()
    {
        if (hitParticles != null && hitParticles.transform.parent != transform)
            Destroy(hitParticles);
    }
}
