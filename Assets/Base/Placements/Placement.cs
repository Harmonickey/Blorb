using UnityEngine;
using System.Collections;

public class Placement : MonoBehaviour {

    private bool selected;
    private bool preSelected;
    public Transform player;
    private Transform placementPiece;
    private Center center;
    public Transform turretPlacement;
    public Transform collectorPlacement;
    public Transform wallPlacement;

    public static Vector3 positionToSnap = Vector3.zero;
    private string type = null;
    public static int spot = -1;
    public static Transform parent = null;

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
        if (GameObject.FindGameObjectsWithTag("Placement").Length == 0) 
            center.FindAllPossiblePlacements();

        CreatePlacement();
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
            placementPiece.position = new Vector3(hit.x, hit.y, 0.0f); //drag the guy along

            if (Input.GetMouseButtonDown(0)) //for now, drop with mouse-button "0" which is left-click
            {
                selected = false;
                
                if (positionToSnap != Vector3.zero && parent != null && spot != -1)
                {
                    placementPiece.position = positionToSnap;
                    center.PlacePiece(type, BuildDirection.ToDirFromSpot(spot), parent);

                    //reset all variables, just in case
                    positionToSnap = Vector3.zero;
                    spot = -1;
                    parent = null;
                    type = null;

                    Destroy(placementPiece.gameObject);

                    //reinit building process
                    player.GetComponent<Center>().RecalculateAllPossiblePlacements();
                    
                }

                if (placementPiece != null)
                    Destroy(placementPiece.gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (placementPiece != null)
                Destroy(placementPiece.gameObject);

            if (GameObject.FindGameObjectsWithTag("Placement").Length > 0)
                center.RemoveAllPossiblePlacements();
        }
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

        placementPiece.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        type = this.tag;
    }
}
