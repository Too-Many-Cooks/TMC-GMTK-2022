using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Die", menuName = "Gameplay/Dice/Die", order = 1)]
public class Die : ScriptableObject
{
    public GameObject prefab; 
    public DieFace[] faces;
    public int Sides => faces.Length;

    public GameObject Instantiate()
    {
        return GameObject.Instantiate(prefab);
    }
    
    public DieFace Roll()
    {
        int index = Random.Range(0, faces.Length);
        return faces[index];
    }
}
