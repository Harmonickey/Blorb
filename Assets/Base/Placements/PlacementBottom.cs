using UnityEngine;
using System.Collections;

public class PlacementBottom : MonoBehaviour {

    private Transform placementPiece;

    public bool CheckDistance
    {
        set { checkDistance = value; }
    }

    private bool checkDistance = false;

    public int Spot
    {
        set { spot = value; }
    }

    private int spot;

    void Update()
    {
        if (checkDistance)
        {
            if (placementPiece == null)
            {
                checkDistance = false;
            }
            else
            {
                float currDistance = Vector3.Distance(placementPiece.position, this.transform.position);
                float halfWidth = (this.GetComponent<SpriteRenderer>().sprite.rect.width / WorldManager.PixelOffset);
                
                if (currDistance < halfWidth) //but also the shorter distance
                {
                    //we are closest to placement, so set snap position
                    Placement.positionToSnap = this.transform.position;
                    Placement.spot = spot;
                    Placement.parent = this.transform.parent;
                    this.GetComponent<SpriteRenderer>().color = new Color(0.516f, 0.886f, 0.882f, 0.9f);
                }
                else
                {
                    this.GetComponent<SpriteRenderer>().color = new Color(0.516f, 0.886f, 0.882f, 0);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Turret Placement" || 
            collider.tag == "Wall Placement" ||
            collider.tag == "Collector Placement")
        {
            placementPiece = collider.transform.parent;
            checkDistance = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        checkDistance = false;
        this.GetComponent<SpriteRenderer>().color = new Color(0.516f, 0.886f, 0.882f, 0);

        Placement.positionToSnap = Vector3.zero;
        Placement.spot = -1;
        Placement.parent = null;
    }
}
