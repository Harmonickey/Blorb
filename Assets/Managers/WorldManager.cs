using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour {
	public static WorldManager instance;
	public Light worldLight;
	public Transform dayNightDial;
	public SpriteRenderer pauseButton, fastForwardButton;
    public static float PixelOffset = 750.0f;
	public bool isDay = true;

	private float phaseDuration = 45f;
	private float startNextPhase, savedTimeScale;

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
				Time.timeScale = savedTimeScale;
				pauseButton.color = new Color(1f, 1f, 1f, 1f);
			} else {
				savedTimeScale = Time.timeScale;
				Time.timeScale = 0f;
				pauseButton.color = new Color(1f, 1f, 1f, 0.5f);
			}
		}
	}

	public void FastForwardButton () {
		if (!GUIManager.Instance.OnTutorialScreen && Time.timeScale != 0f) {
			if (Time.timeScale == 1f) {
				Time.timeScale = 2f;
				fastForwardButton.color = new Color(1f, 1f, 1f, 0.5f);
			} else {
				Time.timeScale = 1f;
				fastForwardButton.color = new Color(1f, 1f, 1f, 1f);
			}
		}
	}

	public void SkipButton () {
		if (Time.timeScale != 0f) {
			if (isDay) {
				startNextPhase = 0f;
				dayNightDial.localEulerAngles = new Vector3(0, 0, -90);
				worldLight.intensity = 0.4f;
			} else if (WaveManager.instance.waveEnded) {
				startNextPhase = 0f;
				dayNightDial.localEulerAngles = new Vector3(0, 0, 90);
				worldLight.intensity = 1f;
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
				worldLight.intensity = 0.4f;
			} else {
				GameEventManager.TriggerDayStart();
				worldLight.intensity = 1f;
			}
			
			isDay = !isDay;
			startNextPhase = phaseDuration;
		}

		if (startNextPhase < 10f) {
			if (isDay) {
				worldLight.intensity = 0.4f + 0.06f * startNextPhase;
			} else {
				worldLight.intensity = 0.4f + 0.06f * (10f - startNextPhase);
			}
		}
	}
}
