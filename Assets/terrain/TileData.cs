using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileData {

	public int width;
	public int height;
	public enum TileType {None, Grass, Stone, Water, Resource, Orange, Purple, Teal};

	private TileType[,] tiles; //Change to Tile sometime?
	private Vector2 cIndex;

	public TileData(int width, int height, Vector2 chunkIndex){
		tiles = new TileType[width, height]; //change to Tile sometime?
		this.width = width;
		this.height = height;
		cIndex = chunkIndex;

		GenerateMap(cIndex);
		//FillMap (TileType.Grass);
	}

	public TileType GetTileType(int x, int y){
		return tiles[x, y];
	}

	public void SetTileType(int x, int y, TileType t){
		tiles[x,y] = t;
	}

	public static bool isPassable(TileType t){
		if (t == TileType.Stone || t == TileType.Water || t == TileType.None){
			return false;
		}
		else{
			return true;
		}
	}

	public bool isPassable(int x, int y){
		return isPassable(tiles[x,y]);
	}

	public static bool isResource(TileType t){
		if (t == TileType.Resource){
			return true;
		}
		else {
			return false;
		}
	}

	public bool isResource(int x, int y){
		if (x > 0 && y > 0 && x < width && y < height){
			return isResource(tiles[x,y]);
		}
		else {
			return false;
		}
	}

	public bool isResource(Vector3 position){
		return isResource((int)position.x, (int)position.y);
	}

	public void FillRange(TileType t, int top, int left, int bottom_offset, int right_offset){
		//Debug.Log (t.ToString() + ", " + left.ToString() + ", " + top.ToString() + ", " + bottom_offset.ToString() + ", " + right_offset.ToString());
		for(int x = left; x < right_offset; x++){
			for(int y = top; y < bottom_offset; y++){
				//Debug.Log ((left + x).ToString() + ", " + (top + y).ToString());
				SetTileType(x, y,t);
			}
		}
	}
	
	public void FillMap(TileType t){
		FillRange (t, 0, 0, width, height);
		/*for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				SetTile (x,y,t);
			}
		}*/
	}

	//Very basic (and shitty) algorithm
	public void GenerateMapRandomly(){
		for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				TileType t = GetRandomEnum<TileType>();
				SetTileType(x, y, t);
			}
		}
	}

	public float ManhattanDistance(Vector2 a, Vector2 b){
		return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
	}

	//Slightly better random algorithm, but still not very good
	public void GenerateMap(Vector2 chunkIndex){
		for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				float seed = Random.Range(0f, 1f);
				float distance = ManhattanDistance(chunkIndex, Vector2.zero);
				float mountainChance = 0.05f * distance + 0.05f; //set arbitrarily, might want to set a cap
				float resourceChance = Mathf.Min (-0.01f * distance + 0.2f, 0.01f);

				if (seed < resourceChance && 
				    x > 0 && y > 0 && x < width-1 && y < height-1){ //not on an edge
					SetTileType(x, y, TileType.Resource);
					//other resource placing stuff
				}
				else if (seed < mountainChance) {
					SetTileType(x, y, TileType.Stone);
				}
				else {
					SetTileType(x, y, TileType.Grass);
				}



			}
		}


		int edgeWidth = (int) ManhattanDistance(chunkIndex, Vector2.zero) + 5; //constant chosen arbitrarily
		int edgeHeight = (int) ManhattanDistance(chunkIndex, Vector2.zero) + 5;
		
		FillRange(TileType.Grass, 0, 0,  edgeHeight, width);
		FillRange(TileType.Grass, height - edgeHeight, 0,  height, width);
		FillRange(TileType.Grass, 0, 0, height, edgeWidth);
		FillRange(TileType.Grass, 0, height - edgeWidth, height, width);
		//FillRange(TileType.Grass, corridorSize, corridorSize, (int) (width - distance), (int) (height - distance));
        //mountains should be denser and resources should be sparser the further away from the center you are
		//also the size of the corridor should also be smaller (min 3?)

	}

	//probably want to move this to a utilities class at some point
	static T GetRandomEnum<T>()
	{
		System.Array A = System.Enum.GetValues(typeof(T));
		T V = (T)A.GetValue(UnityEngine.Random.Range(0,A.Length));
		return V;
	}
}
