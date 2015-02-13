using UnityEngine;
using System.Collections;

public class Attachments : MonoBehaviour {

    private float health = 100F;

    public bool[] takenSpots = new bool[4] { false, false, false, false };

    public int spot;

    private Center center;

    public void takeDamage(float damage)
    {
        health -= damage;
    }

    public void FindAllPossiblePlacements(Center centralController)
    {
        center = centralController;
        for (int i = 0; i < takenSpots.Length; i++)
        {
            if (!takenSpots[i])
            {
                //check to make sure it's not going to place on another branch of the structure that
                //  isn't necessarily a parent or child
                Debug.Log("Start Dir: " + BuildDirection.ToDirFromSpot(i)[0] + ":" + BuildDirection.ToDirFromSpot(i)[1]);
                if (!DetectOtherObjects(BuildDirection.ToDirFromSpot(i)))
                    centralController.SetPlacement(BuildDirection.ToDirFromSpot(i), this.transform);
                
            }
        }
        Debug.Log("End Placement---------------");

        foreach (Transform child in this.transform)
        {
            if (child.GetComponent<Attachments>() != null)
            {
                child.GetComponent<Attachments>().FindAllPossiblePlacements(centralController);
            }
        }
    }

    private bool DetectOtherObjects(int[] dir)
    {
        //raycast to find objects
        RaycastHit2D[] hits;
        hits = Physics2D.LinecastAll(new Vector2(this.transform.position.x + dir[0], this.transform.position.y + dir[1]),
                                     new Vector2(this.transform.position.x + (dir[0] * 1.5f), this.transform.position.y + (dir[1] * 1.5f)));
        
        //blacklist children, self, and placement pieces
        ArrayList blackList = new ArrayList();
        foreach (Transform child in this.transform) //iterate only immediate children
        {
            for (int j = 0; j < hits.Length; j++)
            {
                if (hits[j].collider.gameObject == child.gameObject ||
                    hits[j].collider.gameObject == this.gameObject)
                {
                    blackList.Add(j);
                }
            }
        }

        for (int j = 0; j < hits.Length; j++)
        {
            if (blackList.Contains(j)) continue; //skip children and placement pieces

            return true;
        }

        return false;
    }
}
