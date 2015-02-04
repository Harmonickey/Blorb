using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour {

	private static float health;

	void GameStart () {
		health = 100f;
	}

	// Use this for initialization
	void Start () {
		GameEventManager.GameStart += GameStart;
	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0f) {
			GameEventManager.TriggerGameOver();
		}
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        Damage(Enemy.hitDamage);
    }

    public static void Damage(float damage)
    {
        health -= damage;

        Debug.Log("HEALTH: " + health);
    }

}
