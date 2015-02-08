using UnityEngine;

public class GUIManager : MonoBehaviour {
	private static GUIManager instance;
	public GUIText gameOverText, instructionsText, titleText;
	public SpriteRenderer background, health;
	public Camera minimap;

	void enableHUD () {
		background.enabled = true;
		minimap.enabled = true;
	}

	void disableHUD () {
		background.enabled = false;
		minimap.enabled = false;
	}

	void Start () {
		instance = this;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		gameOverText.enabled = false;
		instance.disableHUD ();
	}

	private void GameOver () {
		gameOverText.enabled = true;
		instructionsText.enabled = true;
		instance.disableHUD ();
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
		enableHUD ();
		enabled = false;
	}

//	void OnGUI () {
//		GUI.Label (new Rect (0, 0, Screen.width, Screen.height), "", bgStyle);
//	}
}