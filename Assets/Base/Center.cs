using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour {
	public GameObject bullet;
    public Transform basePiece;
    public Transform turret;
    public Transform wall;
    public Transform collector;
    public Transform placement;
    public Transform healthbar;
	private Transform centerTurret;
    public TextMesh resourcePoolText;
	public GameObject blorbIndicator;

    private const float placementOffset = 0.725f;
	private static float fireDelay = 0.25f;
	private float nextFireTime;

	private float healthInternal;
	private float resourcesInternal;

    public float speed;

	public float health
	{
		get { return healthInternal;}
		set {
			healthInternal = value;
			healthbar.localScale = new Vector2 (health, 1f);
		}
	}

	public float blorbAmount
	{ 
		//made property so updates text dynamically
		get {return resourcesInternal;}
		set {
			float diff = value - resourcesInternal;
			resourcesInternal = value; 
			resourcePoolText.text = ((int)resourcesInternal).ToString();
			GUIManager.UpdateTowerGUI(blorbAmount);

			if (Mathf.Abs(diff) > 0f) {
				// Add blorb indicator
				GameObject g = (GameObject)Instantiate(blorbIndicator, resourcePoolText.transform.position, Quaternion.identity);
				g.transform.parent = resourcePoolText.transform;
				BlorbIndicator b = g.GetComponent<BlorbIndicator>();
				b.setDiff(diff);
			}
		}
	}

    public bool CollectingFromResource
    {
        get { return collectingFromResource; }
    }

	private bool collectingFromResource = false;

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

    private bool isActive;

	void GameStart () {
		enabled = true;

		health = 100f;

		blorbAmount = 100f;
		//resourcePoolText.text = resourcePool.ToString ();
		collectingFromResource = false;

        this.transform.FindChild("Player Bottom").renderer.enabled = true;
        this.transform.FindChild("Player Center").renderer.enabled = true;
        isActive = true;
	}

	void OnTriggerEnter (Collider collision) {
		gameObject.SetActive (false);
	}

	// Use this for initialization
	void Start () {
		GameEventManager.GameStart += GameStart;
		enabled = false;
        this.transform.FindChild("Player Bottom").renderer.enabled = false;
		centerTurret = this.transform.FindChild ("Player Center").transform;
		centerTurret.renderer.enabled = false;
		healthbar = this.transform.Find ("GUI/HUD/Health");

		resourcePoolText.renderer.sortingLayerName = "UI";
		resourcePoolText.renderer.sortingOrder = 2;
	}

    public void FindAllPossiblePlacements()
    {
        //start with the center
        for (int i = 0; i < BuildDirection.Directions.Count; i++)
        {
            //place down a placement spot, don't have to worry about 
            if (!BuildDirection.DetectOtherObjects(BuildDirection.ToDirFromSpot(i), this.transform))
                SetPlacement(BuildDirection.ToDirFromSpot(i), this.transform);
        }

        foreach (Transform child in basePiece)
            if (child.GetComponent<Attachments>() != null)
                child.GetComponent<Attachments>().FindAllPossiblePlacements(this);

    }

    public void RemoveAllPossiblePlacements()
    {
        GameObject[] placements = GameObject.FindGameObjectsWithTag("Placement");

        foreach (GameObject placement in placements)
        {
            Destroy(placement);
        }
    }

    public void RecalculateAllPossiblePlacements()
    {
        RemoveAllPossiblePlacements();
        FindAllPossiblePlacements();
    }

    public void SetPlacement(float[] dir, Transform parent)
    {
        PlacePiece(new PlacementPiece("Placement", dir, parent));

        //check if there is a valid path to the center
        /*
        PlacePiece("TestPiece", dir, parent, true);
        if (HasPathToCenter())
        {
            Debug.Log("HAS PATH");
            RemovePiece(dir, parent);
            PlacePiece("Placement", dir, parent, true);
        }
        else
        {
            RemovePiece(dir, parent);
            PlacePiece("Placement", dir, parent, false);
        }
         * */
        //Debug.Log("Has Path: " + HasPathToCenter());
        
    }

	void Update()
	{
		if (!WorldManager.instance.isDay) {
			nextFireTime -= Time.deltaTime;
			
			if (nextFireTime < 0f && Input.GetMouseButton(0)) {
				GameObject g = (GameObject)Instantiate(bullet, transform.position, Quaternion.identity);
				
				// get access to bullet component
				Bullet b = g.GetComponent<Bullet>();
				
				// set destination
				float angle = centerTurret.eulerAngles.z * Mathf.Deg2Rad;
				b.setDirection(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)));
				
				nextFireTime = fireDelay;
			}
			
			Vector3 lookTarget = new Vector3();
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast (ray, out hit)) { 
				lookTarget = hit.point; 
			}
			
			Quaternion rotation = Quaternion.LookRotation(lookTarget, centerTurret.TransformDirection(Vector3.forward));
			centerTurret.rotation = new Quaternion(0, 0, rotation.z, rotation.w) * Quaternion.Euler(0, 0, -90);
		}
	}

    void FixedUpdate()
    {
        if (!isActive) return;
        
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(inputHorizontal, inputVertical, 0.0f);

        basePiece.position += movement * speed * Time.deltaTime;

        if (inputHorizontal != 0 || inputVertical != 0)
            GameObject.FindObjectsOfType<Placement>()[0].StopPlacement(); //quick hack to access from here...
    }

    public void PlacePiece(PlacementPiece placementPiece)
    {
        if (!isActive) return;

        //get the location of the top of the base
        Color color = new Color(1f, 1f, 1f); //normal color

        switch (placementPiece.type)
        {
            case "Placement":
                placementPiece.piece = placement;
                color = new Color(0.516f, 0.886f, 0.882f, 0); 
                break;
            case "Turret":
            case "TestPiece":
                placementPiece.piece = turret;
                break;
            case "Wall":
                placementPiece.piece = wall;
                break;
            case "Collector":
                placementPiece.piece = collector;
                break;
        }
    
        //create the placement piece
        Transform tower = Instantiate(placementPiece.piece) as Transform;
        tower.tag = placementPiece.type;
        tower.transform.parent = basePiece;

        if (placementPiece.type == "Placement")
        {
            tower.GetComponent<SpriteRenderer>().color = color;
            tower.GetComponent<PlacementBottom>().pseudoParent = placementPiece.parent;
        }

        float xOffset = (placementPiece.type == "Placement"
            ? (placementPiece.parent.transform.localPosition.x  //get offset from nearest parent
              + (GetPixelOffset() + placementOffset) 
              * placementPiece.direction[0])
            : placementPiece.positionToSnap.x);  //or just place it exactly where there was a placement piece

        float yOffset = (placementPiece.type == "Placement" 
            ? (placementPiece.parent.transform.localPosition.y
              + (GetPixelOffset() + placementOffset) 
              * placementPiece.direction[1])
            : placementPiece.positionToSnap.y);

        tower.transform.localPosition = new Vector3(xOffset, yOffset);

        if (placementPiece.type != "Placement")
        {
            tower.GetComponent<FixedJoint>().connectedBody = this.GetComponent<Rigidbody>();
        }

        if (placementPiece.type == "Placement")
            blorbAmount -= placementPiece.cost; //can pass in tower.transform.position here for floating blorb amount
		//resourcePoolText.text = resourcePool.ToString ();
    }

    void RemovePiece(float[] dir, Transform parent)
    {
        return;
    }

    private float GetPixelOffset()
    {
        SpriteRenderer bottomRenderer = this.transform.FindChild("Player Bottom").GetComponent<SpriteRenderer>();

        return (bottomRenderer.sprite.rect.width / WorldManager.PixelOffset); 
    }

    public void takeDamage(float damage)
    {
        health -= damage;

		if (health <= 0f) {
			GameEventManager.TriggerGameOver();
		}
    }

	public void receiveBlorb(float blorb)
	{
		blorbAmount += blorb;
		//resourcePoolText.text = resourcePool.ToString ();
	}

    private bool HasPathToCenter()
    {
        Pathfinding2D finder = this.gameObject.GetComponent<Pathfinding2D>();
        finder.FindPath(new Vector3(20.0f + this.transform.position.x, 20.0f + this.transform.position.y, 0.0f), this.transform.position);
        Debug.DrawLine(new Vector3(20.0f + this.transform.position.x, 20.0f + this.transform.position.y, 0.0f),
                       this.transform.position, Color.red, 100.0f, false);

        if (finder.Path.Count > 0)
            return true;

        return false;
    }

    public bool HasEnoughResources(int cost)
    {
		return blorbAmount >= cost;
    }

    void OnCollisionEnter(Collision other)
    {
		if (other.gameObject.tag == "Enemy") { //only hit the player
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
			takeDamage(enemy.HitDamage);
			Destroy (enemy.gameObject);
		}
    }
}
