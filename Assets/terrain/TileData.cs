using UnityEngine;
using System.Collections;

public class Tile {

	private enum TileType {None, Grass, Stone};
	private bool passable;
	private TileType type;

}

public class TileData {

	public int width;
	public int height;
	public enum TileType {None, Grass, Stone, Water, Yellow, Orange, Purple, Teal};

	private TileType[,] tiles; //Change to Tile sometime?

	public TileData(int width, int height){
		tiles = new TileType[width, height]; //change to Tile sometime?
		this.width = width;
		this.height = height;
		GenerateMap();
	}

	public TileType GetTile(int x, int y){
		return tiles[x, y];
	}

	public void SetTile(int x, int y, TileType t){
		tiles[x,y] = t;
	}

	//Very basic (and shitty) algorithm
	public void GenerateMapRandomly(){
		for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				TileType t = GetRandomEnum<TileType>();
				SetTile(x, y, t);
			}
		}
	}

	//Slightly better random algorithm, but still not very good
	public void GenerateMap(){
		for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				float tileChance = Random.Range(0f, 1f);
				if (tileChance > 0.8f) {
					SetTile(x, y, TileType.Stone);
				}
				else {
					SetTile(x, y, TileType.Grass);
				}

			}
		}

	}

	public void FillMap(TileType t){
		for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				SetTile (x,y,t);
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
