using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour {
	public static WaveManager instance;
	public Transform enemy;
    public Transform player;
	private float enemySpawnDelay = 2.0f;
	private int enemiesPerWave = 10;

	private int waveCount = 0;
	
	private float spawnNextEnemy;
	private int enemiesSpawned;
	public bool waveEnded = true;
    private bool waveStart = false;

    public bool enemyAgro = false;

	void Start() {
		instance = this;

		enabled = false;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		GameEventManager.DayStart += DayStart;
		GameEventManager.NightStart += NightStart;
	}

	void DayStart() {
        player.GetComponent<Center>().IsActive = true;
	}

	void NightStart() {
		waveEnded = false;
		enemiesSpawned = 0;
        spawnNextEnemy = enemySpawnDelay;
        player.GetComponent<Center>().IsActive = false;

        Pathfinder2D.Instance.MapStartPosition = new Vector2(-25 + player.position.x, -25 + player.position.y);
        Pathfinder2D.Instance.MapEndPosition = new Vector2(25 + player.position.x, 25 + player.position.y);
        Pathfinder2D.Instance.DisallowedTags.Clear();
        Pathfinder2D.Instance.DisallowedTags.AddRange(new string[5] { "Turret", "Collector", "Wall", "Mountain", "Resource" });
        Pathfinder2D.Instance.Create2DMap();
        
        //should check here if there is a path to the player
        //if not, then change the disallowed tags

        player.GetComponent<Center>().HasPathToCenter();

        Placement.StopPlacement();

	}

    public void PreWaveInit(bool hasPath)
    {
        if (!hasPath)
        {
            enemyAgro = true;
            Pathfinder2D.Instance.DisallowedTags.Clear();
            Pathfinder2D.Instance.DisallowedTags.AddRange(new string[2] { "Mountain", "Resource" });
            Pathfinder2D.Instance.Create2DMap();
        }

        waveCount++;
        waveStart = true;
    }


	void GameStart() {
		enabled = true;

		waveCount = 0;
		waveEnded = true;
		enemiesSpawned = 0;
		spawnNextEnemy = 0f;
	}

	void GameOver() {
		GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag("Enemy"); 

		foreach (GameObject obj in taggedGameObjects) {
			ObjectPool.instance.PoolObject (obj);
		}

		enabled = false;
	}

	void Update () {
		spawnNextEnemy -= Time.deltaTime;

		if (!WorldManager.instance.isDay && spawnNextEnemy < 0f && enemiesSpawned < enemiesPerWave && waveStart) {
			GameObject newEnemy = ObjectPool.instance.GetObjectForType("Enemy", true);
			float randAngle = Random.Range(0f, 2 * Mathf.PI);

			newEnemy.transform.position = player.position + 10f * new Vector3(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
            newEnemy.GetComponent<SimpleAI2D>().Player = player; //set the target as the player

			if (enemyAgro) {
				newEnemy.GetComponent<SpriteRenderer>().color = new Color(0.874f, 0.56f, 0.525f);
			}

            enemiesSpawned++;
            spawnNextEnemy = enemySpawnDelay;
        }

		if (enemiesSpawned == enemiesPerWave && GameObject.FindGameObjectWithTag("Enemy") == null) {
			waveEnded = true;
            waveStart = false;
            enemyAgro = false;
		}
	}
}
