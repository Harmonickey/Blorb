using UnityEngine;
using System.Collections;

public class Placement : MonoBehaviour {

    public Transform player;

    public static PlacementPiece placementPiece;

    public int cost;

    public ArrayList possiblePlacements = new ArrayList();

    private bool selected;
    private bool preSelected;
    private Center center;

    void Start()
    {
        selected = false;
        center = player.GetComponent<Center>();
    }

	// Use this for initialization
	void OnMouseDown()
    {
        if (GUIManager.Instance.OnTutorialScreen) return;

        preSelected = true;

        //only create new ones if we have none
        if (center.HasEnoughResources(this.cost))
        {
            if (GameObject.FindGameObjectsWithTag("Placement").Length == 0)
            {
                center.FindAllPossiblePlacements();
            }

            UpdateGUIColors();

            CreatePlacement();
        }
    }

    void OnMouseUp()
    {
        if (GUIManager.Instance.OnTutorialScreen) return;

        if (preSelected == true)
        {
            selected = true;
            preSelected = false;
        }
    }

    void Update()
    {
        if (selected)
        {   
            if (Input.GetMouseButtonDown(0)) //for now, drop with mouse-button "0" which is left-click
            {

                if (placementPiece.positionToSnap != Vector3.zero)
                {
                    center.PlacePiece(placementPiece);

                    //reset our placement piece
                    placementPiece = new PlacementPiece(this.cost, this.tag);

                    //remove the placment piece after it's set because now it was replaced by a real tower
                    if (!center.HasEnoughResources(this.cost))
                        StopPlacement();

                    //reinit building process
                    //Pathfinder2D.Instance.Create2DMap();
                    center.RecalculateAllPossiblePlacements();
                    
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopPlacement();
        }
    }

    private void UpdateGUIColors()
    {
        GUIManager.RefreshTowerGUIColors();

        this.GetComponent<SpriteRenderer>().color = new Color(0.337f, 0.694f, 1f);
        this.transform.parent.GetComponent<SpriteRenderer>().color = new Color(0.337f, 0.694f, 1f);
    }

    public void StopPlacement()
    {
        if (GameObject.FindGameObjectsWithTag("Placement").Length > 0)
            center.RemoveAllPossiblePlacements();

        GUIManager.RefreshTowerGUIColors();

        selected = false;
    }

    void CreatePlacement()
    {
        placementPiece = new PlacementPiece(this.cost, this.tag);
    }

}
