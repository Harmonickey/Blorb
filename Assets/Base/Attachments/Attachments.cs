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
                if (!DetectOtherObjects(BuildDirection.ToDirFromSpot(i)))
                    centralController.SetPlacement(BuildDirection.ToDirFromSpot(i), this.transform);
            }
        }

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
        hits = Physics2D.RaycastAll(new Vector2(this.transform.position.x, this.transform.position.y), new Vector2(dir[0], dir[1]), 1.0f);

        //blacklist children and self
        ArrayList blackList = new ArrayList();
        foreach (Transform child in this.transform)
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
            if (blackList.Contains(j)) continue; //skip children

            return true;
        }

        return false;
    }
}
