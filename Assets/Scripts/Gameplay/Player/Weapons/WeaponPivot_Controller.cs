using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPivot_Controller : MonoBehaviour
{
    Vector3 originalPos;    // The original position of the weaponPivot.

    [SerializeField] Vector3 oscillationPosDiff = new Vector3(0, -1, 0);    // How much the weaponPivot will displace.
    [SerializeField] Vector3 changingWeaponsPosDiff = new Vector3(0, -5, 0);    // How much the weaponPivot will displace.

    [SerializeField] [Range(0.1f, 6)] float oscillationDuration = 1;     // How much each oscillation should last.
    [SerializeField] [Range(0.1f, 6)] float changingWeaponsDuration = 1;     // How much each oscillation should last.

    float oscillationTimer = 0;
    bool oscillationDir = true, changingWeapons = false;
    
    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.localPosition;
    }

    private void Update()
    {
        UpdateTimer();

        if(!changingWeapons)
            transform.localPosition = Vector3.Lerp(originalPos, originalPos + oscillationPosDiff, oscillationTimer / oscillationDuration);
        else
            transform.localPosition = Vector3.Lerp(originalPos, originalPos + changingWeaponsPosDiff, oscillationTimer / 
                (changingWeaponsDuration/2));
    }


    // Updates our oscillation timer.
    private void UpdateTimer()
    {   
        if (!changingWeapons)
        {
            if (oscillationDir)
            {
                oscillationTimer += Time.deltaTime;

                if (oscillationTimer >= oscillationDuration) // Changing the orientation of the movement if the timer completes.
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
        else
        {
            if (oscillationDir)
            {
                oscillationTimer += Time.deltaTime;

                if (oscillationTimer >= changingWeaponsDuration/2) // Changing the orientation of the movement if the timer completes.
                {
                    oscillationTimer = changingWeaponsDuration/2;
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
                    changingWeapons = false;
                }
            }
        }
    }


    // Function called when weapons need to be changed.
    public void ChangeWeapons()
    {
        // If we weren't changing weapons when the function was called, we start the movement from start.
        if(changingWeapons == false)
        {
            changingWeapons = true;
            oscillationDir = true;
            oscillationTimer = 0;
        }
        else
        {
            oscillationDir = true;
            oscillationTimer = oscillationDuration / 2 - oscillationTimer;
        }
    }
}
