using UnityEngine;
using System.Collections;

public class Attachments : MonoBehaviour {

    private float health = 100F;
    private bool selected = false;

    public bool[] takenSpots = new bool[4] { false, false, false, false };

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
        if (selected && Center.selectedBasePiece != this.transform)
        {
            SpriteRenderer sr = this.transform.GetComponent<SpriteRenderer>();
            sr.color = new Color(255.0f, 255.0f, 255.0f);
            selected = false;
        }
	}

    void OnMouseDown()
    {
        if (!selected && Input.GetButtonDown("Select"))
        {
            Debug.Log("CLICKED ON ATTACHMENT!");
           
            Center.selectedBasePiece = this.transform;
            SpriteRenderer sr = Center.selectedBasePiece.FindChild("Bottom(Clone)").GetComponent<SpriteRenderer>();
            sr.color = new Color(0.0f, 219.0f, 255.0f);
            selected = true;
        }
    }

    public void takeDamage(float damage)
    {
        health -= damage;

        Debug.Log("ATTACHMENT HEALTH: " + health);
    }
}
