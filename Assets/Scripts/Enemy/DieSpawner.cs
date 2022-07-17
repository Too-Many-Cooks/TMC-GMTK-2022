using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

internal class DieSpawner : MonoBehaviour
{
    [Serializable]
    public struct DieFaceProperties
    {
        public DieFace dieFace;
        public float weight; // Higher weight -> higher likelyhood of occurring
        public int buffValue; // From -10 (large debuff) to +10 (large buff); Sum of buffValues will match die powerLevel
    }

    [SerializeField]
    private List<DieFaceProperties> dieFaceProperties;

    [Serializable]
    public struct PowerLevelProperties
    {
        public int powerLevel;
        public float weight;
    }

    [SerializeField]
    List<PowerLevelProperties> powerLevels = new List<PowerLevelProperties>();

    /*[SerializeField]
    int[] minMaxDiePowerLevel = new int[2];
    [SerializeField]
    float zeroPowerDieProbability = 0.8f;
    [SerializeField]
    float zeroPowerDieProbability = 0.8f;
    [SerializeField]*/



    //[SerializeField]
    //List<DieFace> possibleDieFaces;

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

    [SerializeField]
    static int dicePoolSize = 20;


    static Dictionary<int, List<Die>> powerLevelDicePools = new Dictionary<int, List<Die>>();

    private void Start()
    {
        if(powerLevelDicePools.Count == 0)
            FillDicePools();
    }

    private void FillDicePools()
    {
        foreach(var powerLevel in powerLevels)
        {
            powerLevelDicePools[powerLevel.powerLevel] = new List<Die>();
        }

        float weightSum = 0f;
        foreach(DieFaceProperties dieFacePropertie in dieFaceProperties)
        {
            weightSum += dieFacePropertie.weight;
        }

        // Might result in endless loop! Be carefull
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        while(!AreDicePoolsFilled() && stopwatch.ElapsedMilliseconds < 60000)
        {
            //Create random die
            int diePowerLevel = 0;

            DieFace[] faces = new DieFace[6];
            for (int i = 0; i < 6; ++i)
            {
                // Choose faces
                DieFace chosenDieFace = null;
                float random = UnityEngine.Random.value * weightSum;
                for (int j = 0; j < dieFaceProperties.Count; ++j)
                {
                    if (random <= dieFaceProperties[j].weight)
                    {
                        chosenDieFace = dieFaceProperties[j].dieFace;
                        diePowerLevel += dieFaceProperties[j].buffValue;
                        break;
                    }
                    else
                    {
                        random -= dieFaceProperties[j].weight;
                    }
                }
                faces[i] = chosenDieFace;
            }

            // If die is valid, save it
            if (powerLevelDicePools.ContainsKey(diePowerLevel) && powerLevelDicePools[diePowerLevel].Count < dicePoolSize)
            {
                Die newDie = Die.CreateDie(prefab, faces, atlas, atlasSize, uvChannel);
                powerLevelDicePools[diePowerLevel].Add(newDie);
            }
        }
        stopwatch.Stop();

        if(!AreDicePoolsFilled())
        {
            Debug.LogWarning("Dice pools could not be filled in time.");
        }
        else
        {
            Debug.Log("Dice pools created in " + stopwatch.ElapsedMilliseconds/1000f + " seconds");
        }
    }

    private bool AreDicePoolsFilled()
    {
        foreach(KeyValuePair<int, List<Die>> p in powerLevelDicePools)
        {
            if (p.Value.Count < 20)
                return false;
        }
        return true;
    }

    internal void SpawnRandomDice()
    {
        // Balance power level distribution
        float probalitySum = 0f;
        foreach(PowerLevelProperties p in powerLevels)
        {
            probalitySum += p.weight;
        }

        /*for(int i = 0; i < powerLevels.Count; ++i)
        {
            powerLevels[i].probability /= probalitySum;
        }*/

        int diePowerLevel = 0;
        float random = UnityEngine.Random.value * probalitySum;
        for(int i = 0; i < powerLevels.Count; ++i)
        {
            if(random <= powerLevels[i].weight)
            {
                diePowerLevel = powerLevels[i].powerLevel;
                break;
            }
            else
            {
                random -= powerLevels[i].weight;
            }
        }

        /*float currentBuffValueSum = 0f;
        List<float> outcomeDistribution;
        CalculateProbabilites(dieFaceProperties, currentBuffValueSum, out outcomeDistribution);

        //Currently only supports D6
        DieFace[] faces = new DieFace[6];
        for(int i = 0; i < 6; ++i)
        {
            //int randomIndex = (int)UnityEngine.Random.Range(0f, possibleDieFaces.Count - float.Epsilon);
            int randomIndex = (int)UnityEngine.Random.Range(0f, 1f);
            faces[i] = possibleDieFaces[randomIndex];
        }

        Die newDie = Die.CreateDie(prefab, faces, atlas, atlasSize, uvChannel);*/

        int randomIndex = (int)(UnityEngine.Random.value * powerLevelDicePools[diePowerLevel].Count - float.Epsilon);
        Die chosenDie = powerLevelDicePools[diePowerLevel][randomIndex];

        GameObject dieDisplay = Instantiate(grabbableDieDisplayPrefab);
        dieDisplay.transform.position = transform.position + new Vector3(0, 1.5f, 0);
        dieDisplay.GetComponent<DieDisplay>().Die = chosenDie;

        Debug.Log("Spawned a dice");
    }
}