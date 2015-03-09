using UnityEngine;
using System.Collections;

public class PlacementPiece {

    public PlacementPiece(int cost, string tag)
    {
        this.cost = cost;
        this.type = tag;
    }

    public PlacementPiece(int cost, string tag, float[] direction)
    {
        this.cost = cost;
        this.type = tag;
        this.direction = direction;
    }

    public PlacementPiece(string tag, float[] direction, Transform parent)
    {
        this.type = tag;
        this.direction = direction;
        this.parent = parent;
    }

    public string type = null;
    public Vector3 positionToSnap = Vector3.zero;
    public int cost = 0;
    public Transform piece, parent;

    public float[] direction; //for the 'possible' pieces
}
