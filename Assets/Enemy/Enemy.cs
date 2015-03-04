using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public Transform HealthbarPrefab;
	private HealthBar healthbar;
	private float health;

    public float HitDamage {
		get {
			return hitDamage;
		}
	}

	private float hitDamage = 10f;
	private const float killValue = 10f;

	void Start () {
		health = 100f;
		Transform tmp = Instantiate (HealthbarPrefab, transform.position, transform.rotation) as Transform;

		tmp.parent = this.transform;
		healthbar = tmp.GetComponent<HealthBar> ();
		healthbar.Reset ();
	}

	public void takeDamage (float amount) {
		health -= amount;
		healthbar.Set (health / 100f);

		if (health <= 0f) {
			ObjectPool.instance.PoolObject(this.gameObject);

			BlorbManager.Instance.Transaction(killValue, transform.position);
		}
	}
}
