using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]

public class TileMap : MonoBehaviour {

	public int chunk_tiles_x;
	public int chunk_tiles_y;
	public Texture2D tileSet;
	public Transform mountain;
	public Transform resource;
	public Transform chunk;
	public Dictionary<Vector2, MapChunk> chunks;

	private int map_tiles_x;
	private int map_tiles_y;
	private float tileSize = 1f;
	private int pixelsPerTile = 32;
	//private Queue<Transform> mountains;

	// Use this for initialization
	void Start () {
		chunks = new Dictionary<Vector2, MapChunk>();

		mountain = (Transform) Resources.Load("Mountain", typeof(Transform));
		resource = (Transform) Resources.Load("Resource", typeof(Transform));
		chunk = (Transform) Resources.Load("Chunk", typeof(Transform));
		tileSet = (Texture2D) Resources.Load("TileTextures", typeof(Texture2D));

		CreateNewChunk(0, 0);
		//CreateNewChunk(0, 1);
		//CreateNewChunk(1,0);
		//CreateNewChunk(-1, 0);
		//CreateNewChunk(0, -1);
	}
	
	void CreateNewChunk(int x, int y){
		Transform newChunkTransform = Instantiate(this.chunk) as Transform;
		newChunkTransform.position = ChunkToPosition(x, y);
		newChunkTransform.parent = this.gameObject.transform;

		MapChunk chunk = newChunkTransform.gameObject.GetComponent<MapChunk>();
		chunk.tiles_x = chunk_tiles_x;
		chunk.tiles_y = chunk_tiles_y;
		chunk.chunkIndex = new Vector2(x, y);
		chunk.tileMap = this;
		chunk.mountain = this.mountain;
		chunk.resource = this.resource;
        chunks[chunk.chunkIndex] = chunk;
		
    }



	public Color[][] ChopTiles(){
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

    public Vector3 PositionToTile(Vector3 pos){
        Vector3 trueTileSize = tileSize * transform.localScale; //maybe lossyScale?
		int tileOffsetWidth = map_tiles_x/2;
		int tileOffsetHeight = map_tiles_y/2;


		return new Vector3 (Mathf.FloorToInt(pos.x/trueTileSize.x) + tileOffsetWidth, -1*Mathf.FloorToInt(pos.y/trueTileSize.y) + tileOffsetHeight, 0);
	}
	
	public Vector3 TileToPosition(int x, int y){
		Vector3 trueTileSize = tileSize * transform.localScale; //maybe lossyScale?
		float centering_offset_x = 0.5f * trueTileSize.x;
		float centering_offset_y = 0.5f * trueTileSize.y;
		
		return new Vector3 (x * trueTileSize.x + centering_offset_x + transform.position.x, -y*trueTileSize.y - centering_offset_y + transform.position.y, 0);
    }

	public bool IsResource(int x, int y){
		foreach (MapChunk chunk in chunks.Values){
			return chunk.tileData.isResource(x, y);
		}

		return false;
	}

	public bool IsResource(Vector3 position){
		foreach (MapChunk chunk in chunks.Values){
			return chunk.tileData.isResource(position);
        }

		return false;
	}

	public Resource GetResource(Vector3 position){
		return GetResource(new Vector2(position.x, position.y));
	}

	public Resource GetResource(Vector2 position){
		Vector2 chunkLocation = PositionToChunk(position);
		return chunks[chunkLocation].resources[position];
	}

	public Vector2 GetChunkContainingTile(int x, int y){
		//integer division deliberate
		int chunk_x = x/chunk_tiles_x;
		int chunk_y = y/chunk_tiles_y;

		return new Vector2(chunk_x, chunk_y);
	}

	public Vector2 ChunkToPosition(int x, int y){
		//0, 0 should be at the position of the TileMap
		float chunkSizeX = chunk_tiles_x * tileSize;
		float chunkSizeY = chunk_tiles_y * tileSize;

		float centering_offset_x = -0.5f * chunkSizeX;
		float centering_offset_y = -0.5f * chunkSizeY;

		float x_offset = x * chunkSizeX + centering_offset_x;
		float y_offset = y * chunkSizeY + centering_offset_y;

		float pos_X = transform.position.x + x_offset;
		float pos_Y = transform.position.y + y_offset;

		return new Vector2(pos_X, -pos_Y);
	}

	public Vector2 PositionToChunk(float x, float y){
		float chunkSizeX = tileSize*chunk_tiles_x;
		float chunkSizeY = tileSize*chunk_tiles_y;

		float x_offset = x - transform.position.x;
		float y_offset = y - transform.position.y;

		float chunk_x = x_offset/chunkSizeX;
		float chunk_y = y_offset/chunkSizeY;

		return new Vector2(chunk_x,chunk_y);
	}

	public Vector2 PositionToChunk(Vector2 position){
		return PositionToChunk(position.x, position.y);
	}

}