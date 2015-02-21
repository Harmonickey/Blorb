using UnityEngine;
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
        if (GUIManager.Instance.OnTutorialScreen) return;

        preSelected = true;

        //only create new ones if we have none
        if (center.HasEnoughResources(this.cost))
        {
            if (GameObject.FindGameObjectsWithTag("Placement").Length == 0)
                center.FindAllPossiblePlacements();

            SpriteRenderer[] srs = GameObject.FindGameObjectsWithTag("Towers")[0].GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in srs)
            {
                if (sr.GetComponent<Placement>() != null)
                {
                    if (!center.HasEnoughResources(sr.GetComponent<Placement>().cost))
                        continue;
                }
                else if (sr.GetComponentInChildren<Placement>() != null)
                {
                    if (!center.HasEnoughResources(sr.GetComponentInChildren<Placement>().cost))
                        continue;
                }

                sr.color = new Color(1f, 1f, 1f);
            }

            this.GetComponent<SpriteRenderer>().color = new Color(0.337f, 0.694f, 1f);
            this.transform.parent.GetComponent<SpriteRenderer>().color = new Color(0.337f, 0.694f, 1f);

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
        if (selected && placementPiece != null)
        {
            Vector3 hit = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            placementPiece.position = new Vector3(hit.x, hit.y, 0.0f); //drag the placment piece with the mouse
            
            if (placementPiece.renderer.enabled == false)
                placementPiece.renderer.enabled = true;

            if (Input.GetMouseButtonDown(0)) //for now, drop with mouse-button "0" which is left-click
            {
                //selected = false;
                
                if (positionToSnap != Vector3.zero && parent != null && spot != -1)
                {
                    placementPiece.position = positionToSnap;
                    center.PlacePiece(type, BuildDirection.ToDirFromSpot(spot), parent, this.cost);

                    //reset all placement tile variables
                    positionToSnap = Vector3.zero;
                    spot = -1;
                    parent = null;

                    //make sure there aren't any hanging Update processes
                    GameObject[] placements = GameObject.FindGameObjectsWithTag("Placement");
                    foreach (GameObject placement in placements)
                        placement.GetComponent<PlacementBottom>().CheckDistance = false;
                    
                    //remove the placment piece after it's set because now it was replaced by a real tower
                    if (!center.HasEnoughResources(this.cost))
                        StopPlacement();

                    //reinit building process
                    Pathfinder2D.Instance.Create2DMap();
                    player.GetComponent<Center>().RecalculateAllPossiblePlacements();
                    
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopPlacement();
        }
    }

    public void StopPlacement()
    {
        RemoveHangingObjects();

        if (GameObject.FindGameObjectsWithTag("Placement").Length > 0)
            center.RemoveAllPossiblePlacements();
    }

    void CreatePlacement()
    {  
        RemoveHangingObjects();

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

    private void RemoveHangingObjects()
    {
        //look for all the placement objects that could have been instantiated
        GameObject t = GameObject.FindGameObjectWithTag("Turret Placement");
        GameObject c = GameObject.FindGameObjectWithTag("Collector Placement");
        GameObject w = GameObject.FindGameObjectWithTag("Wall Placement");

        //remove the one that's out
        foreach (GameObject obj in new GameObject[] {t, c, w})
            if (obj != null)
                Destroy(obj.transform.parent.gameObject);
    }
}
