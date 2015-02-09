using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour {

    private const float pixelOffset = 100.0F;
    private const float placementOffset = -0.2f;

	private float health;

	public int resourcePool;
	public bool collectingFromResource = false;

    public Transform tower;
    public Transform wall;
	public Transform collector;
    public Transform bottom;
	public TextMesh healthText;
	public SpriteRenderer playerTopRenderer;

    public static Transform selectedBasePiece;

    private bool selected = false;

    public bool[] takenSpots = new bool[4] {false, false, false, false};

    public bool canMove = false;
    public bool canBuild = true; //TODO Change this to false when GameStart is fixed

	void GameStart () {
		health = 100f;
		resourcePool = 0;
		collectingFromResource = false;
		enabled = true;
		this.renderer.enabled = true;
		playerTopRenderer.enabled = true;
        canMove = false;
        canBuild = true;
	}

	void OnTriggerEnter (Collider collision) {
		gameObject.SetActive (false);
	}

    void OnMouseDown()
    {
        if (!selected && Input.GetButtonDown("Select"))
        {
            Debug.Log("Clicked on main base!");

            selectedBasePiece = this.transform;
            SpriteRenderer sr = selectedBasePiece.GetComponent<SpriteRenderer>();
            sr.color = new Color(0.0f, 219.0f, 255.0f); // "selected" color
            selected = true;
        }
    }

	// Use this for initialization
	void Start () {
		GameEventManager.GameStart += GameStart;
		enabled = false;
		this.renderer.enabled = false;
		playerTopRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (selected && Center.selectedBasePiece != this.transform)
        {
            SpriteRenderer sr = this.transform.GetComponent<SpriteRenderer>();
            sr.color = new Color(255.0f, 255.0f, 255.0f); //return to original sprite color
            selected = false;
        }

        canBuild = true;
        if (Center.selectedBasePiece != null && canBuild)
        {
            
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (SpotTaken(0))
                    return;

                Debug.Log("Building Turret");               
                PlacePiece("Turret", BuildDirection.Up); // no x direction, pos y direction

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (SpotTaken(1))
                    return;

                PlacePiece("Turret", BuildDirection.Right); // pos x direction, no y direction

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (SpotTaken(2))
                    return;

                PlacePiece("Turret", BuildDirection.Down); // no x direction, neg y direction

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (SpotTaken(3))
                    return;

                PlacePiece("Turret", BuildDirection.Left); // neg x direction, no y direction
            }
        }
	}

    private void PlacePiece(string tag, int[] direction)
    {
        float xDirection = (float)direction[0];
        float yDirection = (float)direction[1];
        //get the location of the top of the base
        float offset = GetOffsetHeight();

        Transform childBottom = Instantiate(bottom) as Transform;
        childBottom.transform.parent = selectedBasePiece;
        childBottom.transform.localScale = Vector3.one;
        childBottom.tag = tag;

        Transform childTower = Instantiate(tower) as Transform;
        childTower.transform.parent = childBottom;
        childTower.transform.localScale = Vector3.one;
        childTower.tag = tag;

        float xOffset = (offset + placementOffset) * (float)xDirection;
        float yOffset = (offset + placementOffset) * (float)yDirection;

        childBottom.transform.position = new Vector3(selectedBasePiece.position.x + xOffset, selectedBasePiece.position.y + yOffset);

    }

    private bool SpotTaken(int spot)
    {
        if (Center.selectedBasePiece != this.transform)
        {
            Attachments attachmentAttr = selectedBasePiece.GetComponent<Attachments>();
            if (attachmentAttr.takenSpots[spot] == true)
                return true;
            attachmentAttr.takenSpots[spot] = true;
        }
        else
        {
            if (takenSpots[spot] == true)
                return true;
            takenSpots[spot] = true;
        }
        return false;
    }

    private float GetOffsetHeight()
    {
        SpriteRenderer bottomRenderer = selectedBasePiece.GetComponentInChildren<SpriteRenderer>();
        //Debug.Log("HEIGHT: " + bottomRenderer.sprite.rect.height);
        return (bottomRenderer.sprite.rect.height / pixelOffset);// +1.8F;
    }

    private float GetOffsetWidth()
    {
        SpriteRenderer bottomRenderer = selectedBasePiece.GetComponentInChildren<SpriteRenderer>();
        //Debug.Log("WIDTH: " + bottomRenderer.sprite.rect.width);
        return (bottomRenderer.sprite.rect.width / pixelOffset);// +2.1F;
    }

    public void takeDamage(float damage)
    {
        health -= damage;
		healthText.text = health.ToString();

		if (health <= 0f) {
			GameEventManager.TriggerGameOver();
		}
    }
}

public abstract class BuildDirection
{
    public static int[] Up = new int[2] { 0, 1 };
    public static int[] Right = new int[2] { 1, 0 };
    public static int[] Down = new int[2] { 0, -1 };
    public static int[] Left = new int[2] { -1, 0 };
}
