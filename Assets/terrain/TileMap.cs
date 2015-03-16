using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]

public class TileMap : MonoBehaviour {

	public int chunk_tiles_x;
	public int chunk_tiles_y;
	public float mountainDistanceFactor, mountainBaseDensity = 0.05f;
	public float resourceDistanceFactor = -0.01f;
	public float resourceBaseDensity = 0.2f;
	public Dictionary<Vector2, MapChunk> chunks;
	
	private Transform chunk;
	private Transform mountain;
	private Transform resource;
	private Texture2D tileSet;
	private int map_tiles_x;
	private int map_tiles_y;
	private int left, right, top, bottom = 0;
	private float tileSize = 1f;
	private int pixelsPerTile = 32;
	private GameObject center;
	//private Queue<Transform> mountains;

	// Use this for initialization
	void Start () {
		chunks = new Dictionary<Vector2, MapChunk>();

		mountain = (Transform) Resources.Load("Mountain", typeof(Transform));
		resource = (Transform) Resources.Load("Resource", typeof(Transform));
		chunk = (Transform) Resources.Load("Chunk", typeof(Transform));
		tileSet = (Texture2D) Resources.Load("TileTextures4", typeof(Texture2D));
		center = GameObject.FindGameObjectWithTag("Center");

		for (int x = -2; x < 2; x++){
			for (int y = -2; y < 2; y ++){
				CreateNewChunk(x, y);
			}
		}
	}

	void Update(){
		StartCoroutine("GenerateIfNearEdge");
	}
	
	void CreateNewChunk(int x, int y){
		Transform newChunkTransform = Instantiate(this.chunk) as Transform;
		Vector2 centeringOffset = new Vector2(0.5f * tileSize,  - 0.5f* tileSize);
		newChunkTransform.position = ChunkToPosition(x, y) - centeringOffset;
		newChunkTransform.parent = this.gameObject.transform;

		MapChunk chunk = newChunkTransform.gameObject.GetComponent<MapChunk>();

		chunk.tiles_x = chunk_tiles_x;
		chunk.tiles_y = chunk_tiles_y;
		chunk.chunkIndex = new Vector2(x, y);
		if (chunks.ContainsKey(chunk.chunkIndex)){
			return;
		}

		chunk.tileData = new TileData(chunk.tiles_x, chunk.tiles_y, 
		                              mountainDistanceFactor, mountainBaseDensity, resourceDistanceFactor, resourceBaseDensity,
		                              chunk.chunkIndex);
		chunk.tileMap = this;
		chunk.mountain = this.mountain;
		chunk.resource = this.resource;

		if (chunks.Count == 0){
			//if there are no other chunks, need to add it no matter what
			map_tiles_x += chunk_tiles_x;
			map_tiles_y += chunk_tiles_y;
			left = right = (int)chunk.chunkIndex.x;
			top = bottom = (int)chunk.chunkIndex.y;
		}

		if (chunk.chunkIndex.x < left){
			map_tiles_x += chunk_tiles_x;
			left = (int)chunk.chunkIndex.x;
		}
		else if (chunk.chunkIndex.x > right){
			map_tiles_x += chunk_tiles_x;
			right = (int)chunk.chunkIndex.x;
		}

		if (chunk.chunkIndex.y < top){
			map_tiles_y += chunk_tiles_y;
			top = (int)chunk.chunkIndex.y;
		}
		else if (chunk.chunkIndex.y > bottom){
			map_tiles_y += chunk_tiles_y;
			bottom = (int)chunk.chunkIndex.y;
		}

        chunks[chunk.chunkIndex] = chunk;
		chunk.GenerateChunk();


		
    }

	IEnumerator GenerateIfNearEdge(){
		Vector3 position = center.transform.position;
		Vector3 currentTile = PositionToTile(position);
		Vector2 currentChunk = MapTileToChunk(currentTile);
		if (currentChunk.x == left){
			left -= 1;
			map_tiles_x += chunk_tiles_x;
			for (int y = top; y <= bottom; y++){
				CreateNewChunk(left, y);
				
				yield return null;
			}
		}
		else if (currentChunk.x == right){
			right += 1;
			map_tiles_x += chunk_tiles_x;
			for (int y = top; y <= bottom; y++){
				CreateNewChunk(right, y);
				yield return null;
			}
		}
		else if (currentChunk.y == top){
			top -= 1;
			map_tiles_y += chunk_tiles_y;
			for (int x = left; x <= right; x++)
			{
				CreateNewChunk(x, top);
				yield return null;
			}
		}
		else if (currentChunk.y == bottom){
			bottom += 1;
			map_tiles_y += chunk_tiles_y;
			for (int x = left; x <= right; x++)
			{
				CreateNewChunk(x, bottom);
				yield return null;
			}
		}
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
		int tileOffsetWidth = 0;//map_tiles_x/2;
		int tileOffsetHeight = 0;//map_tiles_y/2;

		return new Vector3 (Mathf.RoundToInt(pos.x/trueTileSize.x) + tileOffsetWidth, -1*Mathf.RoundToInt(pos.y/trueTileSize.y) + tileOffsetHeight, 0);
	}

	/*public Vector3 PositionToTileOffset(Vector3 pos){
		Vector3 trueTileSize = tileSize * transform.localScale; //maybe lossyScale?
		return new Vector3 (Mathf.RoundToInt(pos.x/trueTileSize.x), -1*Mathf.RoundToInt(pos.y/trueTileSize.y), 0);
	}*/
	
	public Vector3 TileToPosition(int x, int y){
		Vector3 trueTileSize = tileSize * transform.localScale; //maybe lossyScale?
		float centering_offset_x = 0;//0.5f * trueTileSize.x;
		float centering_offset_y = 0;//0.5f * trueTileSize.y;
		
		return new Vector3 (x * trueTileSize.x + centering_offset_x + transform.position.x, -y*trueTileSize.y - centering_offset_y + transform.position.y, 0);
    }

	public Vector3 TileToPosition(Vector3 tile){
		return TileToPosition((int)tile.x, (int)tile.y);
	}

	public Vector2 MapTileToChunkTile(int x, int y){
		Vector2 cIndex = MapTileToChunk(x,y);

		int stride_x = chunk_tiles_x * (int)cIndex.x;
		int stride_y = chunk_tiles_y * (int)cIndex.y;

		return new Vector2((float)(x - stride_x), (float)(y - stride_y));
	}

	public Vector2 MapTileToChunkTile(Vector2 mapTile){
		return MapTileToChunkTile((int) mapTile.x, (int) mapTile.y);
	}

	public Vector2 MapTileToChunk(Vector2 mapTile){
		return MapTileToChunk((int)mapTile.x, (int)mapTile.y);
	}

	public Vector2 MapTileToChunk(int x, int y){
		return new Vector2(Mathf.Floor((float)x/(float)chunk_tiles_x), Mathf.Floor((float)y/(float)chunk_tiles_y));
	}

	public bool IsResource(int x, int y){
		return IsResource(new Vector2((float) x, (float) y));
	}

	public bool IsResource(Vector2 position){
		Vector2 cIndex = MapTileToChunk(position);
		//Debug.Log ("IsResource: cIndex = " + cIndex.ToString());
		if (chunks[cIndex].resources.ContainsKey(MapTileToChunkTile(position))){
			return true;
		}
		else {
			Debug.Log("Chunk " + cIndex.ToString() + " did not contain resource at " + MapTileToChunkTile(position).ToString());
			foreach (Vector2 key in chunks[cIndex].resources.Keys){
				Debug.Log ("["+cIndex.ToString()+"]: " + key.ToString() + " maps to " + chunks[cIndex].resources[key].ToString());
			}
		}
		
		return false;
	}

	public Resource GetResource(Vector3 mapTile){
		return GetResource(new Vector2(mapTile.x, mapTile.y));
	}

	public Resource GetResource(Vector2 mapTile){

		Vector2 chunkIndex = MapTileToChunk(mapTile);
		Vector2 chunkPosition = MapTileToChunkTile(mapTile);
		Debug.Log("Current Chunk:" + chunkIndex.ToString() + "\n");
		return chunks[chunkIndex].resources[chunkPosition];
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

		float centering_offset_x = 0;//-0.5f * chunkSizeX;
		float centering_offset_y = 0;//-0.5f * chunkSizeY;

		float x_offset = x * chunkSizeX + centering_offset_x;
		float y_offset = y * chunkSizeY + centering_offset_y;

		float pos_X = transform.position.x + x_offset;
		float pos_Y = transform.position.y + y_offset;

		return new Vector2(pos_X, -pos_Y);
	}

	public Vector2 PositionToChunk(float x, float y){
		int chunkSizeX = (int)tileSize*chunk_tiles_x;
		int chunkSizeY = (int)tileSize*chunk_tiles_y;

		int x_offset = (int)(x - transform.position.x);
		int y_offset = (int)(y - transform.position.y);

		int chunk_x = x_offset/chunkSizeX;
		int chunk_y = y_offset/chunkSizeY;

		return new Vector2((float)chunk_x, (float)chunk_y);
	}

	public Vector2 PositionToChunk(Vector2 position){
		return PositionToChunk(position.x, position.y);
	}

}