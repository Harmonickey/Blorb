using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public Transform HealthBarPrefab;
	private HealthBar healthbar;
	private float currentHealth, maxHealth;

    public float HitDamage {
		get {
			return hitDamage;
		}
	}

	private float hitDamage = 10f;
	private const int killValue = 5;
	private const float daylightDamageDelay = 1f;
	private float daylightDamage;

	private void DayStart() {
		daylightDamage = daylightDamageDelay;
	}

	void Start () {
		currentHealth = 100f;
		maxHealth = 100f;
		Transform tmp = Instantiate (HealthBarPrefab, transform.position, transform.rotation) as Transform;

		tmp.parent = this.transform;
		healthbar = tmp.GetComponent<HealthBar> ();
		healthbar.Reset ();
	}

	void Update () {
		if (WorldManager.instance.isDay) {
			daylightDamage -= Time.deltaTime;
		}

		if (daylightDamage < 0f) {
			takeDamage(10f);
			daylightDamage = daylightDamageDelay;
		}
	}

	// called before the object is returned to the ObjectPool
	public void Reset (float newMaxHealth) {
		currentHealth = newMaxHealth;
		maxHealth = newMaxHealth;

		// Reset gets executed before Start on initialization...
		if (healthbar != null) {
			healthbar.Reset ();
		}

		gameObject.GetComponent<SimpleAI2D>().Speed = 1f;
		gameObject.GetComponent<SpriteRenderer>().color = new Color(0.874f, 0.914f, 0.525f);
	}

	public void takeDamage (float amount) {
		currentHealth -= amount;
		healthbar.Set (currentHealth / maxHealth);

		if (currentHealth <= 0f) {
			ObjectPool.instance.PoolObject(this.gameObject);

			BlorbManager.Instance.Transaction(killValue, transform.position);
		}
	}
}
