using UnityEngine;

public class GUIManager : MonoBehaviour {
	private static GUIManager instance;
	public GUIText gameOverText, instructionsText, titleText;
	public Camera minimap, maincamera;
	public GameObject HUD;
	public Transform GUI;
	public SpriteRenderer HUDTutorial;

	private static float easing = 0.05f;
	private Transform towers, skipButton, moveIndicator;
	private Vector3 towersDay, moveIndicatorDay, skipButtonDay,
		towersNight, moveIndicatorNight, skipButtonNight;

	private int viewStage = 0; // 0- title screen, 1- tutorial screen, 2- main game, 3- game over screen
	private bool shownTutorial = false; // skip stage 1 after seeing it once

	void setHUD (bool enabled) {
		SpriteRenderer[] renderers = HUD.GetComponentsInChildren<SpriteRenderer>();
		TextMesh[] texts = HUD.GetComponentsInChildren<TextMesh> ();

		foreach (SpriteRenderer renderer in renderers) {
			if (renderer.sprite.name != "hud_4") {
				renderer.enabled = enabled;
			}
		}

		foreach (TextMesh text in texts) {
			text.renderer.enabled = enabled;
		}

		minimap.enabled = enabled;
	}

	public static void UpdateTowerGUI(float blorbAmount)
	{
		Placement[] placements = GameObject.FindGameObjectWithTag("Towers").GetComponentsInChildren<Placement>();
		foreach (Placement placement in placements)
		{
			if (placement.cost > blorbAmount)
			{
				placement.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0f, 0f);
				placement.transform.parent.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0f, 0f);
			}
			else
			{
				placement.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
				placement.transform.parent.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
			}
		}
	}

	void Start () {
		instance = this;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		gameOverText.enabled = false;
		instance.setHUD (false);

		towers = GameObject.FindGameObjectWithTag ("Towers").transform;
		skipButton = GameObject.FindGameObjectWithTag ("Skip").transform;
		moveIndicator = GameObject.FindGameObjectWithTag ("Movement Indicator").transform;

		towersDay = towersNight = towers.localPosition;
		skipButtonDay = skipButtonNight = skipButton.localPosition;
		moveIndicatorDay = moveIndicatorNight = moveIndicator.localPosition;

		towersNight.x += 3.2f;
		skipButtonNight.y -= 1.64f;
		moveIndicatorNight.y -= 1.64f;
	}

	private void GameOver () {
		gameOverText.enabled = true;
		instructionsText.enabled = true;
		instance.setHUD (false);
		viewStage = 3;
	}
	
	void Update () {
		if (Input.GetButtonDown("Jump")) {
			if (viewStage == 0 || viewStage == 3) {
				GameEventManager.TriggerGameStart();
			} else if (viewStage == 1) {
				HUDTutorial.enabled = false;
				Time.timeScale = 1;
				viewStage = 2;
			}
		}

		if (viewStage == 2 && Input.GetAxis ("Mouse ScrollWheel") != 0f) {
			maincamera.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * 5f;

			if (maincamera.orthographicSize < 4f) {
				maincamera.orthographicSize = 4f;
			} else if (maincamera.orthographicSize > 24f) {
				maincamera.orthographicSize = 24f;
			}

			float scale = maincamera.orthographicSize / 12f;

			GUI.localScale = new Vector2(scale, scale);
		}

		if (WorldManager.instance.isDay) {
			towers.localPosition += (towersDay - towers.localPosition) * easing;
			skipButton.localPosition += (skipButtonDay - skipButton.localPosition) * easing;
			moveIndicator.localPosition += (moveIndicatorDay - moveIndicator.localPosition) * easing;
		} else {
			towers.localPosition += (towersNight - towers.localPosition) * easing;
			moveIndicator.localPosition += (moveIndicatorNight - moveIndicator.localPosition) * easing;

			if (!WaveManager.instance.waveEnded) {
				skipButton.localPosition += (skipButtonNight - skipButton.localPosition) * easing;
			} else {
				skipButton.localPosition += (skipButtonDay - skipButton.localPosition) * easing;
			}
		}
	}
	
	private void GameStart () {
		gameOverText.enabled = false;
		instructionsText.enabled = false;
		titleText.enabled = false;
		instance.setHUD (true);
		viewStage = 2;

		if (!shownTutorial) {
			HUDTutorial.enabled = true;
			Time.timeScale = 0; // pause time
			shownTutorial = true;
			viewStage = 1;
		}
	}
}