using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshCollider))]
public class MapChunk : MonoBehaviour {
	public int tiles_x;
	public int tiles_y;
	public Vector2 chunkIndex;
	public TileData tileData;
	public TileMap tileMap;
	public Transform mountain;
	public Transform resource;
	public Dictionary<Vector2, Resource> resources;
	
	private float tileSize = 1f;
	private int pixelsPerTile = 32;

	void Start () {
		//tileData = new TileData(tiles_x, tiles_y, chunkIndex);

		//GenerateChunk();
	}

	public void GenerateChunk () {
		GenerateMesh();
		GenerateTexture();
		CreateObjects();
		//ResourceTest();
		//Debug.Log ("Chunk complete!");
	}

	void ResourceTest(){
		foreach (Resource r in resources.Values){
			if (tileData.isResource(PositionToChunkTile(r.transform.position))){
				Debug.Log("Resource at " + r.transform.position.ToString() + " is detecting as resource properly.");
				Debug.DrawLine(Vector3.zero, r.transform.position, Color.green, 30f);
			}
			else {
				Debug.LogError("Resource at " + r.transform.position.ToString() + " is not detecting as resource!");
				Debug.DrawLine(Vector3.zero, r.transform.position, Color.red, 30f);
			}

			/*for (int y = 0; y < tiles_y; y++){
				for (int x = 0; x < tiles_x; x++){
					if (tileData.isResource(x,y)){
						//Debug.DrawLine(Vector3.zero, ChunkTileToPosition(new Vector2(x,y)), Color.black, 30f);
						//Debug.DrawLine(ChunkTileToPosition(new Vector2(x,y)), r.transform.position, Color.black, 30f);
						//Debug.DrawLine(r.transform.position, Vector3.zero, Color.black, 30f);
						//Debug.Log("x: " + x.ToString() + ", y: " + y.ToString() + ", pos: " + PositionToChunkTile(r.transform.position));
						
					}
				}
			}*/
		}


	}

	void GenerateTexture() {
		int texWidth = tiles_x * pixelsPerTile;
		int texHeight = tiles_y * pixelsPerTile;
		Texture2D mapTexture = new Texture2D(texWidth, texHeight);
		mapTexture.filterMode = FilterMode.Point;
		
		Color[][] tiles = tileMap.ChopTiles();
		
		for(int y = 0; y < tiles_y; y++){
			for(int x = 0; x < tiles_x; x++){
				int start_x = x*pixelsPerTile;
				int start_y = y*pixelsPerTile;
				Color[] pixels = tiles[(int)tileData.GetTileType(x, y)];
				mapTexture.SetPixels(start_x, start_y, pixelsPerTile, pixelsPerTile, pixels);
			}
		}
		mapTexture.Apply();
		
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = new Material((Material) Resources.Load("MapMaterial", typeof(Material)));
		//meshRenderer.sharedMaterial.SetTextureScale("_MainTex", new Vector2(1f, -1f)); //otherwise texture is upside down
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
				
				int topLeftVertex = y * vertices_x + x; 
				int topRightVertex = y * vertices_x + x + 1;
				int bottomLeftVertex = (y + 1) * vertices_x + x;
				int bottomRightVertex = (y + 1) * vertices_x + x + 1;
				
				triangles[triangleOffset + 0] = topLeftVertex;
				triangles[triangleOffset + 1] = topRightVertex;
				triangles[triangleOffset + 2] = bottomRightVertex;
				
				triangles[triangleOffset + 3] = topLeftVertex;
				triangles[triangleOffset + 4] = bottomRightVertex;
				triangles[triangleOffset + 5] = bottomLeftVertex;
				
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
	
	void CreateObjects(){
		//Debug.Log ("Creating Objects");
		resources = new Dictionary<Vector2, Resource>();
		for (int y = 0; y < tiles_y; y++){
			for (int x = 0; x < tiles_x; x++){
				//Vector2 centeringOffset = new Vector2(0.5f * tileSize,  - 0.5f* tileSize);
				Vector2 absolutePosition = ChunkTileToMapTile(x, y);
				if (!tileData.isPassable(x, y)){
					Transform newMountain = Instantiate(mountain) as Transform;
					newMountain.transform.position = tileMap.TileToPosition((int) absolutePosition.x, (int) absolutePosition.y);
					//do something with the scale?
					newMountain.tag = "Mountain";
					newMountain.transform.parent = this.gameObject.transform;
					//mountains.Enqueue(newMountain);
				}
				
				if(tileData.isResource(x, y)){
					Transform newResource = Instantiate(resource) as Transform;
					newResource.transform.position = tileMap.TileToPosition((int)absolutePosition.x,(int)absolutePosition.y);
					newResource.tag = "Resource";
					newResource.parent = this.gameObject.transform;
					resources[new Vector2(x,y)] = newResource.GetComponent<Resource>();
				}
			}
		}
	}

	public Vector2 ChunkTileToMapTile(int x, int y){
		int tile_x = (int)chunkIndex.x * tiles_x + x;// - ((int) tiles_x / 2);
		int tile_y = (int)chunkIndex.y * tiles_y + y;// - ((int) tiles_y / 2);

		return new Vector2((float)tile_x, (float)tile_y);
	}

	public Vector2 ChunkTileToMapTile(Vector2 chunkTile){
		return ChunkTileToMapTile((int)chunkTile.x, (int)chunkTile.y);
	}

	/*public Vector2 MapTileToChunkTile(int x, int y){
		int tile_x = x % tiles_x;
		int tile_y = y % tiles_y;
		if (chunkIndex.x * tiles_x + tile_x != x || chunkIndex.y * tiles_y + tile_y != y){
			Debug.LogWarning("MapTileToChunkTile: Result not in current chunk!");
			return new Vector2(float.NaN, float.NaN);
		}
		return new Vector2((float)tile_x, (float)tile_y);
	}

	public Vector2 MapTileToChunkTile(Vector2 mapTile){
		return MapTileToChunkTile((int) mapTile.x, (int) mapTile.y);
	}*/

	public Vector2 ChunkTileToPosition(Vector2 chunkTile){
		return tileMap.TileToPosition(ChunkTileToMapTile(chunkTile));
	}

	public Vector2 PositionToChunkTile(Vector2 pos){
		return tileMap.MapTileToChunkTile(tileMap.PositionToTile(pos));
	}

	/*
	public Vector2 ChunkTileToIndex (Vector2 chunkTile){
		return new Vector2(chunkTile.x + ((int) tiles_x / 2), chunkTile.y + ((int) tiles_y / 2));
	}

	public Vector2 MapTileToIndex(int x, int y){
		return ChunkTileToIndex(MapTileToChunkTile(new Vector2(x,y)));
	}

	Vector2 PositionToIndex(Vector2 pos){
		return ChunkTileToIndex(MapTileToChunkTile(tileMap.PositionToTile(pos)));
	}*/

}
