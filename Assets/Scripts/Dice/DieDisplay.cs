using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieDisplay : MonoBehaviour
{
    public Die Die
    {
        get => _die;
        set
        {
            if (value == null)
                return;
            
            if (_dieObject != null)
                Destroy(_dieObject);
            
            _die = value;
            _builder = _die.Instantiate();
            _dieObject = _builder.gameObject;
            _dieObject.transform.SetParent(transform, false);
            
            OnSetDie?.Invoke(this, _dieObject);
        }
    }

    public event Action<DieDisplay, GameObject> OnSetDie; 
    
    public Vector3 rotationRate;
    public bool rotateInWorldSpace = true;

    [SerializeField] private Die _die;
    [SerializeField, HideInInspector] public DieTextureBuilder _builder;
    [SerializeField, HideInInspector] private GameObject _dieObject;

    private void Start()
    {
        if (_dieObject != null)
            Destroy(_dieObject);

        Die = _die;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_dieObject != null)
        {
            _dieObject.transform.Rotate(rotationRate * Time.deltaTime, rotateInWorldSpace ? Space.World : Space.Self);
        }
    }

    public void SetSides(int sides)
    {
        DieManager manager = DieManager.Instance;
        if (manager == null) return;

        Die = manager.FindDie(sides);
    }
    
    public DieFace FindFace(Vector3 direction)
    {
        direction = _dieObject.transform.InverseTransformDirection(direction);
        int id = _die.FindFace(direction);
        return _builder.GetFace(id);
    }
}
