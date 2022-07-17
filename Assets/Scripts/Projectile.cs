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

    public bool explodes = false;
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

        Released = true;
    }

    public void Explode()
    {
        if (!explodes) { return; }
        //play animation
        gameObject.GetComponent<ParticleSystem>()?.Play();
        gameObject.GetComponent<AudioSource>()?.Play();
        //destroy
        //damage stuff around
        //destroy

    }
}
