using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    float spawningDelay = 5f;
    [SerializeField]
    float spawningDelayReduction = 0.95f;
    [SerializeField]
    List<GameObject> enemyPrefabs = new List<GameObject>();

    Vector3[] navMeshBounds = new Vector3[2];

    // Start is called before the first frame update
    void Start()
    {
        AnalyzeNavMeshBounds();

        StartCoroutine(SpawnCoroutine());
        //StartCoroutine(SpawnDelayUpdaterCoroutine());
    }

    private void AnalyzeNavMeshBounds()
    {
        NavMeshTriangulation navMeshTriangulation = NavMesh.CalculateTriangulation();
        navMeshBounds[0] = navMeshTriangulation.vertices[0];
        navMeshBounds[1] = navMeshTriangulation.vertices[0];
        foreach(Vector3 vertex in navMeshTriangulation.vertices)
        {
            navMeshBounds[0].x = Mathf.Min(vertex.x, navMeshBounds[0].x);
            navMeshBounds[1].x = Mathf.Max(vertex.x, navMeshBounds[1].x);

            navMeshBounds[0].y = Mathf.Min(vertex.y, navMeshBounds[0].y);
            navMeshBounds[1].y = Mathf.Max(vertex.y, navMeshBounds[1].y);

            navMeshBounds[0].z = Mathf.Min(vertex.z, navMeshBounds[0].z);
            navMeshBounds[1].z = Mathf.Max(vertex.z, navMeshBounds[1].z);
        }
    }

    /*private IEnumerator SpawnDelayUpdaterCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            spawningDelay *= spawningDelayReductionPerSecond;
        }
    }*/

    private IEnumerator SpawnCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(spawningDelay);
            SpawnEnemy();
            spawningDelay *= spawningDelayReduction;
        }
    }

    private void SpawnEnemy()
    {
        GameObject randomEnemy = SelectRandomEnemy();

        Vector3 randomPositionOnNavMesh;
        do
        {
            while (!GetRandomNavMeshPosition(out randomPositionOnNavMesh)) ;
        } while (CouldPlayerSeeEnemy(randomEnemy, randomPositionOnNavMesh));

        PlaceEnemy(randomEnemy, randomPositionOnNavMesh);
    }

    private GameObject SelectRandomEnemy()
    {
        return enemyPrefabs[(int)UnityEngine.Random.Range(0f, enemyPrefabs.Count-float.Epsilon)]; // -Epsilon avoids selecting enemyPrefabs.Count
    }

    private bool GetRandomNavMeshPosition(out Vector3 position)
    {
        Vector3 randomRelativePosition = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        Vector3 randomPosInBounds = new Vector3();
        for(int i = 0; i < 3; ++i)
        {
            randomPosInBounds[i] = navMeshBounds[0][i] + (navMeshBounds[1][i] - navMeshBounds[0][i]) * randomRelativePosition[i];
        }
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPosInBounds, out hit, float.PositiveInfinity, NavMesh.AllAreas);
        position = hit.position;
        return hit.hit;
    }

    private bool CouldPlayerSeeEnemy(GameObject randomEnemy, Vector3 randomPositionOnNavMesh)
    {
        Collider enemyCollider = randomEnemy.GetComponent<Collider>();
        Vector3 playerPosition = Camera.main.transform.position;

        bool isHidden = false;
        Vector3[] bounds = new Vector3[2] { enemyCollider.bounds.min * 0.9f, enemyCollider.bounds.max * 0.9f };
        List<Vector3> rayDirections = new List<Vector3>();
        for(int i = 0; i < 8; ++i)
        {
            Vector3 boundsPos = new Vector3();
            int remainder = i;
            
            boundsPos.x = bounds[i % 2].x;
            remainder /= 2;

            boundsPos.y = bounds[i % 2].y;
            remainder /= 2;

            boundsPos.z = bounds[i % 2].z;

            rayDirections.Add(transform.TransformDirection(-transform.InverseTransformPoint(playerPosition - randomPositionOnNavMesh) + boundsPos));
        }

        foreach(Vector3 dir in rayDirections)
        {
            RaycastHit hit;

            LayerMask nonPlayerLayerMask = ~LayerMask.GetMask("Player");

            isHidden |= Physics.Raycast(playerPosition, dir , out hit, dir.magnitude, nonPlayerLayerMask);

        }

        if (isHidden)
        {
            foreach (Vector3 dir in rayDirections)
            {
                Debug.DrawRay(playerPosition, dir, Color.red, 10000f);
            }
        }
        return !isHidden;
    }

    private void PlaceEnemy(GameObject randomEnemy, Vector3 randomPositionOnNavMesh)
    {
        Instantiate(randomEnemy, randomPositionOnNavMesh, new Quaternion(), transform);
    }

}
