﻿/*using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(TileMap))]
[RequireComponent(typeof(TileData))]

public class TileGraphics : MonoBehaviour {

	public Texture2D tileSet;
		
	private int tiles_x;
	private int tiles_y;
	private float tileSize = 1f/5f;
	private int pixelsPerTile = 32;
	private TileMap parentMap;
	
	// Use this for initialization
	void Start () {
		parentMap = GetComponent<TileMap>();
		tiles_x = parentMap.tiles_x;
		tiles_y = parentMap.tiles_y;
		GenerateMap();
	}
	
	public void GenerateMap () {
		GenerateMesh();
		GenerateTexture();
		Debug.Log ("Map graphics complete!");
	}
	
	Color[][] ChopTiles(){
		int textureTilesX = tileSet.width / pixelsPerTile;
		int textureTilesY = tileSet.height / pixelsPerTile;
		Color[][] tileTextures = new Color[textureTilesX*textureTilesY][];
		
		for(int y = 0; y < textureTilesY; y++){
			for(int x = 0; x < textureTilesX; x++){
				int currentIndex = y * textureTilesX + x;
				int start_x = x*pixelsPerTile;
				int start_y = y*pixelsPerTile;
				
				tileTextures[currentIndex] = tileSet.GetPixels(start_x, start_y, pixelsPerTile, pixelsPerTile);
			}
		}
		
		return tileTextures;
		
	}
	
	void GenerateTexture() {
		TileData map_data = parentMap.GetData();
		if (map_data != null){
			Debug.Log(map_data.ToString());
		}
		else {
			Debug.LogWarning("Map data null!");
			if (parentMap == null){
				Debug.LogWarning("Parent map also null!");
			}
			else { Debug.Log ("But parent map not null!");}
		}
		
		int texWidth = tiles_x * pixelsPerTile;
		int texHeight = tiles_y * pixelsPerTile;
		Texture2D mapTexture = new Texture2D(texWidth, texHeight);
		mapTexture.filterMode = FilterMode.Point;
		
		Color[][] tiles = ChopTiles();
		
		for(int y = 0; y < tiles_y; y++){
			for(int x = 0; x < tiles_x; x++){
				int start_x = x*pixelsPerTile;
				int start_y = y*pixelsPerTile;
				Color[] pixels = tiles[(int)map_data.GetTile(x, y)];
				mapTexture.SetPixels(start_x, start_y, pixelsPerTile, pixelsPerTile, pixels);
			}
		}
		mapTexture.Apply();
		
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.sharedMaterial.mainTexture = mapTexture;
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
				UVs[currentVertex] = new Vector2((float)x/tiles_x, (float)y/tiles_y);
				
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
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		meshFilter.mesh = mesh;
		meshCollider.sharedMesh = mesh;
	}
	
	
}*/