using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour {

    private const float pixelOffset = 100.0F;
    private const float placementOffset = -0.2f;

	private float health;

    public float speed;
	public int resourcePool;
	public bool collectingFromResource = false;

    public Transform turret;
    public Transform wall;
	public Transform collector;
    public Transform bottom;
    public Transform placement;
	public TextMesh healthText;

    public bool[] takenSpots = new bool[4] {false, false, false, false};

    public bool canMove = false;
    public bool canBuild = true; //TODO Change this to false when GameStart is fixed

	void GameStart () {
		health = 100f;
		resourcePool = 0;
		collectingFromResource = false;
		enabled = true;
		this.renderer.enabled = true;
        this.transform.FindChild("Player").renderer.enabled = true;
        canMove = true;
        canBuild = true;
	}

	void OnTriggerEnter (Collider collision) {
		gameObject.SetActive (false);
	}

	// Use this for initialization
	void Start () {
		GameEventManager.GameStart += GameStart;
		enabled = false;
		this.renderer.enabled = false;
        this.transform.FindChild("Player").renderer.enabled = false;
	}

    public void FindAllPossiblePlacements()
    {
        //start with the center and iterate through children
        for (int i = 0; i < takenSpots.Length; i++)
        {
            if (!takenSpots[i])
            {
                //place down a placement spot
                SetPlacement(BuildDirection.ToDirFromSpot(i), this.transform);
            }
        }

        foreach (Transform child in this.transform)
        {
            if (child.GetComponent<Attachments>() != null)
            {
                child.GetComponent<Attachments>().FindAllPossiblePlacements(this);
            }
        }
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

    public void SetPlacement(int[] dir, Transform parent)
    {
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
        PlacePiece("Placement", dir, parent, true);
    }

    void FixedUpdate()
    {
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(inputHorizontal, inputVertical, 0.0f);

        this.transform.position += movement * speed * Time.deltaTime;
        
    }

    public void PlacePiece(string tag, int[] direction, Transform selectedBasePiece, bool isOkay = true)
    {
        float xDirection = (float)direction[0];
        float yDirection = (float)direction[1];

        //get the location of the top of the base
        float offset = GetOffset(selectedBasePiece);
        Transform piece = placement;
        Transform tempBottom = bottom;
        Color color = new Color(255.0f, 255.0f, 255.0f); //normal color

        switch (tag)
        {
            case "Placement":
                tempBottom = placement;
                color = (isOkay ? new Color(0.0f, 255.0f, 0.0f, 200.0f) : new Color(255.0f, 0.0f, 0.0f, 200.0f));
                break;
            case "Turret":
            case "TestPiece":
                piece = turret;
                break;
            case "Wall":
                piece = wall;
                break;
            case "Collector":
                piece = collector;
                break;
        }

        Transform childBottom = Instantiate(tempBottom) as Transform;
        childBottom.transform.parent = selectedBasePiece;
        childBottom.transform.localScale = Vector3.one;
        childBottom.GetComponent<SpriteRenderer>().color = color;
        childBottom.tag = tag;

        if (tag != "Placement")
            childBottom.GetComponent<Attachments>().spot = BuildDirection.ToSpotFromDir(direction);
        if (tag == "Placement")
            childBottom.GetComponent<PlacementBottom>().spot = BuildDirection.ToSpotFromDir(direction);

        if (tag != "Placement")
        {
            if (selectedBasePiece == this.transform) //if we're actually the center
                ReserveSpot(BuildDirection.ToSpotFromDir(direction), selectedBasePiece.GetComponent<Center>());
            else
                ReserveSpot(BuildDirection.ToSpotFromDir(direction), selectedBasePiece.GetComponent<Attachments>());

            ReserveSpot(BuildDirection.OppositeDirection(BuildDirection.ToSpotFromDir(direction)), childBottom.GetComponentInChildren<Attachments>());

            //here we need a switch for different towers
            Transform childTower = Instantiate(piece) as Transform;
            childTower.transform.parent = childBottom;
            childTower.transform.localScale = Vector3.one;
            childTower.tag = tag;
        }

        float xOffset = (offset + placementOffset) * (float)xDirection;
        float yOffset = (offset + placementOffset) * (float)yDirection;

        childBottom.transform.position = new Vector3(selectedBasePiece.position.x + xOffset, selectedBasePiece.position.y + yOffset);

    }

    void RemovePiece(int[] dir, Transform parent)
    {
        Attachments[] attachments = parent.GetComponentsInChildren<Attachments>();

        foreach (Attachments attachment in attachments)
        {
            if (attachment.spot == BuildDirection.ToSpotFromDir(dir) &&
                attachment.gameObject.GetInstanceID() != parent.gameObject.GetInstanceID())
            {
                Destroy(attachment);
                break;
            }
        }
    }

    private void ReserveSpot(int dir, Attachments obj)
    {
        obj.takenSpots[dir] = true;
    }

    private void ReserveSpot(int dir, Center obj)
    {
        obj.takenSpots[dir] = true;
    }

    private float GetOffset(Transform selectedBasePiece)
    {
        SpriteRenderer bottomRenderer = selectedBasePiece.GetComponentInChildren<SpriteRenderer>();;
        return (bottomRenderer.sprite.rect.height / pixelOffset); //height or width works because they're equivalent
    }

    public void takeDamage(float damage)
    {
        health -= damage;
		healthText.text = health.ToString();

		if (health <= 0f) {
			GameEventManager.TriggerGameOver();
		}
    }

    private bool HasPathToCenter()
    {
        Pathfinding2D finder = this.gameObject.GetComponent<Pathfinding2D>();
        finder.FindPath(new Vector3(20.0f + this.transform.position.x, 20.0f + this.transform.position.y, 0.0f), this.transform.position);

        if (finder.Path.Count > 0)
            return true;

        return false;
    }
}

public abstract class BuildDirection
{
    public static int[] Up = new int[2] { 0, 1 };  //0
    public static int[] Right = new int[2] { 1, 0 };  //1
    public static int[] Down = new int[2] { 0, -1 };  //2
    public static int[] Left = new int[2] { -1, 0 };  //3

    public static bool IsSameDir(int[] l, int[] r)
    {
        if (l[0] == r[0] && l[1] == r[1]) 
           return true;

        return false;
    }
    
    public static int OppositeDirection(int dir)
    {
        return (dir + 2) % 4;
    }

    public static int ToSpotFromDir(int[] dir)
    {
        if (IsSameDir(dir, Up))
        {
            return 0;
        }
        else if (IsSameDir(dir, Right))
        {
            return 1;
        }
        else if (IsSameDir(dir, Down))
        {
            return 2;
        }

        return 3;
    }

    public static int[] ToDirFromSpot(int spot)
    {
        if (spot == 1)
        {
            return BuildDirection.Right;
        }
        else if (spot == 2)
        {
            return BuildDirection.Down;
        }
        else if (spot == 3)
        {
            return BuildDirection.Left;
        }
        
        return BuildDirection.Up;
    }
}
