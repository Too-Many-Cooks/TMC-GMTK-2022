using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateFacesWithNewDie : MonoBehaviour
{
    [SerializeField]
    List<Image> dieFaceImages;

    [SerializeField]
    DieDisplay dieDisplay;

    [SerializeField]
    TextMeshProUGUI powerLevelDisplay;

    // Better: only update die switched
    void Start()
    {
        for (int i = 0; i < dieFaceImages.Count; ++i)
        {
            dieFaceImages[i].sprite = dieDisplay.Die.faces[i].sprite;
        }
        powerLevelDisplay.text = "(Power Level " + dieDisplay.Die.powerLevel.ToString() + ")";
    }
}
