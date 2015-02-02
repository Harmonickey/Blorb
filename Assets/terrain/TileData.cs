using UnityEngine;
using System.Collections;

public class TileData {
	enum Tile {None, Grass, Stone};

	public int width;
	public int height;

	Tile[][] Tiles = new Tile[width][height];

	public static int GetTileData(Tile t){
		switch (t){
			case Tile.None:
				
				break;
			case Tile.Grass:
				break;
			case Tile.Stone:
				break;

		}
	}

}
