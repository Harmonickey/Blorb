using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour {
	private static WaveManager instance;
	public Transform enemy;
    public Transform player;
	public Transform dayNightDial;
	public float phaseDuration = 45f;
	public static float enemySpawnDelay = 2.0f;
	public int enemiesPerWave = 10;

	private float startNextPhase;
	private bool isDay = true;
	private float spawnNextEnemy;
	private int enemiesSpawned;
	private bool waveStarted = false;
    private bool mapCreated = false;

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
		Debug.Log ("Night");
	}


	void GameStart() {
		enabled = true;

		startNextPhase = Time.time + phaseDuration;
		isDay = true;

		enemiesSpawned = 0;
	}

	void GameOver() {
		GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag("Enemy"); 

		foreach (GameObject obj in taggedGameObjects) {
			Destroy (obj);
		}

		enabled = false;
		waveStarted = false;
	}

	void FixedUpdate () {
		dayNightDial.Rotate (0, 0, -180f * Time.deltaTime / phaseDuration);
	}

	// Update is called once per frame
	void Update () {
		if (startNextPhase < Time.time) {
			if (isDay) {
				GameEventManager.TriggerNightStart();
			} else {
				GameEventManager.TriggerDayStart();
			}

			isDay = !isDay;
			startNextPhase = Time.time + phaseDuration;
		}
		if (!waveStarted && startNextPhase < Time.time) {
			waveStarted = true;

            if (!mapCreated)
            {
                //Pathfinder2D.Instance.Create2DMap(); //create the map for the enemies to run on
                mapCreated = true;
            }
		}

		if (waveStarted && spawnNextEnemy < Time.time && enemiesSpawned < enemiesPerWave) {
			Transform newEnemy = Instantiate (enemy) as Transform;
			newEnemy.transform.position = new Vector2 (-10.0f, 10.0f);
            newEnemy.GetComponent<SimpleAI2D>().Player = player; //set the target as the player

			enemiesSpawned++;
			spawnNextEnemy = Time.time + enemySpawnDelay;
		} else if (waveStarted && enemiesSpawned >= enemiesPerWave) {
			waveStarted = false;

			//TODO multiple waves
		}
	}
}
