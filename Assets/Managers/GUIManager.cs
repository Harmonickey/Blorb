using UnityEngine;

public class GUIManager : MonoBehaviour {
	private static GUIManager instance;
	public GUIText gameOverText, instructionsText, titleText;
	public Camera minimap;
	public GameObject HUD;
	public SpriteRenderer HUDTutorial;

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

	void Start () {
		instance = this;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		gameOverText.enabled = false;
		instance.setHUD (false);
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