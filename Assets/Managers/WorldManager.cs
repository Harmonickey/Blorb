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
	}

	void GameOver () {
		dayNightDial.rotation.Set (0, 0, 90, 0);
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
