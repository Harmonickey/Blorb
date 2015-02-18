using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public float hitSpeed = 1.0f;  //default to every second
    public float hitDamage = 1.0f; //default to 1 hit
    public bool isHitting;
    public bool instantDamage; // true if the enemy deals one-time damage and disappears, false if damages until killed
	public TextMesh healthText;
	private float health = 100.0f;
	private float nextHit;
    public Center target;

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
		healthText.text = health.ToString();

		if (health <= 0f) {
			Destroy(this.gameObject);
			//tmp.SendMessage("receiveBlorb", 10);
		}
	}
}
