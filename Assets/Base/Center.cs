using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour {

    private const float pixelOffset = 100.0F;

	private float health;

    public Transform tower;
    public Transform wall;
    public Transform bottom;

	void GameStart () {
		health = 100f;
		enabled = true;
	}

	void OnTriggerEnter (Collision2D collision) {
		gameObject.SetActive (false);
	}

	// Use this for initialization
	void Start () {
		GameEventManager.GameStart += GameStart;
		enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0f) {
			GameEventManager.TriggerGameOver();
		}

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //Debug.Log("Init Tower");

            //get the location of the top of the base
            float offset = GetOffsetToTop();

            //do the tower piece
            Transform childTower = Instantiate(tower) as Transform;
            childTower.transform.parent = this.transform;
            childTower.tag = "Tower";
            
            //do the bottom piece as a child of the tower
            Transform childBottom = Instantiate(bottom) as Transform;
            childBottom.transform.parent = childTower;
            childBottom.transform.localScale = Vector3.one;
            childBottom.tag = "Tower";

            //Debug.Log("TOWER TOP OFFSET: " + offset);
            childTower.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + offset);
            
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //Debug.Log("Init Tower");

            //get the location of the top of the base
            float offset = GetOffsetToRight();

            //do the tower piece
            Transform childTower = Instantiate(tower) as Transform;
            childTower.transform.parent = this.transform;
            childTower.tag = "Tower";

            //do the bottom piece as a child of the tower
            Transform childBottom = Instantiate(bottom) as Transform;
            childBottom.transform.parent = childTower;
            childBottom.transform.localScale = Vector3.one;
            childBottom.tag = "Tower";

            //Debug.Log("TOWER TOP OFFSET: " + offset);
            childTower.transform.position = new Vector3(this.transform.position.x + offset, this.transform.position.y);

        }
	}

    private float GetOffsetToTop()
    {
        SpriteRenderer bottomRenderer = this.GetComponentInChildren<SpriteRenderer>();
        //Debug.Log("HEIGHT: " + bottomRenderer.sprite.rect.height);
        return (bottomRenderer.sprite.rect.height / pixelOffset) + 1.8F;
    }

    private float GetOffsetToRight()
    {
        SpriteRenderer bottomRenderer = this.GetComponentInChildren<SpriteRenderer>();
        //Debug.Log("WIDTH: " + bottomRenderer.sprite.rect.width);
        return (bottomRenderer.sprite.rect.width / pixelOffset) + 2.1F;
    }

    public void Damage(float damage)
    {
        health -= damage;

        Debug.Log("HEALTH: " + health);
    }

}
