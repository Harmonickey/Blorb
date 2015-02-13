using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TileMap))]

public class Pathfinder : MonoBehaviour {

	private static TileData map;

	// Use this for initialization
	void Start () {
		map = GetComponent<TileMap>().GetTileData();
	}
	
	// Update is called once per frame
	void Update () {

	}

	private static List<Vector2> GetNeighbors(Vector2 node){
		List<Vector2> neighbors = new List<Vector2>();
		if (map.isPassable(node + Vector2.up)){
			neighbors.Add(node + Vector2.up);
		}
		if (map.isPassable(node - Vector2.up)){
			neighbors.Add(node - Vector2.up);
		}
		if (map.isPassable(node + Vector2.right)){
			neighbors.Add(node + Vector2.right);
		}
		if (map.isPassable(node - Vector2.right)){
			neighbors.Add(node - Vector2.right);
		}

		return neighbors;
	}

	public static List<Vector2> GetPath(Vector2 start, Vector2 end){
		//A* based pathfinder

		//First, verify that it is actually possible to be on the end:
		if (!map.isPassable(end))
		{
			return null;
		}

		Queue<Vector2> frontier = new Queue<Vector2>();
		Dictionary<Vector2, Vector2> came_from = new Dictionary<Vector2, Vector2>();
		Dictionary<Vector2, float> cost_so_far = new Dictionary<Vector2, float>();
		came_from[start] = start;
		cost_so_far[start] = 0;
		frontier.Enqueue(start);

		while(frontier.Count != 0){
			Vector2 current = frontier.Dequeue();

			if (current == end){
				//we found a path so return it
				List<Vector2> path = new List<Vector2>();
				Vector2 next = end;
				while (next != start){
					path.Add(next);
					next = came_from[next];
				}
				path.Reverse();
				return path;
			}

			//else keep expanding neighbors
			foreach (Vector2 neighbor in GetNeighbors(current)){
				float new_cost = cost_so_far[current] + map.GetDistance(current, end);
				if(!cost_so_far.ContainsKey(neighbor) || new_cost < cost_so_far[neighbor]){
					cost_so_far[neighbor] = new_cost;
					//priority = new_cost + map.GetDistance(current, end);
					frontier.Enqueue(neighbor);
					came_from[neighbor] = current;
				}
			}
		}
		//if there are no nodes in the frontier and we haven't returned something's wrong
		Debug.LogError("Pathfinder: PATH NOT FOUND!");
		return null;


	}
}
