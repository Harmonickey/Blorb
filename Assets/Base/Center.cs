using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour {
	public static Center Instance
	{
		get { return instance; }
	}
	private static Center instance;

	public GameObject bullet;
    public Transform basePiece;
    public Transform turret;
    public Transform wall;
    public Transform collector;
    public Transform placement;
    public Transform healthbar;
	public Transform centerTurret;
    public Transform playerBottom;

    private const float placementOffset = 0.81f;
	private float turretDamage = 10f;
	private float fireDelay = 0.2f;
	private float nextFireTime;
	
	private float healthInternal;

    public float speed;

    public bool HasPath;
    public bool PathIsSet;

	public float health
	{
		get { return healthInternal;}
		set {
			healthInternal = value;
			healthbar.localScale = new Vector2 (health, 1f);
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
		collectingFromResource = false;

        playerBottom.renderer.enabled = true;
        centerTurret.renderer.enabled = true;
        isActive = true;
	}

    void GameOver() {
        enabled = false;
        playerBottom.renderer.enabled = false;
        centerTurret.renderer.enabled = false;
    }

	// Use this for initialization
	void Start () {
		instance = this;
		GameEventManager.GameStart += GameStart;
        GameEventManager.GameOver += GameOver;
		enabled = false;
        playerBottom.renderer.enabled = false;
		centerTurret.renderer.enabled = false;

        HasPath = false;
        PathIsSet = false;
	}

    public void FindAllPossiblePlacements()
    {
        //start with the center
        for (int i = 0; i < BuildingManager.Directions.Count; i++)
            if (!BuildingManager.DetectOtherObjects(BuildingManager.ToDirFromSpot(i), this.transform))
                SetPlacement(BuildingManager.ToDirFromSpot(i), this.transform);

        //go through all the children
        foreach (Transform child in basePiece)
            if (child != null && child.gameObject.activeSelf && child.GetComponent<Attachments>() != null)
                child.GetComponent<Attachments>().FindAllPossiblePlacements(this);

    }

    public static void RemoveAllPossiblePlacements()
    {
        GameObject[] placements = GameObject.FindGameObjectsWithTag("Placement");
        //just destroying all placements is fine for now
        foreach (GameObject placement in placements)
        {
            Destroy(placement);
        }
    }

    //simply function to do both tasks at once
    public void RecalculateAllPossiblePlacements()
    {
        RemoveAllPossiblePlacements();
        FindAllPossiblePlacements();
    }

    public void SetPlacement(float[] dir, Transform parent)
    {
        PlacePiece(new PlacementPiece("Placement", dir, parent));
    }

	void Update()
	{
		if (!WorldManager.instance.isDay && !GUIManager.Instance.OnTutorialScreen) {
			Vector3 mouse = new Vector3(Input.mousePosition.x - Screen.width / 2f,
			                            Input.mousePosition.y - Screen.height / 2f,
			                            0f);
			
			Quaternion rotation = Quaternion.LookRotation(mouse.normalized, Vector3.forward);
			centerTurret.rotation = new Quaternion(0, 0, rotation.z, rotation.w) * Quaternion.Euler(0, 0, -90);

			nextFireTime -= Time.deltaTime;

			if (nextFireTime < 0f && Input.GetMouseButton(0) && !GUIManager.Instance.MouseOverUI) {
				GameObject g = ObjectPool.instance.GetObjectForType("Bullet", false);
				g.transform.position = transform.position;
				// get access to bullet component
				Bullet b = g.GetComponent<Bullet>();
				
				// set destination
				float angle = centerTurret.eulerAngles.z * Mathf.Deg2Rad;
				b.setDirection(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)));
				b.setDamage(turretDamage);
                this.audio.Play(); //make a bullet sound
				
				nextFireTime = fireDelay;
			}
		}

        if (PathIsSet)
        {
            WaveManager.instance.PreWaveInit(HasPath);
            
            HasPath = false;
            PathIsSet = false;
        }
	}

    void FixedUpdate()
    {
        if (!isActive) return;
        
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(inputHorizontal, inputVertical, 0.0f);

		basePiece.position += movement * speed * Time.fixedDeltaTime;

        if (inputHorizontal != 0 || inputVertical != 0)
            Placement.StopPlacement();
    }

    public void PlacePiece(PlacementPiece placementPiece, Placement selectedTower = null)
    {
        if (!isActive) return;

        //get the location of the top of the base
        Color color = new Color(1f, 1f, 1f); //normal color

        switch (placementPiece.type)
        {
            case "Placement":
                placementPiece.piece = placement;
                color = PlacementBottom.unSelectedColor;
                break;
            case "Turret":
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
            tower.GetComponent<SpriteRenderer>().receiveShadows = false;

        } else {
			tower.GetComponent<Attachments>().cost = placementPiece.cost;
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
            tower.GetComponent<FixedJoint>().anchor = tower.transform.localPosition;
            tower.GetComponent<FixedJoint>().connectedAnchor = tower.transform.localPosition;
            tower.GetComponent<FixedJoint>().connectedBody = this.GetComponent<Rigidbody>();
			BlorbManager.Instance.Transaction(-placementPiece.cost, tower.transform.position, selectedTower);
        }
    }

    private float GetPixelOffset()
    {
        SpriteRenderer bottomRenderer = playerBottom.GetComponent<SpriteRenderer>();

        return (bottomRenderer.sprite.rect.width / WorldManager.PixelOffset); 
    }

    public void takeDamage(float damage)
    {
        health -= damage;

		if (health <= 0f) {
			GameEventManager.TriggerGameOver();
		}
    }

    public void HasPathToCenter()
    {
        //spawn an enemy
        Pathfinding2D finder = this.GetComponent<Pathfinding2D>();
        float randAngle = Random.Range(0f, 2 * Mathf.PI);
        Vector3 endPos;

        //make sure we're not on a disallowed piece, we want to be able to find a valid path
        do {
            endPos = this.transform.position + 10f * new Vector3(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
        } while (IsWithinDisallowedObject(endPos));


        finder.FindPath(this.transform.position, endPos, true);

        //asynchronous, need to wait for message...
    }

    public void SetHasPath(bool hasPath)
    {
        this.HasPath = hasPath;

        PathIsSet = true;
    }

    void OnCollisionEnter(Collision other)
    {
		if (other.gameObject.tag == "Enemy") { //only hit the player
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
			takeDamage(enemy.HitDamage);
			ObjectPool.instance.PoolObject(enemy.gameObject);
		}
    }

    static IEnumerator DelayAfterStoppingPlacement()
    {
        yield return new WaitForSeconds(0.01f);
        Placement.isPlacingTowers = false;
    }

    public void DelayPlacementStopping()
    {
        StartCoroutine(DelayAfterStoppingPlacement());
    }

    private bool IsWithinDisallowedObject(Vector3 endPos)
    {
        GameObject[] mountains = GameObject.FindGameObjectsWithTag("Mountain");
        GameObject[] resources = GameObject.FindGameObjectsWithTag("Resource");

        foreach (GameObject mountain in mountains)
        {
            if (mountain.collider.bounds.Contains(endPos))
            {
                return true;
            }
        }

        foreach (GameObject resource in resources)
        {
            if (resource.collider.bounds.Contains(endPos))
            {
                return true;
            }
        }

        return false;
    }
}
