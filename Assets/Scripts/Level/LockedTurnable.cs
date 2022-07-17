using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LockedTurnable : MonoBehaviour
{
    public Vector3 activeRotation;
    public Vector3 inactiveRotation;
    public KeyContainer keyContainer;
    public float duration = 4f;
    public UnityEvent onLock;
    public UnityEvent onUnlock;
    public UnityEvent onMove;

    protected bool _isActive;
    
    public bool IsRotating { get; protected set; }
    
    protected void OnEnable()
    {
        if (keyContainer != null)
        {
            keyContainer.OnActivated += OnKeyActivated;
            _isActive = keyContainer.Active;
            transform.rotation = GetTargetRotation(_isActive);
        }
    }

    protected Quaternion GetTargetRotation(bool value)
    {
        return Quaternion.Euler(value ? activeRotation : inactiveRotation);
    }

    protected IEnumerator Rotate()
    {
        IsRotating = true;
        float startTime = Time.time;
        bool isActive = _isActive;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = GetTargetRotation(isActive);
        
        onMove?.Invoke();
        
        while (Time.time - startTime < duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, Ease((Time.time - startTime) / duration));
            yield return null;
        }

        if (isActive)
            onUnlock?.Invoke();
        else
            onLock?.Invoke();

        IsRotating = false;
    }

    private void OnKeyActivated(KeyContainer keyContainer, bool value)
    {
        _isActive = value;
        StartCoroutine(Rotate());
    }

    private static float Ease(float x)
    {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
}
