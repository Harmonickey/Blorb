using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	private float speed = 30.0f;
	private float damage = 10.0f;
	private Vector3 velocity;
	private float TTL = 2f;

	// called before the object is returned to the ObjectPool
	void Reset () {
		TTL = 2f;
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Enemy") {
			other.gameObject.SendMessage("takeDamage", damage);
			Reset ();
			ObjectPool.instance.PoolObject(gameObject);
		}
	}
	
	void FixedUpdate () {
		TTL -= Time.fixedDeltaTime;
		// destroy bullet if destination does not exist anymore
		if (TTL <= 0f) {
			Reset ();
			ObjectPool.instance.PoolObject(gameObject);
			return;
		}

		transform.position += velocity * Time.fixedDeltaTime;
	}

	public void setDirection(Vector3 d) {
		velocity = speed * Vector3.Normalize(d);
	}
}
