using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour {
	public Transform dayNightDial;

	private float phaseDuration = 45f;
	private float startNextPhase;
	private bool isDay = true;

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

	void Start () {
		enabled = false;

		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
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
	}
}
