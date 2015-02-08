using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public static float hitSpeed = 1.0f;
    public static float hitDamage = 1.0f;
    public bool isHitting = false;
	public TextMesh healthText;
	private float health = 100.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
        Debug.Log("ENEMY HITTING");

        other.gameObject.SendMessage("takeDamage", hitDamage);

        isHitting = true; //stop jittery movement
    }

    void OnCollisionExit2D(Collision2D other)
    {
        Debug.Log("EXITING COLLISION");
        isHitting = false;
    }
}
