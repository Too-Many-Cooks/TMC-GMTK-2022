using System;
using System.Collections.Generic;
using UnityEngine;

internal class DieSpawner : MonoBehaviour
{
    [SerializeField]
    List<DieFace> possibleDieFaces;

    List<Die> createdDice = new List<Die>();

    [SerializeField]
    GameObject grabbableDieDisplayPrefab;

    [SerializeField]
    GameObject prefab;
    [SerializeField]
    Texture atlas;
    [SerializeField]
    int atlasSize = 4;
    [SerializeField]
    int uvChannel = 1;


    internal void SpawnRandomDice()
    {
        //Currently only supports D6
        DieFace[] faces = new DieFace[6];
        for(int i = 0; i < 6; ++i)
        {
            int randomIndex = (int)UnityEngine.Random.Range(0f, possibleDieFaces.Count - float.Epsilon);
            faces[i] = possibleDieFaces[randomIndex];
        }

        Die newDie = Die.CreateDie(prefab, faces, atlas, atlasSize, uvChannel);

        GameObject dieDisplay = Instantiate(grabbableDieDisplayPrefab);
        dieDisplay.transform.position = transform.position + new Vector3(0, 1.5f, 0);
        dieDisplay.GetComponent<DieDisplay>().Die = newDie;

        Debug.Log("Spawned a dice");
    }
}