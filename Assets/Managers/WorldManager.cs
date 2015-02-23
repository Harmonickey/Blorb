using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour {
	public static WorldManager instance;
	public Light worldLight;
	public Transform dayNightDial;
	public SpriteRenderer pauseButton;
    public static float PixelOffset = 750.0f;
	public bool isDay = true;

	private float phaseDuration = 45f;
	private float startNextPhase;

	void GameStart () {
		enabled = true;

		startNextPhase = phaseDuration;
		isDay = true;
		dayNightDial.rotation = Quaternion.Euler (0f, 0f, 90f);
	}

	void GameOver () {
		enabled = false;
	}

	void FixedUpdate () {
		dayNightDial.Rotate (0, 0, -180f * Time.deltaTime / phaseDuration);
	}

	public void PauseButton () {
		if (!GUIManager.Instance.OnTutorialScreen) {
			if (Time.timeScale == 0f) {
				Time.timeScale = 1f;
				pauseButton.color = new Color(1f, 1f, 1f, 1f);
			} else {
				Time.timeScale = 0f;
				pauseButton.color = new Color(1f, 1f, 1f, 0.5f);
			}
		}
	}

	public void SkipButton () {
		if (Time.timeScale != 0f) {
			if (isDay) {
				startNextPhase = 0f;
				dayNightDial.localEulerAngles = new Vector3(0, 0, -90);
			} else if (WaveManager.instance.waveEnded) {
				startNextPhase = 0f;
				dayNightDial.localEulerAngles = new Vector3(0, 0, 90);
			}
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
		startNextPhase -= Time.deltaTime;

		if (startNextPhase < 0f) {
			if (isDay) {
				GameEventManager.TriggerNightStart();
			} else {
				GameEventManager.TriggerDayStart();
			}
			
			isDay = !isDay;
			startNextPhase = phaseDuration;
		}

		if (isDay) {
			worldLight.intensity += (1f - worldLight.intensity) * 0.01f;
		} else {
			worldLight.intensity += (0.4f - worldLight.intensity) * 0.01f;
		}
	}
}
