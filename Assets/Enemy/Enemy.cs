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

    void OnTriggerEnter2D(Collider2D other)
    {
        Center.Damage(hitDamage);
        isHitting = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        isHitting = false;
    }
}
