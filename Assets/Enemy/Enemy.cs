using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public static float hitSpeed = 1.0f;  //default to every second
    public static float hitDamage = 1.0f; //default to 1 hit
    public bool isHitting;
    public bool instantDamage; // true if the enemy deals one-time damage and disappears, false if damages until killed
	public TextMesh healthText;
	private float health = 100.0f;
	private float nextHit;
	private GameObject target;

    private float approxFrameRate = 1.0f / Time.deltaTime;

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
	        target.SendMessage("takeDamage", hitDamage);
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

    void OnCollisionEnter(Collision other)
    {
		if (other.gameObject.tag == "Player") { //only hit the player
			if (!instantDamage) {
				target = other.gameObject;
				isHitting = true; //stop jittery movement
			} else {
				other.gameObject.SendMessage("takeDamage", 10 * hitDamage);
				Destroy (this.gameObject);
			}
		}
    }

    void OnCollisionExit(Collision other)
    {
		if (other.gameObject == target) {
			isHitting = false;
		}
    }
}
