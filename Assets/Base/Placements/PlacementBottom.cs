using UnityEngine;
using System.Collections;

public class PlacementBottom : MonoBehaviour {

    public Transform pseudoParent; //for detection logic

    public PlacementVote vote = new PlacementVote();

    void Start()
    {
        Placement.possiblePlacements.Add(this);
    }

    void Update()
    {
        /*
        Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        mouse = new Vector3(mouse.x, mouse.y, 0);
        Vector3 offsetDirection = GetOffset();
        float frontCurrDistance = Vector2.Distance(mouse, new Vector2(this.transform.position.x + (offsetDirection.x * 0.2f),
                                                                        this.transform.position.y + (offsetDirection.y * 0.2f)));
           
        float backCurrDistance = Vector2.Distance(mouse, new Vector2(this.transform.position.x - (offsetDirection.x * 0.2f),
                                                                        this.transform.position.y - (offsetDirection.y * 0.2f)));

        float selectionThreshold = (((this.GetComponent<SpriteRenderer>().sprite.rect.width * 2) / WorldManager.PixelOffset) / 2) + 0.1f;
            
        Debug.DrawLine(new Vector3(this.transform.position.x + (offsetDirection.x * 0.2f), this.transform.position.y + (offsetDirection.y * 0.2f)), 
                        new Vector3((this.transform.position.x + (offsetDirection.x * 0.2f)) + selectionThreshold,
                                    this.transform.position.y + (offsetDirection.y * 0.2f)), Color.red);
        Debug.DrawLine(new Vector3(this.transform.position.x - (offsetDirection.x * 0.2f), this.transform.position.y - (offsetDirection.y * 0.2f)), 
                        new Vector3((this.transform.position.x - (offsetDirection.x * 0.2f)) + selectionThreshold,
                                    this.transform.position.y - (offsetDirection.y * 0.2f)), Color.blue);
            
            
        vote.frontCurrDistance = frontCurrDistance;
        vote.backCurrDistance = backCurrDistance;
        vote.placement = this.transform;
        //we are closest to placement, so set snap position to this placement piece
        Placement.possiblePlacements.Add(vote);

        Transform closestPiece = GetClosest().placement;
        Placement.placementPiece.positionToSnap = closestPiece.localPosition;
        PlacementBottom[] srs = GameObject.FindObjectsOfType<PlacementBottom>();
        closestPiece.GetComponent<SpriteRenderer>().color = new Color(0.516f, 0.886f, 0.882f, 0.9f);

        Placement.possiblePlacements.Remove(vote);

        if (frontCurrDistance > selectionThreshold && backCurrDistance > selectionThreshold)
        {
            this.GetComponent<SpriteRenderer>().color = new Color(0.516f, 0.886f, 0.882f, 0);
            Placement.possiblePlacements.Remove(vote);
            vote = new PlacementVote();
        }
        */
    }

    private PlacementVote GetClosest()
    {
        //loop through all the possiblePlacements
        PlacementVote frontClosest = (PlacementVote)Placement.possiblePlacements[0];
        PlacementVote backClosest = (PlacementVote)Placement.possiblePlacements[0];
        //find the front-closest
        foreach (PlacementVote vote in Placement.possiblePlacements)
        {
            if (vote.frontCurrDistance < frontClosest.frontCurrDistance)
            {
                frontClosest = vote;
            }
        }

        //find the back-closest
        foreach (PlacementVote vote in Placement.possiblePlacements)
        {
            if (vote.backCurrDistance < backClosest.backCurrDistance)
            {
                backClosest = vote;
            }
        }

        return (frontClosest.frontCurrDistance < backClosest.backCurrDistance ? frontClosest : backClosest);
    }

    private Vector2 GetOffset()
    {
        Vector3 direction = this.transform.position - pseudoParent.transform.position;
        float angle = Vector2.Angle(Vector2.up, direction);
        Vector3 cross = Vector3.Cross(Vector2.up, direction);

        if (cross.z > 0)
            angle = 360 - angle;

        if (angle <= 45)
        {
            return Vector2.up;
        }

        if (angle >= 45 && angle <= 135)
        {
            return Vector2.right;
        }

        if (angle >= 135 && angle <= 225)
        {
            return Vector2.up * -1;
        }

        if (angle >= 225 && angle <= 325)
        {
            return Vector2.right * -1;
        }

        return Vector2.up;
    }
}

public class PlacementVote
{
    public Transform placement;
    public float currDistance;
    public float frontCurrDistance;
    public float backCurrDistance;
}
