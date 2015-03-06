using UnityEngine;
using System.Collections;

public class Attachments : MonoBehaviour {

    private float health = 100F;

    public bool wasFound = false; // for base cohesion checking

    public void takeDamage(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            //find all neighbors if possible (starting from center), skipping this particular attachment
            BaseCohesionManager.FindAllNeighbors(this.transform);
            BaseCohesionManager.DeleteAllBrokenAttachments(false); //delete all that were not found
            BaseCohesionManager.UnMarkAllAttachments();
        }
    }

    public void FindAllPossiblePlacements(Center center)
    {
        for (int i = 0; i < BuildingManager.Directions.Count; i++)
        {
            //check to make sure it's not going to place on another branch of the structure that
            //  isn't necessarily a parent or child
            if (!BuildingManager.DetectOtherObjects(BuildingManager.ToDirFromSpot(i), this.transform))
                center.SetPlacement(BuildingManager.ToDirFromSpot(i), this.transform);
        }

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
        { //only hit the player
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            takeDamage(enemy.HitDamage);
            ObjectPool.instance.PoolObject(enemy.gameObject);
        }
    }
}
