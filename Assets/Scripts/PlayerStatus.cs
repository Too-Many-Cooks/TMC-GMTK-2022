using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField]
    float _health;
    bool _isDead = false;

    public float Health
    {
        get
        {
            return _health;
        }
    }

    public void DamageHealth(float damage)
    {
        if (_isDead)
            return;
        _health -= damage;
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
}
