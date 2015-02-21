using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	private float speed = 10.0f;
	private float damage = 10.0f;
	private Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {
		// destroy bullet if destination does not exist anymore
		if (target == null) {
			Destroy(gameObject);
			return;
		}
		
		// fly towards the destination
		float stepSize = Time.deltaTime * speed;
		transform.position = Vector3.MoveTowards(transform.position, target.position, stepSize);
		
		// reached?
		if (transform.position.Equals(target.position)) {
			// decrease teddy health
			Enemy e = target.GetComponent<Enemy>();
			e.takeDamage(damage);          
			
			// destroy bullet
			Destroy(gameObject);
		}
	}
	
	public void setTarget(Transform v) {
		target = v;
	}
}
