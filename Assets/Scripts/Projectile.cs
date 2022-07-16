using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f;
    public float damage = 1f;
    
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) 
        {
            collision.gameObject.GetComponent<Enemy>().DamageHealth(damage);
        }
        Destroy(gameObject);
    }
}
