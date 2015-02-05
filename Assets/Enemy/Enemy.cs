using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    public static float hitSpeed = 1.0f;
    public static float hitDamage = 1.0f;
    public bool isHitting = false;
    public Transform Player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("ENTERING COLLISION");
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("HIT BASE");
            Center playerGameObject = other.gameObject.GetComponentInParent<Center>();

            playerGameObject.Damage(hitDamage);
        }
        else if (other.gameObject.tag == "Tower")
        {
            Debug.Log("HIT TOWER");
            Attachments tower = other.gameObject.GetComponentInParent<Attachments>();

            tower.Damage(hitDamage);
        }

        isHitting = true; //stop jittery movement
    }

    void OnCollisionExit2D(Collision2D other)
    {
        Debug.Log("EXITING COLLISION");
        isHitting = false;
    }
}
