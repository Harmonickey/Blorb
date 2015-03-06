using UnityEngine;
using System.Collections;

public class PlacementBottom : MonoBehaviour {

    public Transform pseudoParent; //for detection logic

    public static Color selectedColor = new Color(0.2f, 0.2f, 0.882f, 1f);
    public static Color unSelectedColor = new Color(0.516f, 0.886f, 0.882f, 1f);

    public PlacementVote vote = new PlacementVote();

    void Start()
    {
        Placement.possiblePlacements.Add(this);
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
