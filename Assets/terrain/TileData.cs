using UnityEngine;
using System.Collections;

public class TileData {
	public enum TileType {None, Grass, Stone};

	public int width;
	public int height;

	private TileType[,] tiles;

	private TileData(){
		tiles = new TileType[width, height];
	}

	public void AddTile(int x, int y, TileType type){
		tiles[x, y] = type;
	}

	public void RemoveTile(int x, int y){
		tiles[x, y] = TileType.None;
	}

	//Currently very basic (and shitty) algorithm
	public void GenerateMap(){
		Random rnd = new Random();
		for(int x = 0; x < width; x++){
			for(int y = 0; y < height; y++){
				TileType t = GetRandomEnum<TileType>();
				if (t.Equals(TileType.None)){
					t = TileType.Grass;
				}
				tiles[x, y] = t;
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
