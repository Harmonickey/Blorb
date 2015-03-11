using UnityEngine;
using System.Collections;

public class Placement : MonoBehaviour {

    public Transform player;
	public SpriteRenderer pictograph;

    public static PlacementPiece placementPiece;

	public TextMesh nameText;
    public int cost;
	public TextMesh costText;

    public static ArrayList possiblePlacements = new ArrayList();

    public bool selected;
    private bool preSelected;
    private Center center;

    public static bool isPlacingTowers = false;

    public bool userSelected = false;

    void Start()
    {
        selected = false;
        userSelected = false;
        center = player.GetComponent<Center>();

		nameText.renderer.sortingLayerName = "UI";
		nameText.renderer.sortingOrder = 2;
		costText.renderer.sortingLayerName = "UI";
		costText.renderer.sortingOrder = 2;
    }

	void setTowerDetail (bool enabled)
	{
		pictograph.renderer.enabled = enabled;
		pictograph.transform.parent.renderer.enabled = enabled;
		nameText.text = gameObject.name;
		nameText.renderer.enabled = enabled;
		costText.text = cost.ToString ();
		costText.renderer.enabled = enabled;
	}

	void OnMouseEnter ()
	{
		GUIManager.Instance.MouseOverUI = true;
		if (GUIManager.Instance.ViewStage == 2) {
			setTowerDetail (true);
		}
	}
	
	void OnMouseExit ()
	{
		GUIManager.Instance.MouseOverUI = false;
		setTowerDetail (false);
	}

	// Use this for initialization
	void OnMouseDown()
    {
        if (GUIManager.Instance.OnTutorialScreen) return;

		BaseCohesionManager.UnMarkAllAttachments();
		
		preSelected = true;

        //only create new ones if we have none
        if (BlorbManager.Instance.HasEnoughResources(this.cost))
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
            UnSelectAllPlacements();

            selected = true;
            isPlacingTowers = true;
            BaseCohesionManager.UnMarkAllAttachments();
        }

        preSelected = false;
    }

    void Update()
    {
        if (selected)
        {
            if (Input.GetMouseButtonDown(0) && placementPiece.positionToSnap != Vector3.zero) //for now, drop with mouse-button "0" which is left-click
            {
				if (BlorbManager.Instance.BlorbAmount >= this.cost) {
                	PlacePiece();
				}
            }
            else if (possiblePlacements.Count > 0)
            {
                FindClosestPlacement();
            }
        }
    }

    private void PlacePiece()
    {
        center.PlacePiece(placementPiece, this);

        //reset our placement piece
        placementPiece = new PlacementPiece(this.cost, this.tag);

        //remove the placment piece after it's set because now it was replaced by a real tower
        if (!BlorbManager.Instance.HasEnoughResources(this.cost)) {

            StopPlacement();
		}

        possiblePlacements = new ArrayList();

        //reinit building process
        center.RecalculateAllPossiblePlacements();
    }

    private void FindClosestPlacement()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        mouse = new Vector3(mouse.x, mouse.y, 0);

        PlacementBottom closest = possiblePlacements[0] as PlacementBottom;
        float selectionThreshold = (((this.GetComponent<SpriteRenderer>().sprite.rect.width * 2) / WorldManager.PixelOffset) / 2) + 0.1f;
        float closestDistance = Mathf.Infinity;
        foreach (PlacementBottom possiblePlacement in possiblePlacements)
        {
            float distance = Vector2.Distance(mouse, possiblePlacement.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = possiblePlacement;
            }
            possiblePlacement.GetComponent<SpriteRenderer>().color = PlacementBottom.unSelectedColor;
            possiblePlacement.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

        if (closestDistance <= selectionThreshold)
        {
            placementPiece.positionToSnap = closest.transform.localPosition;
            closest.GetComponent<SpriteRenderer>().color = PlacementBottom.selectedColor;
            closest.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        else
        {
            placementPiece.positionToSnap = Vector3.zero;
        }
    }

    private void UpdateGUIColors()
    {
        GUIManager.Instance.UpdateTowerGUI(this);

        this.GetComponent<SpriteRenderer>().color = new Color(0.337f, 0.694f, 1f);
        this.transform.parent.GetComponent<SpriteRenderer>().color = new Color(0.337f, 0.694f, 1f);
    }

    public static void StopPlacement()
    {
        Debug.Log("stop placement called");

        UnSelectAllPlacements();
        
        Center.Instance.DelayPlacementStopping();

        BaseCohesionManager.UnMarkAllAttachments();

        possiblePlacements = new ArrayList();

        if (GameObject.FindGameObjectsWithTag("Placement").Length > 0)
            Center.RemoveAllPossiblePlacements();

        GUIManager.Instance.UpdateTowerGUI();

    }

    static void UnSelectAllPlacements()
    {
        Debug.Log("unselecting");
        Placement[] placements = GameObject.FindObjectsOfType<Placement>();
        foreach (Placement placement in placements)
        {
            placement.selected = false;
            placement.preSelected = false;
        }
    }

    void CreatePlacement()
    {
        placementPiece = new PlacementPiece(this.cost, this.tag);
    }

}
