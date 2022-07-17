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
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Despawn(lifetime));
    }

    IEnumerator Despawn(float lifetime) 
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return;

        if (damagesEnemy && other.gameObject.CompareTag("Enemy")) 
        {
            other.gameObject.GetComponent<Enemy>().DamageHealth(damage);
        }
        if (damagesPlayer && other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerStatus>().DamageHealth(damage);
        }
        if ((damagesPlayer || !other.gameObject.CompareTag("Player")) && (damagesEnemy || !other.gameObject.CompareTag("Enemy")))
        {
            Destroy(gameObject);
        }
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
