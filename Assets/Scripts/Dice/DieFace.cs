using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicDieFace", menuName = "Gameplay/Dice/Face/Basic", order = 2)]
public class DieFace : ScriptableObject
{
    public Sprite sprite;

    public virtual void Use(GameObject owner)
    {
        
    }
}
