using UnityEngine;
using System.Collections;

public class Placement : MonoBehaviour {

    public Transform player;
	public SpriteRenderer pictograph;

    public static PlacementPiece placementPiece;

    public int cost;
	public TextMesh costText;

    public static ArrayList possiblePlacements = new ArrayList();

    private bool selected;
    private bool preSelected;
    private Center center;

    public static bool isInRange = false;

    void Start()
    {
        selected = false;
        center = player.GetComponent<Center>();
    }

	void setTowerDetail (bool enabled)
	{
		pictograph.renderer.enabled = enabled;
		pictograph.transform.parent.renderer.enabled = enabled;
		costText.text = cost.ToString ();
		costText.renderer.enabled = enabled;
	}

	void OnMouseEnter ()
	{
		GUIManager.Instance.MouseOverUI = true;
		if (!GUIManager.Instance.OnTutorialScreen) {
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

        BaseCohesion.UnMarkAllAttachments();

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

            BaseCohesion.UnMarkAllAttachments();
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

                    possiblePlacements = new ArrayList();

                    //reinit building process
                    center.RecalculateAllPossiblePlacements();

                }
            }
            else if (possiblePlacements.Count > 0)
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
                    possiblePlacement.GetComponent<SpriteRenderer>().color = new Color(0.516f, 0.886f, 0.882f, 0);
                }

                if (closestDistance <= selectionThreshold)
                {
                    placementPiece.positionToSnap = closest.transform.localPosition;
                    closest.GetComponent<SpriteRenderer>().color = new Color(0.516f, 0.886f, 0.882f, 0.9f);
                }
                else
                {
                    placementPiece.positionToSnap = Vector3.zero;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopPlacement();
            BaseCohesion.UnMarkAllAttachments();
        }
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0) && !selected)
        {

            Attachments[] attachments = GameObject.FindObjectsOfType<Attachments>();
            Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            mouse = new Vector3(mouse.x, mouse.y, 0);

            foreach (Attachments attachment in attachments)
            {
                if (attachment.collider.bounds.Contains(mouse))
                {
                    BaseCohesion.FindAllNeighbors(attachment.transform); // find out our base cohesion network
                    BaseCohesion.MarkAllBrokenAttachments(attachment.transform);
                }
            }
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
        selected = false;

        possiblePlacements = new ArrayList();

        if (GameObject.FindGameObjectsWithTag("Placement").Length > 0)
            center.RemoveAllPossiblePlacements();

        GUIManager.RefreshTowerGUIColors();

        
    }

    void CreatePlacement()
    {
        placementPiece = new PlacementPiece(this.cost, this.tag);
    }

}
