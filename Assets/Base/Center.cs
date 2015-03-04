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
	private Transform centerTurret;

    private const float placementOffset = 0.81f;
	private static float fireDelay = 0.25f;
	private float nextFireTime;

	public static float addHealthCost = 25f;

	private float healthInternal;

    public float speed;

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

        this.transform.FindChild("Player Bottom").renderer.enabled = true;
        this.transform.FindChild("Player Center").renderer.enabled = true;
        isActive = true;
	}

	// Use this for initialization
	void Start () {
		instance = this;
		GameEventManager.GameStart += GameStart;
		enabled = false;
        this.transform.FindChild("Player Bottom").renderer.enabled = false;
		centerTurret = this.transform.FindChild ("Player Center").transform;
		centerTurret.renderer.enabled = false;
		healthbar = this.transform.Find ("GUI/HUD/Health");
	}

	public void AddHealthButton()
	{
		if (!GUIManager.Instance.OnTutorialScreen && Time.timeScale != 0f &&
		    BlorbManager.Instance.BlorbAmount >= addHealthCost && health <= 90f) {
			BlorbManager.Instance.Transaction(-addHealthCost, transform.position);
			health += 10f;
		}
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
			Vector3 mouse = new Vector3(Input.mousePosition.x / (float)(Screen.width * 2f) - 1f,
			                            Input.mousePosition.y / (float)(Screen.height * 2f) - 1f,
			                            0f);
			
			Quaternion rotation = Quaternion.LookRotation(mouse, centerTurret.TransformDirection(Vector3.forward));
			centerTurret.rotation = new Quaternion(0, 0, rotation.z, rotation.w) * Quaternion.Euler(0, 0, -90);

			nextFireTime -= Time.deltaTime;
			
			if (nextFireTime < 0f && Input.GetMouseButton(0)) {
				GameObject g = ObjectPool.instance.GetObjectForType("Bullet", false);
				g.transform.position = transform.position;
				// get access to bullet component
				Bullet b = g.GetComponent<Bullet>();
				
				// set destination
				float angle = centerTurret.eulerAngles.z * Mathf.Deg2Rad;
				b.setDirection(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)));
				
				nextFireTime = fireDelay;
			}
		}
	}

    void FixedUpdate()
    {
        if (!isActive) return;
        
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(inputHorizontal, inputVertical, 0.0f);

		//rigidbody.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
        basePiece.position += movement * speed * Time.fixedDeltaTime;

        //if (BoundsIntersect(this.collider))
        //    basePiece.position -= movement * speed * Time.fixedDeltaTime;

        if (inputHorizontal != 0 || inputVertical != 0)
            GameObject.FindObjectsOfType<Placement>()[0].StopPlacement(); //quick hack to access from here...
    }

    private bool BoundsIntersect(Collider collider)
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag("Resource");
        GameObject[] mountains = GameObject.FindGameObjectsWithTag("Mountain");
        foreach (GameObject re in resources)
        {
            if (collider.bounds.Intersects(re.collider.bounds))
                return true;
        }

        foreach (GameObject mo in mountains)
        {
            if (collider.bounds.Intersects(mo.collider.bounds))
                return true;
        }

        return false;
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
			BlorbManager.Instance.Transaction(-placementPiece.cost, tower.transform.position);
        }
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
		return BlorbManager.Instance.BlorbAmount >= cost;
    }

    void OnCollisionEnter(Collision other)
    {
		if (other.gameObject.tag == "Enemy") { //only hit the player
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
			takeDamage(enemy.HitDamage);
			ObjectPool.instance.PoolObject(enemy.gameObject);
		}
    }
}
