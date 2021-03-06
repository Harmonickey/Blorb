﻿using UnityEngine;
using System.Collections;

public class Placement : MonoBehaviour {

    public Transform player;
	public SpriteRenderer pictograph;

    public static PlacementPiece placementPiece;

    public Transform turret;
    public Transform collector;
    public Transform wall;

    public Transform range;

    public static Transform tempPlacement;
    public static Transform rangeIndicator;

	public TextMesh nameText;
    public int cost;
	public TextMesh costText;

    public static ArrayList possiblePlacements = new ArrayList();

    public bool selected;
    private bool preSelected;
    private Center center;

    public static bool isPlacingTowers = false;

    public bool userSelected = false;

    bool hasEntered = false;

    bool rendererDisabled = false;

    void Start()
    {
        selected = false;
        userSelected = false;
        center = player.GetComponent<Center>();

		nameText.renderer.sortingLayerName = "UI";
		nameText.renderer.sortingOrder = 2;
		costText.renderer.sortingLayerName = "UI";
		costText.renderer.sortingOrder = 2;

        GameEventManager.NightStart += NightStart;
    }

    void NightStart()
    {
        //just to double check, weird corner case where tower details stay up at night
        GUIManager.Instance.MouseOverUI = false;
        setTowerDetail(false);
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

        hasEntered = true;
	}
	
	void OnMouseExit ()
	{
        Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        mouse = new Vector3(mouse.x, mouse.y, 0);

        if (this.collider2D.bounds.Contains(mouse) && hasEntered) return;

        hasEntered = false;
		GUIManager.Instance.MouseOverUI = false;
		setTowerDetail (false);
	}

	// Use this for initialization
	void OnMouseDown()
    {
        if (GUIManager.Instance.OnTutorialScreen || Time.timeScale == 0) return;

		BaseCohesionManager.UnMarkAllAttachments();

        //only create new ones if we have none
        if (BlorbManager.Instance.HasEnoughResources(this.cost))
        {
            preSelected = true;

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
                Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                mouse = new Vector3(mouse.x, mouse.y, 0);

                if (rendererDisabled)
                {
                    if (this.tag != "Wall")
                    {
                        rangeIndicator.renderer.enabled = true;
                    }

                    foreach (Transform child in tempPlacement)
                    {
                        if (child.GetComponent<SpriteRenderer>() != null)
                        {
                            child.renderer.enabled = true;
                        }
                    }

                    rendererDisabled = false;
                }

                tempPlacement.transform.position = mouse;

                FindClosestPlacement(mouse);
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

    private void FindClosestPlacement(Vector3 mouse)
    {
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
        UnSelectAllPlacements();
        
        Center.Instance.DelayPlacementStopping();

        BaseCohesionManager.UnMarkAllAttachments();

        possiblePlacements = new ArrayList();

        if (GameObject.FindGameObjectsWithTag("Placement").Length > 0)
            Center.RemoveAllPossiblePlacements();

        GUIManager.Instance.UpdateTowerGUI();

        if (tempPlacement != null)
            Destroy(tempPlacement.gameObject);
        if (rangeIndicator != null)
            Destroy(rangeIndicator.gameObject);

    }

    static void UnSelectAllPlacements()
    {
        
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

        if (tempPlacement != null)
            Destroy(tempPlacement.gameObject);
        if (rangeIndicator != null)
            Destroy(rangeIndicator.gameObject);

        //create a piece that is shown to the user, a fake kinematic object
        switch (this.tag)
        {
            case "Turret":
                tempPlacement = Instantiate(turret) as Transform;
                rangeIndicator = Instantiate(range) as Transform;
                rangeIndicator.transform.parent = tempPlacement;
                break;

            case "Collector":
                tempPlacement = Instantiate(collector) as Transform;
                tempPlacement.GetComponentInChildren<ResourceCollector>().enabled = false;
                rangeIndicator = Instantiate(range) as Transform;
                rangeIndicator.transform.parent = tempPlacement;
                break;

            case "Wall":
                tempPlacement = Instantiate(wall) as Transform;
                if (rangeIndicator != null)
                    Destroy(rangeIndicator.gameObject);
                break;
        }

        if (this.tag != "Wall")
        {
            rangeIndicator.renderer.enabled = false;
        }

        foreach (Transform child in tempPlacement)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                child.renderer.enabled = false;
                child.GetComponent<SpriteRenderer>().sortingOrder -= 4;
                child.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
            }
        }

        rendererDisabled = true;

    }
}
