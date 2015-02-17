using UnityEngine;

public class GUIManager : MonoBehaviour {
	private static GUIManager instance;
	public GUIText gameOverText, instructionsText, titleText;
	public Camera minimap;
	public GameObject HUD;

	void setHUD (bool enabled) {
		SpriteRenderer[] renderers = HUD.GetComponentsInChildren<SpriteRenderer>();
		TextMesh[] texts = HUD.GetComponentsInChildren<TextMesh> ();

		foreach (SpriteRenderer renderer in renderers) {
			renderer.enabled = enabled;
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
		enabled = true;
	}
	
	void Update () {
		if(Input.GetButtonDown("Jump")){
			GameEventManager.TriggerGameStart();
		}
	}
	
	private void GameStart () {
		gameOverText.enabled = false;
		instructionsText.enabled = false;
		titleText.enabled = false;
		instance.setHUD (true);
		enabled = false;
	}

//	void OnGUI () {
//		GUI.Label (new Rect (0, 0, Screen.width, Screen.height), "", bgStyle);
//	}
}