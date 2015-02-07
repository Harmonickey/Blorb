using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour {

    private const float pixelOffset = 100.0F;
    private const float placementOffset = 0.6f;

	private float health;

    public Transform tower;
    public Transform wall;
    public Transform bottom;

    public static Transform selectedBasePiece;

    private bool selected = false;

    public bool[] takenSpots = new bool[4] {false, false, false, false};

    public bool canMove = false;
    public bool canBuild = false;

	void GameStart () {
		health = 100f;
		enabled = true;
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
            selectedBasePiece = this.transform;
            SpriteRenderer sr = selectedBasePiece.FindChild("Bottom").GetComponent<SpriteRenderer>();
            sr.color = new Color(0.0f, 219.0f, 255.0f); // "selected" color
            selected = true;
        }
    }

	// Use this for initialization
	void Start () {
		GameEventManager.GameStart += GameStart;
		//enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (selected && Center.selectedBasePiece != this.transform)
        {
            SpriteRenderer sr = this.transform.FindChild("Bottom").GetComponent<SpriteRenderer>();
            sr.color = new Color(255.0f, 255.0f, 255.0f); //return to original sprite color
            selected = false;
        }

        if (Center.selectedBasePiece != null && canBuild)
        {

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (SpotTaken(0))
                    return;
                
                PlacePiece("Tower", BuildDirection.Up); // no x direction, pos y direction

            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (SpotTaken(1))
                    return;

                PlacePiece("Tower", BuildDirection.Right); // pos x direction, no y direction

            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (SpotTaken(2))
                    return;

                PlacePiece("Tower", BuildDirection.Down); // no x direction, neg y direction

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (SpotTaken(3))
                    return;

                PlacePiece("Tower", BuildDirection.Left); // neg x direction, no y direction
                
            }
        }
	}

    private void PlacePiece(string tag, int[] direction)
    {
        float xDirection = (float)direction[0];
        float yDirection = (float)direction[1];
        //get the location of the top of the base
        float offset = GetOffsetHeight();

        //do the tower piece
        Transform childTower = Instantiate(tower) as Transform;
        childTower.transform.parent = selectedBasePiece;
        childTower.transform.localScale = Vector3.one;
        childTower.tag = tag;

        //do the bottom piece as a child of the tower
        Transform childBottom = Instantiate(bottom) as Transform;
        childBottom.transform.parent = childTower;
        childBottom.transform.localScale = Vector3.one;
        childBottom.tag = tag;

        float xOffset = (offset + placementOffset) * (float)xDirection;
        float yOffset = (offset + placementOffset) * (float)yDirection;

        childTower.transform.position = new Vector3(selectedBasePiece.position.x + xOffset, selectedBasePiece.position.y + yOffset);

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

		if (health <= 0f) {
			GameEventManager.TriggerGameOver();
		}

        Debug.Log("HEALTH: " + health);
    }

}

public abstract class BuildDirection
{
    public static int[] Up = new int[2] { 0, 1 };
    public static int[] Right = new int[2] { 1, 0 };
    public static int[] Down = new int[2] { 0, -1 };
    public static int[] Left = new int[2] { -1, 0 };
}
