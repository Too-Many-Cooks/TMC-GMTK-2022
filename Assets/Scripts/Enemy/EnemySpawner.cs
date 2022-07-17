using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public Enemy[] enemyPrefabs;
    public LayerMask layerMask;
    public float spawningDelay = 5f;
    public float minSpawnDelay = 2f;
    public float spawningDelayReduction = 0.95f;
    public float minDistance = 100f;
    public int preloadEnemies = 10;
    public int maxEnemies = 200;
    public int batchSize = 3;
    public int overflow = 32;

    private readonly Vector3[] navMeshBounds = new Vector3[2];
    private readonly HashSet<Enemy> _enemies = new();
    private readonly Vector3[] colliderBounds =
    {
        Vector3.up + Vector3.right,
        Vector3.up + Vector3.left,
        Vector3.up + Vector3.forward,
        Vector3.up + Vector3.right,
        Vector3.down + Vector3.right,
        Vector3.down + Vector3.left,
        Vector3.down + Vector3.forward,
        Vector3.down + Vector3.right,
    };
    private PlayerMovement _player;

    // Start is called before the first frame update
    void Start()
    {
        AnalyzeNavMeshBounds();
        StartCoroutine(SpawnCoroutine());
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
        yield return null;
        
        for (int i = 0; i < preloadEnemies; i++)
            SpawnEnemy();
        
        while(true)
        {
            yield return new WaitForSeconds(spawningDelay);

            for (int i = 0; i < batchSize; i++)
                if (!SpawnEnemy())
                    break;
            
            spawningDelay = Mathf.Max(spawningDelay * spawningDelayReduction, minSpawnDelay);
        }
    }

    private bool SpawnEnemy()
    {
        if (_enemies.Count >= maxEnemies)
            return false;
        
        if (_player == null)
        {
            _player = FindObjectOfType<PlayerMovement>();
            if (_player == null) return false;
        }

        Enemy enemy = SelectRandomEnemy();
        Bounds bounds =
            enemy.TryGetComponent(out CapsuleCollider c) ?
            new Bounds(c.center, new Vector3(c.radius, c.height * 0.5f, c.radius)) :
            default;

        for (int i = 0; i < overflow; i++)
        {
            bool hasPosition = GetRandomNavMeshPosition(out Vector3 randomPosition);
            if (!hasPosition) continue;
            
            bool isVisible = IsVisible(randomPosition, _player.transform.position, bounds);
            if (isVisible) continue;
            
            PlaceEnemy(enemy, randomPosition);
            
            return true;
        }

        return false;
    }

    private Enemy SelectRandomEnemy()
    {
        return enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Length)];
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

    private bool IsVisible(Vector3 from, Vector3 to, Bounds bounds)
    {
        if (Vector3.Distance(from, to) > minDistance)
            return true;

        Vector3 dir = to - from;
        
        foreach (Vector3 x in colliderBounds)
        {
            Vector3 origin = from + Vector3.Scale(x, bounds.size) + bounds.center;
            bool didHit = Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dir.magnitude, layerMask); 
            // Debug.DrawRay(origin, didHit ? dir.normalized * hit.distance : dir, didHit ? Color.green * 0.5f : Color.red, 10f);
            
            if (!didHit)
                return true;
        }

        return false;
    }

    private void PlaceEnemy(Enemy enemy, Vector3 randomPositionOnNavMesh)
    {
        Enemy instance = Instantiate(enemy, randomPositionOnNavMesh, Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up), transform);
        instance.OnDeath += OnEnemyDeath;
        instance.OnDestroyed += OnEnemyDeath;
        _enemies.Add(instance);
    }

    private void OnEnemyDeath(Enemy obj)
    {
        _enemies.Remove(obj);
    }
}
