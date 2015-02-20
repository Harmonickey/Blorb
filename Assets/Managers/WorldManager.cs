using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour {
	public static WorldManager instance;
	public Transform dayNightDial;
    public static float PixelOffset = 750.0f;
	public bool isDay = true;

	private float phaseDuration = 45f;
	private float startNextPhase;
	private bool paused = false;
	
	void GameStart () {
		enabled = true;

		startNextPhase = Time.time + phaseDuration;
		isDay = true;
		dayNightDial.rotation = Quaternion.Euler (0f, 0f, 90f);
	}

	void GameOver () {
		enabled = false;
	}

	void FixedUpdate () {
		dayNightDial.Rotate (0, 0, -180f * Time.deltaTime / phaseDuration);
	}

	public void PauseGame () {
		paused = true;
		Time.timeScale = 0f;
	}

	public void UnpauseGame () {
		paused = false;
		Time.timeScale = 1f;
	}

	public void SkipButton () {
		if (isDay) {
			startNextPhase = Time.time;
			dayNightDial.localEulerAngles = new Vector3(0, 0, -90);
		} else if (WaveManager.instance.waveEnded) {
			startNextPhase = Time.time;
			dayNightDial.localEulerAngles = new Vector3(0, 0, 90);
		}
	}

	void Start () {
		instance = this;
		enabled = false;

		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
	}
	
	// Update is called once per frame
	void Update () {
		if (!paused && startNextPhase < Time.time) {
			if (isDay) {
				GameEventManager.TriggerNightStart();
			} else {
				GameEventManager.TriggerDayStart();
			}
			
			isDay = !isDay;
			startNextPhase = Time.time + phaseDuration;
		}
	}
}
