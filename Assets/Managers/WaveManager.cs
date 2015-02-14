using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour {
	private static WaveManager instance;
	public Transform enemy;
    public Transform player;
	private float enemySpawnDelay = 2.0f;
	private int enemiesPerWave = 10;
	
	private float spawnNextEnemy;
	private int enemiesSpawned;

	void Start() {
		instance = this;

		enabled = false;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		GameEventManager.DayStart += DayStart;
		GameEventManager.NightStart += NightStart;
	}

	void DayStart() {
		Debug.Log ("Day");
	}
	void NightStart() {
		spawnNextEnemy = Time.time + enemySpawnDelay;
		Debug.Log ("Night");
	}


	void GameStart() {
		enabled = true;

		enemiesSpawned = 0;
	}

	void GameOver() {
		GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag("Enemy"); 

		foreach (GameObject obj in taggedGameObjects) {
			Destroy (obj);
		}

		enabled = false;
	}

	void Update () {
		if (spawnNextEnemy < Time.time && enemiesSpawned < enemiesPerWave) {
			Transform newEnemy = Instantiate (enemy) as Transform;
			newEnemy.transform.position = 3 * Random.insideUnitCircle;
            newEnemy.GetComponent<SimpleAI2D>().Player = player; //set the target as the player

			enemiesSpawned++;
			spawnNextEnemy = Time.time + enemySpawnDelay;
		}
	}
}
