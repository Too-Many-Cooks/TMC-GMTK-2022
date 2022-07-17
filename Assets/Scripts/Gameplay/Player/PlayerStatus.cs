using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour
{
    float _health;
    public float maxHealth;
    bool _isDead = false;
    bool _fadingToBlack = false;
    Texture2D blk;

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

        yield return new WaitForSeconds(1f);

        
        //bool fade;
        float alpha = 0f;
    
        //make a tiny black texture
        blk = new Texture2D(1, 1);
        blk.SetPixel(0, 0, new Color(0, 0, 0, 0));
        blk.Apply();

        _fadingToBlack = true;
        
        float fadeDuration = 1.5f;
        while(alpha < 1f)
        {
            blk.SetPixel(0, 0, new Color(0, 0, 0, alpha));
            blk.Apply();

            alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }

        SceneManager.LoadScene("GameOver");
    }

    private void OnGUI()
    {
        if(_fadingToBlack)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blk);
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
