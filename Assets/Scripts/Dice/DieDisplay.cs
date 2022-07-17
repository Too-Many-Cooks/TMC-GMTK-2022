using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
            _builder = _die.Instantiate(transform);
            _dieObject = _builder.gameObject;
            
            OnSetDie?.Invoke(this, _dieObject);
        }
    }

    public Vector3 rotationRate;
    public bool rotateInWorldSpace = true;
    public float peakRollSpeed = 1800.0f;
    public bool isRolling = false;
    public Quaternion rollingTo;
    private Quaternion randomRoll;
    private Quaternion directedRoll;
    public float timeRolling = 0.0f;
    public float rollDuration = 3.0f;
    public float randomRollPower = 4f;
    public float randomRollChangeRate = 36f;
    public float slowDuration = 0.5f;
    private Quaternion directedRollRotation;
    private Quaternion randomRollRotation;

    public event Action<DieDisplay, GameObject> OnSetDie; 

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
            if(isRolling)
            {
                if(timeRolling == rollDuration)
                {
                    _dieObject.transform.localRotation = rollingTo;
                    isRolling = false;
                    timeRolling = 0.0f;
                } else
                {
                    if (timeRolling == 0.0f)
                    {
                        randomRoll = Quaternion.RotateTowards(Quaternion.identity, Random.rotation, 1.0f);
                        rotationRate = Vector3.zero;
                        directedRollRotation = _dieObject.transform.localRotation;
                        randomRollRotation = _dieObject.transform.localRotation;
                        directedRoll = Quaternion.RotateTowards(Quaternion.identity, Quaternion.Inverse(_dieObject.transform.localRotation) * rollingTo, 1.0f);
                        var directedAngle = Quaternion.Angle(directedRoll, Quaternion.identity);
                        if(directedAngle < 0.01f)
                        {
                            directedRoll = randomRoll;
                        } else if (directedAngle < 0.99f)
                        {
                            directedRoll = Quaternion.SlerpUnclamped(Quaternion.identity, directedRoll, 1.0f / directedAngle);
                        }
                    }

                    var randomRollSpeed = peakRollSpeed;
                    var directedRollSpeed = peakRollSpeed;
                    if (timeRolling > rollDuration - slowDuration)
                    {
                        randomRollSpeed *= 1.0f - (timeRolling - rollDuration + slowDuration) / slowDuration;

                        if (rollDuration - timeRolling > 0)
                        {
                            var naturalDistance = randomRollSpeed / 2.0f * (rollDuration - timeRolling);
                            var fullSpins = Mathf.Floor(naturalDistance / 360.0f);
                            var spinAdjust = Quaternion.Angle(directedRollRotation, rollingTo);
                            var underSpinDistance = fullSpins * 360.0f - spinAdjust;
                            var overSpinDistance = fullSpins * 360.0f + spinAdjust;
                            var underSpinStop = directedRollRotation * Quaternion.SlerpUnclamped(Quaternion.identity, directedRoll, underSpinDistance);
                            var overSpinStop = directedRollRotation * Quaternion.SlerpUnclamped(Quaternion.identity, directedRoll, overSpinDistance);

                            var underSpinError = Quaternion.Angle(underSpinStop, rollingTo);
                            var overSpinError = Quaternion.Angle(overSpinStop, rollingTo);

                            var desiredDistance = underSpinError < overSpinError ? underSpinDistance : overSpinDistance;

                            directedRollSpeed = desiredDistance * 2.0f / (rollDuration - timeRolling);
                        }
                        else
                        {
                            directedRollSpeed = 0.0f;
                        }
                    }
                    else
                    {
                        randomRollSpeed *= timeRolling / (rollDuration - slowDuration);
                        directedRollSpeed = randomRollSpeed;
                    }

                    randomRoll *= Quaternion.RotateTowards(Quaternion.identity, Random.rotation, Time.deltaTime * randomRollChangeRate * randomRollSpeed / peakRollSpeed);
                    randomRoll = Quaternion.RotateTowards(Quaternion.identity, randomRoll, 1.0f);

                    randomRollRotation = _dieObject.transform.localRotation * Quaternion.SlerpUnclamped(Quaternion.identity, randomRoll, randomRollSpeed * Time.deltaTime);
                    directedRollRotation *= Quaternion.SlerpUnclamped(Quaternion.identity, directedRoll, directedRollSpeed * Time.deltaTime);

                    _dieObject.transform.localRotation = Quaternion.Slerp(randomRollRotation, directedRollRotation, Mathf.Pow(timeRolling / rollDuration, randomRollPower));
                    timeRolling += Time.deltaTime;
                    if (timeRolling > rollDuration)
                    {
                        timeRolling = rollDuration;
                    }
                }
            } else
            {
                _dieObject.transform.Rotate(rotationRate * Time.deltaTime, rotateInWorldSpace ? Space.World : Space.Self);
            }
        }
    }

    public void StartIdleRotation(Vector3 rotationRate, bool rotateInWorldSpace = true)
    {
        this.rotationRate = rotationRate;
        this.rotateInWorldSpace = rotateInWorldSpace;
    }

    public void RollTo(Quaternion rotation, float rollDuration = -1.0f, float slowDuration = -1.0f, float randomRollPower = -1.0f)
    {
        if(_dieObject != null)
        {
            if(rollDuration > 0) this.rollDuration = rollDuration;
            if (randomRollPower > 0) this.randomRollPower = randomRollPower;
            if (slowDuration > 0) this.slowDuration = slowDuration;
            this.timeRolling = 0.0f;
            this.rollingTo = rotation;
            isRolling = true;
        }
    }

    public void RollTo(Vector3 euelerAngles, float rollDuration = -1.0f, float slowDuration = -1.0f, float randomRollPower = -1.0f)
    {
        RollTo(Quaternion.Euler(euelerAngles), rollDuration, slowDuration, randomRollPower);
    }

    public void RollTo(Vector3 forwards, Vector3 up, float rollDuration = -1.0f, float slowDuration = -1.0f, float randomRollPower = -1.0f)
    {
        RollTo(Quaternion.LookRotation(forwards, up), rollDuration, slowDuration, randomRollPower);
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
