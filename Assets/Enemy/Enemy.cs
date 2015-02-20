using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public Transform healthbar;
    public Center target;

    public float HitSpeed
    {
        get { return hitSpeed; }
    }

    private const float hitSpeed = 1.0f;  //default to every second

    public float HitDamage
    {
        get { return hitDamage; }
    }

    private const float hitDamage = 1.0f; //default to 1 hit

    public bool IsHitting
    {
        get { return isHitting; }
        set { isHitting = value; }
    }

    private bool isHitting;

    public bool InstantDamage
    {
        get { return instantDamage; }
    }

    private bool instantDamage; // true if the enemy deals one-time damage and disappears, false if damages until killed

    private float health = 100.0f;
    private float nextHit;
    private float totalDamage;

	// Use this for initialization
	void Start () {
        instantDamage = true;
        isHitting = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (!instantDamage && target && isHitting && nextHit < Time.time)
        {
	        target.takeDamage(hitDamage);
	        nextHit = Time.time + hitSpeed;
	        totalDamage += hitDamage;
        }
	}

	public void takeDamage (float amount) {
		health -= amount;
		healthbar.localScale = new Vector2 (health * 0.0015f, 0.15f);

		if (health <= 0f) {
			Destroy(this.gameObject);
			//tmp.SendMessage("receiveBlorb", 10);
		}
	}
}
