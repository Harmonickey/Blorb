using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	private float speed = 10.0f;
	private float damage = 10.0f;
	private Vector3 direction;
	private float TTL;
	
	// Use this for initialization
	void Start () {
		TTL = 2f;
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Enemy") {
			other.gameObject.SendMessage("takeDamage", damage);
			ObjectPool.instance.PoolObject(gameObject);
		}
	}
	
	void Update () {
		TTL -= Time.deltaTime;
		// destroy bullet if destination does not exist anymore
		if (TTL <= 0f) {
			ObjectPool.instance.PoolObject(gameObject);
			return;
		}
		
		// fly towards the destination
		transform.position += direction * speed * Time.deltaTime;
	}

	public void setDirection(Vector3 d) {
		direction = d;
	}
}
