using UnityEngine;

public class GUIManager : MonoBehaviour {
	private static GUIManager instance;
	public GUIText gameOverText, instructionsText, titleText;
	public GUITexture HUD;
	
	void Start () {
		instance = this;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		gameOverText.enabled = false;
		HUD.enabled = false;
	}

	private void GameOver () {
		gameOverText.enabled = true;
		instructionsText.enabled = true;
		HUD.enabled = false;
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
		HUD.enabled = true;
		enabled = false;
	}
}