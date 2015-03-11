using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attachments : MonoBehaviour {
	public Transform HealthBarPrefab;
	private HealthBar healthbar;
    private float health = 100F;

    public bool wasFound = false; // for base cohesion checking

    public int cost;

    private List<Enemy> attackingEnemies = new List<Enemy>();

    public void takeDamage()
    {
        attackingEnemies.RemoveAll(e => e.gameObject == null); //remove all dead enemies

        float totalDamage = 0f;
        foreach (Enemy enemy in attackingEnemies)
            totalDamage += enemy.HitDamage;

        health -= totalDamage;

		healthbar.Set (health / 100f);

        if (health <= 0f)
        {
			CancelInvoke();
            //find all neighbors if possible (starting from center), skipping this particular attachment
            BaseCohesionManager.FindAllNeighbors(this.transform);
            BaseCohesionManager.DeleteUnconnectedAttachments(false); //delete all that were not found
            BaseCohesionManager.UnMarkAllAttachments();
        }
    }

    void Start()
    {
        GameEventManager.DayStart += DayStart;
        GameEventManager.GameOver += GameOver;

		Transform tmp = Instantiate (HealthBarPrefab, transform.position, transform.rotation) as Transform;
		
		tmp.parent = this.transform;
		healthbar = tmp.GetComponent<HealthBar> ();
		healthbar.HideWhenFull ();
		healthbar.Reset ();
    }

    void GameOver()
    {
        if (this.gameObject != null)
            Destroy(this.gameObject);
    }

    void DayStart()
    {
        if (this.gameObject != null)
            CancelInvoke();
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
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (attackingEnemies.Count == 0 && WaveManager.instance.enemyAgro) // only damage is enemy is in agro
                InvokeRepeating("takeDamage", 0, 1.0f); //start hitting every second

            attackingEnemies.Add(enemy);
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            attackingEnemies.Remove(enemy);
        }
    }
}
