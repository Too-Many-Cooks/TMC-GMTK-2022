using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public sealed class MeshRendererOptimizer : MonoBehaviour
{
	public Vector3Int size = new(10, 10, 1);

	private void Start()
	{
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshRenderer == null) return;

		Mesh mesh = meshFilter.sharedMesh;

		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			IEnumerable<Mesh> subMeshes = mesh.Divide(size, i, 0);

			foreach (Mesh subMesh in subMeshes)
			{
				GameObject subObject = new("SubCollider");
				subObject.transform.SetParent(transform);
				subObject.transform.localPosition = Vector3.zero;
				subObject.transform.localRotation = Quaternion.identity;
				subObject.layer = gameObject.layer;
				subObject.isStatic = gameObject.isStatic;

				MeshFilter subFilter = subObject.AddComponent<MeshFilter>();
				MeshRenderer subRenderer = subObject.AddComponent<MeshRenderer>();
				subFilter.sharedMesh = subMesh;
				subRenderer.sharedMaterial = meshRenderer.sharedMaterials[i];
			}
		}
		
		Destroy(meshRenderer);
		Destroy(meshFilter);
		Destroy(this);
	} 
}
