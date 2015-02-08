using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public static float hitSpeed = 1.0f;
    public static float hitDamage = 1.0f;
    public bool isHitting = false;
	public TextMesh healthText;
	private float health = 100.0f;
	private float nextHit;
	private GameObject target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (target && isHitting && nextHit < Time.time) {
			target.SendMessage("takeDamage", hitDamage);
			nextHit = Time.time + hitSpeed;
		}
	}

	public void takeDamage (float amount) {
		health -= amount;
		healthText.text = health.ToString();

		if (health <= 0f) {
			Destroy(this.gameObject);
		}
	}

    void OnCollisionEnter2D(Collision2D other)
    {
		if (other.gameObject.tag != "Enemy") {
			Debug.Log ("ENEMY HITTING");
			target = other.gameObject;

			isHitting = true; //stop jittery movement
		}
    }

    void OnCollisionExit2D(Collision2D other)
    {
		if (other.gameObject == target) {
			Debug.Log ("EXITING COLLISION");
			isHitting = false;
		}
    }
}
