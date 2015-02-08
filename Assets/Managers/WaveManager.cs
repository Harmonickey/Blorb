using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour {
	public Transform enemy;
	public static float waveDelay = 3.0f;
	public static float enemySpawnDelay = 2.0f;
	public int enemiesPerWave = 15;

	private float startNextWave;
	private float spawnNextEnemy;
	private int enemiesSpawned = 0;
	private bool waveStarted = false;

	void GameStart() {
		enabled = true;
		startNextWave = Time.time + waveDelay;
	}

	void GameOver() {
		GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag("Enemy"); 

		foreach (GameObject obj in taggedGameObjects) {
			Destroy (obj);
		}

		enabled = false;
	}

	// Use this for initialization
	void Start () {
		enabled = false;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
	}
	
	// Update is called once per frame
	void Update () {
		if (!waveStarted && startNextWave < Time.time) {
			waveStarted = true;
		}

		if (waveStarted && spawnNextEnemy < Time.time && enemiesSpawned < enemiesPerWave) {
			Transform newEnemy = Instantiate (enemy) as Transform;
			newEnemy.transform.position = new Vector2 (-10.0f, 10.0f);

			enemiesSpawned++;
			spawnNextEnemy = Time.time + enemySpawnDelay;
		} else if (waveStarted && enemiesSpawned >= enemiesPerWave) {
			waveStarted = false;

			//TODO multiple waves
		}
	}
}
