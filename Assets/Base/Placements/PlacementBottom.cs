using UnityEngine;
using System.Collections;

public class PlacementBottom : MonoBehaviour {

    public int spot;
    private const float pixelOffset = 100.0F;

    private Transform placementPiece;
    private bool checkDistance = false;
    private Color oldColor;

    void Update()
    {
        if (checkDistance)
        {
            float currDistance = Vector3.Distance(placementPiece.position, this.transform.position);
            float halfWidth = (this.GetComponent<SpriteRenderer>().sprite.rect.width / pixelOffset) / 2;
            if (currDistance < halfWidth)
            {
                //oldColor = this.GetComponent<SpriteRenderer>().color;
                //this.GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 255.0f);
                //we are closest to placement, so set snap position
                Placement.positionToSnap = this.transform.position;
                Placement.spot = spot;
                Placement.parent = this.transform.parent;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Turret" || 
            collider.tag == "Wall" ||
            collider.tag == "Collector")
        {
            placementPiece = collider.transform.parent;
            checkDistance = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        checkDistance = false;   
    }
}
