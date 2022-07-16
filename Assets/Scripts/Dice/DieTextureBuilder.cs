using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

   public static readonly System.Random s_random = new();
   public KeywordNames keywords;
   
   [SerializeField, HideInInspector] protected Material _material;
   [SerializeField, HideInInspector] protected Die _die;
   [SerializeField, HideInInspector] protected Texture2D _remap;

   protected readonly Dictionary<int, int> _faces = new();

   public int NormalToId()
   {
      return 0;
   }
   
   public void SetFace(int id, Sprite sprite)
   {
      if (_die == null || _die.atlas == null) return;
      
      int x = id % _die.atlasSize;
      int y = id / _die.atlasSize;

      Rect rect = sprite.rect;
      rect.width /= _die.atlas.width;
      rect.height /= _die.atlas.height;
      rect.x /= _die.atlas.width;
      rect.y /= _die.atlas.height;
      
      _remap.SetPixel(x, y, new(rect.x, rect.y, rect.width, rect.height));
      _remap.Apply();
   }

   public void SetDie(Die die)
   {
      _die = die;
      
      MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
      _material = meshRenderer.material;
      _remap = new(die.atlasSize, die.atlasSize, TextureFormat.ARGB32, false)
      {
         filterMode = FilterMode.Point,
         alphaIsTransparency = true,
         wrapMode = TextureWrapMode.Clamp,
      };
      
      _material.SetInt(keywords.gridSize, die.atlasSize);
      _material.SetTexture(keywords.atlas, die.atlas);
      _material.SetTexture(keywords.remap, _remap);
      
      _faces.Clear();
      
      List<int> indices = Enumerable.Range(0, die.Sides).ToList();

      for (int i = 0; i < die.Sides; i++)
      {
         int j = s_random.Next(0, indices.Count);
         int index = indices[j];
         indices.RemoveAt(j);
         
         DieFace face = die.faces[index];
         _faces.Add(i, index);
         SetFace(i, face.sprite);
      }
   }

   public DieFace GetFace(int id)
   {
      if (!_faces.TryGetValue(id, out int index)) return null;
      return _die.faces[index];
   }
}
