using UnityEngine;
using System.Collections;

public class PlacementBottom : MonoBehaviour {

    public Transform pseudoParent; //for detection logic

    void Update()
    {
        //if (checkDistance)
        //{
            
            Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            float currDistance = Vector2.Distance(mouse, this.transform.position);
            
            float halfWidth = (this.GetComponent<SpriteRenderer>().sprite.rect.width / WorldManager.PixelOffset);
            
            if (currDistance < halfWidth) //only set the placement piece value if we're the closest...
            {
                //we are closest to placement, so set snap position to this placement piece
                Placement.placementPiece.positionToSnap = this.transform.localPosition;
                this.GetComponent<SpriteRenderer>().color = new Color(0.516f, 0.886f, 0.882f, 0.9f);
            }
            else
            {
                this.GetComponent<SpriteRenderer>().color = new Color(0.516f, 0.886f, 0.882f, 0);
                //Placement.placementPiece.positionToSnap = Vector3.zero;
            }
        //}
    }

    void OnMouseEnter()
    {
        //checkDistance = true; //because we could be within multiple tower spots
    }

    void OnMouseExit()
    {
        //checkDistance = false;
        //this.GetComponent<SpriteRenderer>().color = new Color(0.516f, 0.886f, 0.882f, 0);
    }
}
