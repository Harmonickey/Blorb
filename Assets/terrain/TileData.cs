using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileData {

	public int width;
	public int height;
	public enum TileType {None, Grass, Stone, Water, Resource, Orange, Purple, Teal};

	private TileType[,] tiles; //Change to Tile sometime?
	private List<Resource> resources = new List<Resource>();

	public TileData(int width, int height){
		tiles = new TileType[width, height]; //change to Tile sometime?
		this.width = width;
		this.height = height;
		GenerateMap();
		GenerateResources();
		//FillMap (TileType.Grass);
	}

	public TileType GetTileType(int x, int y){
		return tiles[x, y];
	}

	public void SetTileType(int x, int y, TileType t){
		tiles[x,y] = t;
	}

	public static bool isPassableType(TileType t){
		if (t == TileType.Stone || t == TileType.Water || t == TileType.None){
			return false;
		}
		else{
			return true;
		}
	}

	public bool isPassable(int x, int y){
		return (isPassableType(tiles[x,y]) && !existsEntityOnTile(x, y));
	}

	public bool isPassable(Vector2 t){
		return isPassable((int)t.x, (int)t.y);
	}

	public bool existsEntityOnTile(int x, int y){

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

	public Resource GetResource(Vector3 position){
		if (isResource(position)){
			foreach (Resource r in resources){
				if (position.Equals(r.getPosition())){
					return r;
				}
			}
		}

		return null;
	}

	public float GetDistance(Vector2 start, Vector2 end){
		float x_squared = (end.x - start.x) * (end.x-start.x);
		float y_squared = (end.y - start.y) * (end.y-start.y);

		return Mathf.Sqrt(x_squared + y_squared);
		
	}

	//Map generation and filling algorithms after here

	public void FillRange(TileType t, int left, int top, int bottom_offset, int right_offset){
		for(int x = 0; x < right_offset; x++){
			for(int y = 0; y < bottom_offset; y++){
				SetTileType(left + x,top + y,t);
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

	//Slightly better random algorithm, but still not very good
	public void GenerateMap(){
		for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				float tileChance = Random.Range(0f, 1f);
				if (tileChance > 0.99f){
					SetTileType(x, y, TileType.Resource);
					//other resource placing stuff
				}
				else if (tileChance > 0.9f) {
					SetTileType(x, y, TileType.Stone);
				}
				else {
					SetTileType(x, y, TileType.Grass);
				}

				FillRange(TileType.Grass, width/3, height/3, width/3, height/3);
			}
		}

	}

	public void GenerateResources(){
		for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				if (isResource(x, y)){
					resources.Add(new Resource(x, y));
				}
			}
		}
	}

	//probably want to move this to a utilities class at some point
	static T GetRandomEnum<T>()
	{
		System.Array A = System.Enum.GetValues(typeof(T));
		T V = (T)A.GetValue(UnityEngine.Random.Range(0,A.Length));
		return V;
	}
}
