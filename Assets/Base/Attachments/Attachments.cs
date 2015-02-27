using UnityEngine;
using System.Collections;

public class Attachments : MonoBehaviour {

    private float health = 100F;

    public bool wasFound = false; // for base cohesion checking

    public void takeDamage(float damage)
    {
        health -= damage;
    }

    public void FindAllPossiblePlacements(Center center)
    {

        for (int i = 0; i < BuildDirection.Directions.Count; i++)
        {
            //check to make sure it's not going to place on another branch of the structure that
            //  isn't necessarily a parent or child
            if (!BuildDirection.DetectOtherObjects(BuildDirection.ToDirFromSpot(i), this.transform))
                center.SetPlacement(BuildDirection.ToDirFromSpot(i), this.transform);
        }

    }

    void OnMouseDown()
    {
        // if we're not placing pieces (maybe change this later...)
        if (GameObject.FindGameObjectsWithTag("Placement").Length == 0)
            BaseCohesion.FindAllNeighbors(null, this.transform); // find out our base cohesion network
    }
}
