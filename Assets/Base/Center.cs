using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour {
	private float health;

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
}
