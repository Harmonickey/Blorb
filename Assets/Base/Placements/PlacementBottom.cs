using UnityEngine;
using System.Collections;

public class PlacementBottom : MonoBehaviour {

    public int spot;

    private Transform placementPiece;
    private bool checkDistance = false;

    void Update()
    {
        if (checkDistance)
        {
            float currDistance = Vector3.Distance(placementPiece.position, this.transform.position);
            float halfWidth = this.GetComponent<SpriteRenderer>().sprite.rect.width / 2;
            if (currDistance < halfWidth)
            {
                //we are closest to placement, so set snap position
                Placement.positionToSnap = this.transform.position;
                Placement.spot = spot;
                Placement.parent = this.transform.parent;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag != "TestPiece")
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
