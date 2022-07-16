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
            _dieObject = _die.Instantiate(transform);
        }
    }

    public Die die;
    public Vector3 rotationRate;
    public bool rotateInWorldSpace = true;
    
    [SerializeField, HideInInspector] private Die _die;
    [SerializeField] private GameObject _dieObject;

    // Start is called before the first frame update
    private void Start()
    {
        if (_dieObject != null)
            Destroy(_dieObject);

        Die = die;
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

    public void OnValidate()
    {
        if (Application.isPlaying)
        {
            Die = die;
        }
    }
}
