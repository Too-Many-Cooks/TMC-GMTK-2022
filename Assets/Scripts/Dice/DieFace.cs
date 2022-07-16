using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Die Face", menuName = "Gameplay/Dice/Face", order = 2)]
public class DieFace : ScriptableObject
{
    public Sprite sprite;

    public virtual void Use(GameObject owner)
    {
        
    }
}
