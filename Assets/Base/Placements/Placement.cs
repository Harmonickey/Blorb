﻿using UnityEngine;
using System.Collections;

public class Placement : MonoBehaviour {

    public Transform player;
    public Transform turretPlacement;
    public Transform collectorPlacement;
    public Transform wallPlacement;

    public int cost;

    private bool selected;
    private bool preSelected;
    private Transform placementPiece;
    private Center center;
    private string type = null;

    // TODO: these three items could be created into an abstraction object
    public static int spot = -1;
    public static Transform parent = null;
    public static Vector3 positionToSnap = Vector3.zero;

    void Start()
    {
        selected = false;
        center = player.GetComponent<Center>();
    }
	// Use this for initialization
	void OnMouseDown()
    {
        preSelected = true;

        //only create new ones if we have none
        if (center.HasEnoughResources(this.cost))
        {
            if (GameObject.FindGameObjectsWithTag("Placement").Length == 0)
                center.FindAllPossiblePlacements();

            CreatePlacement();
        }
    }

    void OnMouseUp()
    {
        if (preSelected == true)
        {
            selected = true;
            preSelected = false;
        }
    }

    void Update()
    {
        if (selected && placementPiece != null)
        {
            Vector3 hit = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            placementPiece.position = new Vector3(hit.x, hit.y, 0.0f); //drag the placment piece with the mouse
            
            if (placementPiece.renderer.enabled == false)
                placementPiece.renderer.enabled = true;

            if (Input.GetMouseButtonDown(0)) //for now, drop with mouse-button "0" which is left-click
            {
                selected = false;
                
                if (positionToSnap != Vector3.zero && parent != null && spot != -1)
                {
                    placementPiece.position = positionToSnap;
                    center.PlacePiece(type, BuildDirection.ToDirFromSpot(spot), parent, this.cost);

                    //reset all placement tile variables
                    positionToSnap = Vector3.zero;
                    spot = -1;
                    parent = null;
                    type = null;

                    //make sure there aren't any hanging Update processes
                    GameObject[] placements = GameObject.FindGameObjectsWithTag("Placement");
                    foreach (GameObject placement in placements)
                    {
                        placement.GetComponent<PlacementBottom>().CheckDistance = false;
                    }
                    
                    //remove the placment piece after it's set because now it was replaced by a real tower
                    Destroy(placementPiece.gameObject);

                    //reinit building process
                    Pathfinder2D.Instance.Create2DMap();
                    player.GetComponent<Center>().RecalculateAllPossiblePlacements();
                    
                }

                if (placementPiece != null)
                    Destroy(placementPiece.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopPlacement();
        }
    }

    public void StopPlacement()
    {
        if (placementPiece != null)
            Destroy(placementPiece.gameObject);

        if (GameObject.FindGameObjectsWithTag("Placement").Length > 0)
            center.RemoveAllPossiblePlacements();
    }

    void CreatePlacement()
    {
        switch (this.tag)
        {
            case "Turret":
                placementPiece = Instantiate(turretPlacement) as Transform;
                break;
            case "Collector":
                placementPiece = Instantiate(collectorPlacement) as Transform;
                break;
            case "Wall":
                placementPiece = Instantiate(wallPlacement) as Transform;
                break;
        }

        placementPiece.localScale = Vector2.one;
        placementPiece.transform.position = this.transform.position; //fixes render bug
        placementPiece.renderer.enabled = false; //don't show it yet, until the frame where the mouse is detected happens
        type = this.tag;
    }
}
