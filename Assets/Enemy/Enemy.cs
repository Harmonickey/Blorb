using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public Transform healthbar;

    public float hitDamage = 10.0f;
	private const float killValue = 10f;
	
    private float health = 100.0f;

	public void takeDamage (float amount) {
		health -= amount;
		healthbar.localScale = new Vector2 (health * 0.0015f, 0.15f);

		if (health <= 0f) {
			Destroy(this.gameObject);

			GameObject.FindGameObjectWithTag("Center").SendMessage("receiveBlorb", killValue);
		}
	}
}
