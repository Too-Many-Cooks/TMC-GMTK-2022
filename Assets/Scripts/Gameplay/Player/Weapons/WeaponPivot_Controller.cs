using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPivot_Controller : MonoBehaviour
{
    Vector3 originalPos;    // The original position of the weaponPivot.

    [SerializeField] Vector3 oscillationPosDiff = new Vector3(0, -1, 0);    // How much the weaponPivot will displace.

    [SerializeField] [Range(0.1f, 6)] float oscillationDuration = 1;     // How much each oscillation should last.

    float oscillationTimer = 0;
    bool oscillationDir = true;
    
    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.localPosition;
    }

    private void Update()
    {
        UpdateTimer();

        if (oscillationTimer == 0)
            transform.localPosition = originalPos;

        else if (oscillationTimer == oscillationDuration)
            transform.localPosition = originalPos + oscillationPosDiff + new Vector3(0, Random.Range(oscillationPosDiff.y/8, -oscillationPosDiff.y)/8, 0);
    }


    // Updates our oscillation timer.
    private void UpdateTimer()
    {
        if(oscillationDir)
        {
            oscillationTimer += Time.deltaTime;

            if(oscillationTimer >= oscillationDuration) // Changing the orientation of the movement if the timer completes.
            {
                oscillationTimer = oscillationDuration;
                oscillationDir = !oscillationDir;
            }
        }
        else
        {
            oscillationTimer -= Time.deltaTime;

            if (oscillationTimer <= 0)                  // Changing the orientation of the movement if the timer completes.
            {
                oscillationTimer = 0;
                oscillationDir = !oscillationDir;
            }
        }
    }
}
