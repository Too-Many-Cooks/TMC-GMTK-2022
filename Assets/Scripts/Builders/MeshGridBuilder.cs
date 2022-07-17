using System.Collections.Generic;
using UnityEngine;

public static class MeshDivider
{
	public static IEnumerable<Mesh> Divide(this Mesh Mesh, Vector3Int gridSize, int subMesh = -1, int uvChannel = -1)
	{
		Vector3 size = Mesh.bounds.size;
		int yOffset = gridSize.x;
		int zOffset = gridSize.x + gridSize.y;
		
		List<Vector3> vertices = new();
		List<Vector3> normals = new();
		List<Vector2> uvs = new();
		Dictionary<int, List<int>> grids = new();

		Mesh.GetVertices(vertices);
		Mesh.GetNormals(normals);
		Mesh.GetUVs(uvChannel, uvs);
		int[] triangles = Mesh.GetTriangles(subMesh);
		
		// Cache the vertices by grid.
		for (int i = 0; i < triangles.Length; i += 3)
		{
			int triangle0 = triangles[i];
			int triangle1 = triangles[i + 1];
			int triangle2 = triangles[i + 2];
			
			Vector3 vertex0 = vertices[triangle0];
			Vector3 vertex1 = vertices[triangle1];
			Vector3 vertex2 = vertices[triangle2];
			Vector3 center = (vertex0 + vertex1 + vertex2) / 3f; 
			
			Vector3Int grid = new(
				(int)(center.x / size.x * gridSize.x),
				(int)(center.y / size.y * gridSize.y),
				(int)(center.z / size.z * gridSize.z)
			);

			int gridId = grid.x + (grid.y * yOffset) + (grid.z * zOffset);

			if (!grids.TryGetValue(gridId, out List<int> gridVertices))
			{
				gridVertices = new();
				grids.Add(gridId, gridVertices);
			}
			
			gridVertices.Add(i);
		}
		
		// Create the sub meshes.
		Mesh[] subMeshes = new Mesh[grids.Count];
		int meshIndex = 0;

		foreach ((int gridId, List<int> grid) in grids)
		{
			Dictionary<int, int> subMap = new();
			List<Vector3> subVertices = new();
			List<Vector3> subNormals = new();
			List<Vector2> subUvs = new();
			List<int> subTriangles = new();

			foreach (int i in grid)
			{
				for (int j = 0; j < 3; j++)
				{
					int triangle = triangles[i + j];

					if (!subMap.TryGetValue(triangle, out int subTriangle))
					{
						subTriangle = subVertices.Count;
						subMap.Add(triangle, subTriangle);
						
						subVertices.Add(vertices[triangle]);
						subNormals.Add(normals[triangle]);
						subUvs.Add(uvs[triangle]);
					}

					subTriangles.Add(subTriangle);
				}
			}

			subMeshes[meshIndex++] = new()
			{
				vertices = subVertices.ToArray(),
				normals = subNormals.ToArray(),
				triangles = subTriangles.ToArray(),
				uv = subUvs.ToArray(),
				name = $"Grid{gridId}",
			};
		}

		return subMeshes;
	}
}
