using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[RequireComponent(typeof(MeshRenderer))]
public class DieTextureBuilder : MonoBehaviour
{
   [Serializable]
   public class KeywordNames
   {
      public string gridSize = "_GridSize";
      public string atlas = "_Atlas";
      public string remap = "_Remap";
   }

   public KeywordNames keywords;
   public int _gridSize = 4;
   public Texture atlas;
   public Sprite[] testSprites;
   
   protected Material _material;
   protected Mesh _mesh;
   [SerializeField] protected Texture2D _remap;
   
   protected void Awake()
   {
      MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
      MeshFilter meshFilter = GetComponent<MeshFilter>();
      
      _mesh = meshFilter.sharedMesh;
      _material = meshRenderer.sharedMaterial;
      _remap = new(_gridSize, _gridSize, TextureFormat.ARGB32, false)
      {
         filterMode = FilterMode.Point,
         alphaIsTransparency = true,
         wrapMode = TextureWrapMode.Clamp,
      };
      
      _material.SetInt(keywords.gridSize, _gridSize);
      _material.SetTexture(keywords.atlas, atlas);
      _material.SetTexture(keywords.remap, _remap);
      
      for (int i = 0; i < testSprites.Length; i++)
      {
         SetFace(i, testSprites[i]);
      }
   }

   protected void Bake()
   {
      // List<Vector2> uvs = new();
      // List<Vector3> vertices = new();
      // List<Vector3> normals = new();
      //
      // _mesh.GetNormals(normals);
      // _mesh.GetVertices(vertices);
      // _mesh.GetUVs(uvChannel, uvs);
      //
      // Dictionary<int, List<Vector3>> grids = new();
      //
      // for (int i = 0; i < _mesh.vertexCount; i++)
      // {
      //    Vector3 vertex = vertices[i];
      //    Vector3 normal = normals[i];
      // }
   }

   public int NormalToId()
   {
      return 0;
   }
   
   public void SetFace(int id, Sprite sprite)
   {
      int x = id % _gridSize;
      int y = id / _gridSize;

      Rect rect = sprite.rect;
      rect.width /= atlas.width;
      rect.height /= atlas.height;
      rect.x /= atlas.width;
      rect.y /= atlas.height;
      
      Debug.Log($"{x}, {y}: {rect}");
      
      _remap.SetPixel(x, y, new(rect.x, rect.y, rect.width, rect.height));
      _remap.Apply();
   }
}
