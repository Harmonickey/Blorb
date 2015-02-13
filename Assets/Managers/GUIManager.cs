using UnityEngine;

public class GUIManager : MonoBehaviour {
	private static GUIManager instance;
	public GUIText gameOverText, instructionsText, titleText;
	public SpriteRenderer background;
	public TextMesh health;
	public Camera minimap;
    public Transform turretPlacement, wallPlacement, collectorPlacement;

	void enableHUD () {
		background.enabled = true;
		minimap.enabled = true;
		health.renderer.enabled = true;
        turretPlacement.renderer.enabled = true;
        turretPlacement.FindChild("Turret").renderer.enabled = true;
        wallPlacement.renderer.enabled = true;
        wallPlacement.FindChild("Wall").renderer.enabled = true;
        collectorPlacement.renderer.enabled = true;
        collectorPlacement.FindChild("Collector").renderer.enabled = true;
	}

	void disableHUD () {
		background.enabled = false;
		minimap.enabled = false;
		health.renderer.enabled = false;
        turretPlacement.renderer.enabled = false;
        turretPlacement.FindChild("Turret").renderer.enabled = false;
        wallPlacement.renderer.enabled = false;
        wallPlacement.FindChild("Wall").renderer.enabled = false;
        collectorPlacement.renderer.enabled = false;
        collectorPlacement.FindChild("Collector").renderer.enabled = false;

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