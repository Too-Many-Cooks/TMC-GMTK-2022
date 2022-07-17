using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KeyContainer : MonoBehaviour
{
    public static readonly HashSet<string> keys = new();

    public event Action<KeyContainer, bool> OnActivated; 

    public string id = "default key";
    public GameObject key;
    public bool activateOnce = true;

    [SerializeField, HideInInspector] protected bool _initialState;

    public bool Active
    {
        get => key.activeSelf;
        set
        {
            if (value == Active) return;
            
            key.SetActive(value);
            OnActivated?.Invoke(this, value);
        }
    }

    public bool HasKey
    {
        get => keys.Contains(id);
        set
        {
            if (value)
                keys.Add(id);
            else
                keys.Remove(id);
        }
    }
    
    public void Activate()
    {
        switch (Active)
        {
            case true when !HasKey:
                HasKey = true;
                Active = false;
                break;
            case false when HasKey:
                HasKey = false;
                Active = true;
                break;
        }
    }
    
    protected void Awake()
    {
        _initialState = Active;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out PlayerMovement _)) return;
        
        if (!activateOnce || _initialState == Active)
            Activate();
    }

    [ContextMenu("Activate")]
    private void Context_Activate()
    {
        Active = !Active;
    }
}
