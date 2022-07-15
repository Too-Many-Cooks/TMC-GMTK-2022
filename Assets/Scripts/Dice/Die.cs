using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Die", menuName = "Gameplay/Dice/Die", order = 1)]
public class Die : ScriptableObject
{
    public DieFace[] faces;
    public int NumberOfSides { get { return faces.Length; } }
    
    public DieFace Roll()
    {
        int index = Random.Range(0, faces.Length);
        return faces[index];
    }
}
