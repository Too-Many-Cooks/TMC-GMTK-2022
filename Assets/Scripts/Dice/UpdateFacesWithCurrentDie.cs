using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateFacesWithCurrentDie : MonoBehaviour
{
    [SerializeField]
    List<Image> dieFaceImages;

    [SerializeField]
    TextMeshProUGUI powerLevelDisplay;

    // Better: only update die switched
    void FixedUpdate()
    {
        Die currentDie = FindObjectOfType<PlayerStatus>(true).gameObject.GetComponent<ShooterController>().CurrentReloadDie;
        for(int i = 0; i < dieFaceImages.Count; ++i)
        {
            dieFaceImages[i].sprite = currentDie.faces[i].sprite;
        }
        powerLevelDisplay.text = "(Power Level " + currentDie.powerLevel.ToString() + ")";
    }
}
