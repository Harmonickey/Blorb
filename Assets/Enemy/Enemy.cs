using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public static float hitSpeed = 1.0f;  //default to every second
    public static float hitDamage = 1.0f; //default to 1 hit
    public bool isHitting;
    public bool instantDamage;
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

        if (target && isHitting && nextHit < Time.time)
        {
            Debug.Log(instantDamage);
            if (instantDamage) //hit every hitSpeed<second>
            {
                target.SendMessage("takeDamage", hitDamage);
                nextHit = Time.time + hitSpeed;
                totalDamage += hitDamage;
            }
            else //hit every frame to eventually amount to the hitDamage every hitSpeed<second>
            {
                target.SendMessage("takeDamage", (hitDamage / approxFrameRate));
                nextHit = Time.time + (hitSpeed / approxFrameRate);
                totalDamage += (hitDamage / approxFrameRate);
            }
            //Debug.Log(totalDamage);
        }
	}

	public void takeDamage (float amount) {
		health -= amount;
		healthText.text = health.ToString();

		if (health <= 0f) {
			Destroy(this.gameObject);
		}
	}

    void OnCollisionEnter(Collision other)
    {
		if (other.gameObject.tag == "Player") { //only hit the player

			target = other.gameObject;
			isHitting = true; //stop jittery movement

		}
    }

    void OnCollisionExit(Collision other)
    {
		if (other.gameObject == target) {
			isHitting = false;
		}
    }
}
