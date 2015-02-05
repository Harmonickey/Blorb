using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour {
	private float health;

	void GameStart () {
		health = 100f;
		enabled = true;
	}

	void OnTriggerEnter (Collision2D collision) {
		gameObject.SetActive (false);
	}

	// Use this for initialization
	void Start () {
		GameEventManager.GameStart += GameStart;
		enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0f) {
			GameEventManager.TriggerGameOver();
		}
	}
}
