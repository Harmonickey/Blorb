using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {
	public SpriteRenderer gun;
	private Transform myTarget;
	
	void OnTriggerEnter2D (Collider2D other) {
		Debug.Log ("Trigger!");
		if (other.gameObject.tag == "Enemy" && !myTarget) {
			myTarget = other.gameObject.transform;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.transform == myTarget) {
			myTarget = null;
		}
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (myTarget) {
			//gun.transform.rotation = Quaternion.LookRotation (myTarget.position);
			Vector3 moveDirection = myTarget.position - gun.transform.position; 
			if (moveDirection != Vector3.zero) 
			{
				float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
				gun.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
			}
		}
	}
}
