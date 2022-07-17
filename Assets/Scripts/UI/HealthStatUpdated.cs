using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthStatUpdated : MonoBehaviour
{

    PlayerStatus playerStatus;

    [SerializeField]
    List<Transform> dieTransforms = new List<Transform>();

    [SerializeField] private Image dieImage;

    [SerializeField]
    List<Sprite> dieSprites = new List<Sprite>();

    bool relativTotalRatiosMode = false;

    //private float maxHealth;

    // Start is called before the first frame update
    void Awake()
    {
        playerStatus = FindObjectOfType<PlayerMovement>().GetComponent<PlayerStatus>();
        relativTotalRatiosMode = playerStatus.hitMode;
        playerStatus.OnHealthChanged.AddListener(HandleHealthChanged);
    }

    private void Start()
    {
        
    }

    private void HandleHealthChanged(float health)
    {
        // List<int> dieValues;
        // List<float> dieRatios;
        // CalculateHealthValues(health, playerStatus.maxHealth, dieTransforms.Count, out dieValues, out dieRatios);
        // UpdateVisuals(dieValues, dieRatios);

        float v = 1f - Mathf.Clamp01(health / playerStatus.maxHealth);
        dieImage.sprite = dieSprites[Mathf.FloorToInt(v * (dieSprites.Count - 1))];
    }

    // private void UpdateVisuals(List<int> dieValues, List<float> dieRatios)
    // {
    //     for(int i = dieTransforms.Count-1; i >= 0; --i)
    //     {
    //         int j = dieTransforms.Count - 1 - i;
    //         dieImages[i].sprite = dieSprites[dieValues[j] - 1];
    //         
    //         dieTransforms[i].localScale = new Vector3(dieTransforms[i].localScale.x,
    //                                                   dieRatios[j],
    //                                                   dieTransforms[i].localScale.z);
    //     }
    // }

    // private void CalculateHealthValues(float health, float maxHealth, int numDices, out List<int> dieValues, out List<float> dieRatios)
    // {
    //     dieValues = new List<int>();
    //     dieRatios = new List<float>();
    //     float steps = (6 * numDices);
    //     float stepSize = maxHealth / steps;
    //     int healthIntValue = (int) ((health - 0.01f) / stepSize) + 1;
    //     float healthFloatValue = (health - ((healthIntValue - 1) * stepSize)) / stepSize;
    //     //Debug.Log(healthIntValue + ", " + healthFloatValue);
    //
    //     for (int i = 0; i < numDices; ++i)
    //     {
    //         if(healthIntValue > 6)
    //         {
    //             dieValues.Add(6);
    //             dieRatios.Add(1f);
    //             healthIntValue -= 6;
    //         }
    //         else if(healthIntValue != -1)
    //         {
    //             dieValues.Add(healthIntValue);
    //             if (!relativTotalRatiosMode)
    //             {
    //                 dieRatios.Add(Mathf.Max(0f, healthFloatValue));
    //             }
    //             else
    //             {
    //                 dieRatios.Add(Mathf.Max(0f, (healthIntValue -1) * 1f / 5f));
    //             }
    //             healthIntValue = -1;
    //         }
    //         else
    //         {
    //             dieValues.Add(1);
    //             dieRatios.Add(0f);
    //         }
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        
    }
}
