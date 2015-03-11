using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public Transform HealthBarPrefab;
	private HealthBar healthbar;
	private float health;

    public float HitDamage {
		get {
			return hitDamage;
		}
	}

	private float hitDamage = 10f;
	private const int killValue = 5;

	void Start () {
		health = 100f;
		Transform tmp = Instantiate (HealthBarPrefab, transform.position, transform.rotation) as Transform;

		tmp.parent = this.transform;
		healthbar = tmp.GetComponent<HealthBar> ();
		healthbar.Reset ();
	}

	// called before the object is returned to the ObjectPool
	void Reset () {
		health = 100f;
		healthbar.Reset ();
		gameObject.GetComponent<SpriteRenderer>().color = new Color(0.874f, 0.914f, 0.525f);
	}

	public void takeDamage (float amount) {
		health -= amount;
		healthbar.Set (health / 100f);

		if (health <= 0f) {
			Reset ();
			ObjectPool.instance.PoolObject(this.gameObject);

			BlorbManager.Instance.Transaction(killValue, transform.position);
		}
	}
}
