using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Die", menuName = "Gameplay/Dice/Die", order = 1)]
public class Die : ScriptableObject
{
    public GameObject prefab; 
    public DieFace[] faces;
    public Texture atlas;
    public int atlasSize = 4;
    public int uvChannel = 1;
    public bool randomizeFaces = false;
    
    public int Sides => faces.Length;


    protected readonly Dictionary<int, Vector3> _normals = new();

    public virtual DieTextureBuilder Instantiate(Transform parent = null)
    {
        GameObject instance = GameObject.Instantiate(prefab, parent);
        
        DieTextureBuilder builder = instance.GetComponent<DieTextureBuilder>();
        builder.SetDie(this);

        return builder;
    }

    public DieFace Roll(out int index)
    {
        index = Random.Range(0, faces.Length);
        return faces[index];
    }

    public DieFace Roll()
    {
        int index;
        return Roll(out index);
    }

    public int RollIndex()
    {
        int index;
        Roll(out index);
        return index;
    }

    public virtual Vector3 GetNormal(int id)
    {
        return _normals.TryGetValue(id, out Vector3 value) ? value : default;
    }

    public virtual int FindFace(Vector3 normal)
    {
        float bestDot = 0f;
        int bestId = -1;

        foreach (var (id, _normal) in _normals)
        {
            float dot = Vector3.Dot(_normal, normal);
            if (bestId != -1 && dot < bestDot) continue;
            
            bestId = id;
            bestDot = dot;
        }

        return bestId;
    }

    protected virtual void OnEnable()
    {
        BakePrefabs();
    }

    private void BakePrefabs()
    {
        if (!prefab.TryGetComponent(out MeshFilter meshFilter)) return;

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null) return;
        
        List<Vector2> uvs = new();
        // List<Vector3> vertices = new();
        List<Vector3> normals = new();
      
        mesh.GetNormals(normals);
        // mesh.GetVertices(vertices);
        mesh.GetUVs(uvChannel, uvs);
      
        Dictionary<int, List<Vector3>> grids = new();
      
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            // Vector3 vertex = vertices[i];
            Vector3 normal = normals[i];
            Vector2 uv = uvs[i];
            
            int gridIndex = (int)(Mathf.Floor(uv.x * atlasSize) + Mathf.Floor((1f - uv.y) * atlasSize) * atlasSize);

            if (!grids.TryGetValue(gridIndex, out var grid))
            {
                grid = new();
                grids.Add(gridIndex, grid);
            }
            
            grid.Add(normal);
        }

        foreach (var (id, grid) in grids)
        {
            Vector3 average = grid.Aggregate(Vector3.zero, (current, normal) => current + normal);

            average /= grid.Count;
            average.Normalize();
            
            _normals.Add(id, average);
        }
    }
}
