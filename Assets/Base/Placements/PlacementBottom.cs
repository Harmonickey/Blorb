using UnityEngine;
using System.Collections;

public class PlacementBottom : MonoBehaviour {

    public int spot;
    private const float pixelOffset = 100.0F;

    private Transform placementPiece;
    public bool checkDistance = false;
    //private Color oldColor;

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
                //Debug.DrawLine(placementPiece.position, this.transform.position, Color.red, 100.0f, false);
                float halfWidth = (this.GetComponent<SpriteRenderer>().sprite.rect.width / pixelOffset) / 2;
                if (currDistance < halfWidth)
                {
                    //we are closest to placement, so set snap position
                    Placement.positionToSnap = this.transform.position;
                    Placement.spot = spot;
                    Placement.parent = this.transform.parent;
                    this.GetComponent<SpriteRenderer>().color = new Color(0.516f, 0.886f, 0.882f, 0.9f);
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
        this.GetComponent<SpriteRenderer>().color = new Color(0.516f, 0.886f, 0.882f, 0.5f);
    }
}
