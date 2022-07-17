using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboDieFace", menuName = "Gameplay/Dice/Face/Combination", order = 4)]
public class MultiFunctionDieFace : DieFace
{
    public DieFace[] faces;

    public override void Use(GameObject owner)
    {
        base.Use(owner);
        for(int i = 0; i < faces.Length; i++)
        {
            faces[i].Use(owner);
        }
    }
}
