using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class TileMap : MonoBehaviour {

	public int tiles_x;
	public int tiles_y;

	private float tileSize = 1f/5f;

	// Use this for initialization
	void Start () {
		GenerateMesh();
	}

	void GenerateMesh() {
		int vertices_x = tiles_x + 1;
		int vertices_y = tiles_y + 1;

		int numTiles = tiles_x * tiles_y;
		int numVertices = vertices_x * vertices_y;
		int numTriangles = numTiles * 2;

		//Generate vertices
		Vector3[] vertices = new Vector3[numVertices];
		Vector2[] UVs = new Vector2[numVertices];
		int[] triangles = new int[numTriangles * 3];
		for (int y = 0; y < vertices_y; y++){
			for (int x = 0; x < vertices_x; x++){
				int currentVertex = y * vertices_x + x;
				vertices[currentVertex] = new Vector3(x*tileSize, -y*tileSize, 0);
				UVs[currentVertex] = new Vector2(0,0); //temporarily hardcoded

				//Debug.Log (String.Format("Vertex[{0}, {1}] is vertex {2}", x, -y, currentVertex));
			}
		}

		//Generate triangles (and other whole-tile related stuff)
		int triangleOffset = 0;
		int tileIndex = 0;
		for (int y = 0; y < tiles_y; y++){
			for (int x = 0; x < tiles_x; x++){
				tileIndex = y * tiles_x + x;
				//Debug.Log (String.Format ("tileIndex = {0}", tileIndex));
				triangleOffset = tileIndex * 6;

				triangles[triangleOffset + 0] = y * vertices_x + x;
				triangles[triangleOffset + 1] = y * vertices_x + x + 1;
				triangles[triangleOffset + 2] = (y + 1) * vertices_x + x + 1;

				triangles[triangleOffset + 3] = y * vertices_x + x;
				triangles[triangleOffset + 4] = (y + 1) * vertices_x + x + 1;
				triangles[triangleOffset + 5] = (y + 1) * vertices_x + x;
			}
		}

		//Make the actual mesh from the data
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = UVs;

		mesh.RecalculateNormals();
		mesh.Optimize();

		//Assign it to components
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		MeshCollider meshCollider = GetComponent<MeshCollider>();

		meshFilter.mesh = mesh;
	}


}