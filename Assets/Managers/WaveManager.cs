using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour {
	public static WaveManager instance;
	public Transform enemy;
    public Transform player;
	private float enemySpawnDelay = 2.0f;
	private int enemiesPerWave = 10;
	
	private float spawnNextEnemy;
	private int enemiesSpawned;
	public bool waveEnded = true;

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
        player.transform.parent.GetComponent<Center>().IsActive = true;
	}

	void NightStart() {
		waveEnded = false;
		spawnNextEnemy = Time.time + enemySpawnDelay;
		Debug.Log ("Night");
        player.transform.parent.GetComponent<Center>().IsActive = false;
        Pathfinder2D.Instance.Create2DMap();
        GameObject.FindObjectOfType<Placement>().StopPlacement();
	}


	void GameStart() {
		enabled = true;

		waveEnded = true;
		enemiesSpawned = 0;
		spawnNextEnemy = 0f;
	}

	void GameOver() {
		GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag("Enemy"); 

		foreach (GameObject obj in taggedGameObjects) {
			Destroy (obj);
		}

		enabled = false;
	}

	void Update () {
		if (spawnNextEnemy != 0f && spawnNextEnemy < Time.time && enemiesSpawned < enemiesPerWave) {
			Transform newEnemy = Instantiate (enemy) as Transform;
			newEnemy.transform.position = 10 * Random.insideUnitCircle;
            newEnemy.GetComponent<SimpleAI2D>().Player = player; //set the target as the player

            enemiesSpawned++;
            spawnNextEnemy = Time.time + enemySpawnDelay;
        }

		if (enemiesSpawned == enemiesPerWave) {
			waveEnded = true;
		}
	}
}
