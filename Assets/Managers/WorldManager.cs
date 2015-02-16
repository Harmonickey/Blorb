﻿using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour {
	public Transform dayNightDial;

	private float phaseDuration = 45f;
	private float startNextPhase;
	private bool isDay = true;
    private static bool towersLowering, towersLifting;
    private static float target;

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

        if (towersLowering)
        {
            GameObject towers = GameObject.FindGameObjectWithTag("Towers");
            towers.transform.position -= new Vector3(0.0f, 5.5f * Time.deltaTime, 0.0f);
            if (towers.transform.position.y <= target)
            {
                towers.transform.position = new Vector3(towers.transform.position.x, target, 0.0f);
                towersLowering = false;
            }
        }

        if (towersLifting)
        {
            GameObject towers = GameObject.FindGameObjectWithTag("Towers");
            towers.transform.position += new Vector3(0.0f, 5.5f * Time.deltaTime, 0.0f);
            if (towers.transform.position.y >= target)
            {
                towers.transform.position = new Vector3(towers.transform.position.x, target, 0.0f);
                towersLifting = false;
            }
        }
	}

    public static void LowerTowers()
    {
        target = GameObject.FindGameObjectWithTag("Towers").transform.position.y - 11.9f;
        towersLowering = true;
    }

    public static void LiftTowers()
    {
        target = GameObject.FindGameObjectWithTag("Towers").transform.position.y + 11.9f;
        towersLifting = true;
    }
}
