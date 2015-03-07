using UnityEngine;

public class GUIManager : MonoBehaviour {
	
	public GUIText gameOverText, instructionsText, titleText;
	public Camera maincamera;
	public GameObject HUD;
	public Transform GUI;
	public SpriteRenderer HUDTutorial;
    public Transform cancelButton;

	private static float easing = 0.05f;
	public Transform towers, skipButton, moveIndicator;
    public Transform addHealth;
	private Vector3 towersDay, moveIndicatorDay, skipButtonDay, cancelButtonDay,
		towersNight, moveIndicatorNight, skipButtonNight, cancelButtonNight;

    public static GUIManager Instance
    {
        get { return instance; }
    }

    private static GUIManager instance;

    public bool OnTutorialScreen
    {
        get { return (viewStage == 1); }
    }

	private int viewStage = 0; // 0- title screen, 1- tutorial screen, 2- main game, 3- game over screen
	private bool shownTutorial = false; // skip stage 1 after seeing it once

	public bool MouseOverUI = false;

	void setHUD (bool enabled) {
		SpriteRenderer[] renderers = HUD.GetComponentsInChildren<SpriteRenderer>();
		TextMesh[] texts = HUD.GetComponentsInChildren<TextMesh> ();

		foreach (SpriteRenderer renderer in renderers) {
			if (renderer.sprite.name != "hud_4" && renderer.gameObject.tag != "Tower Detail") {
				renderer.enabled = enabled;
			}
		}

		foreach (TextMesh text in texts) {
			if (text.gameObject.tag != "Tower Detail") {
				text.renderer.enabled = enabled;
			}
		}
	}

	public static void UpdateTowerGUI(float blorbAmount)
	{
		Placement[] placements = GUIManager.instance.towers.gameObject.GetComponentsInChildren<Placement>();
		foreach (Placement placement in placements)
		{
			if (placement.cost > blorbAmount)
			{
				placement.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0f, 0f);
				placement.transform.parent.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0f, 0f);
			}
		}

		// add health button
		if (blorbAmount < Center.addHealthCost) {
            GUIManager.instance.addHealth.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0f, 0f);
		}
	}

    public static void RefreshTowerGUIColors()
    {
		SpriteRenderer[] srs = GUIManager.instance.towers.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in srs)
        {
            if (sr.GetComponent<Placement>() != null)
            {
                if (!Center.Instance.HasEnoughResources(sr.GetComponent<Placement>().cost))
                    continue;
            }
            if (sr.GetComponentInChildren<Placement>() != null)
            {
                if (!Center.Instance.HasEnoughResources(sr.GetComponentInChildren<Placement>().cost))
                    continue;
            }

            sr.color = new Color(1f, 1f, 1f);
        }
    }

	void Start () {
		instance = this;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		gameOverText.enabled = false;
		instance.setHUD (false);

		towersDay = towersNight = towers.localPosition;
		skipButtonDay = skipButtonNight = skipButton.localPosition;
		moveIndicatorDay = moveIndicatorNight = moveIndicator.localPosition;
        cancelButtonDay = cancelButtonNight = cancelButton.localPosition;

		// magic numbers for how much to move each of the elements to be out of frame
		towersNight.x += 5f;
		skipButtonNight.y -= 1.64f;
		moveIndicatorNight.y -= 1.64f;
        cancelButtonNight.x += 5f;
	}

	private void GameOver () {
		gameOverText.enabled = true;
        instructionsText.text = "Try Again! \n\n Press space to start";
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

        if (WorldManager.instance != null) // on GameOver WorldManager is unenabled, so need to check null here...
        {
            if (WorldManager.instance.isDay)
            {
                towers.localPosition += (towersDay - towers.localPosition) * easing;
                skipButton.localPosition += (skipButtonDay - skipButton.localPosition) * easing;
                moveIndicator.localPosition += (moveIndicatorDay - moveIndicator.localPosition) * easing;
                cancelButton.localPosition += (cancelButtonDay - cancelButton.localPosition) * easing;
            }
            else
            {
                towers.localPosition += (towersNight - towers.localPosition) * easing;
                moveIndicator.localPosition += (moveIndicatorNight - moveIndicator.localPosition) * easing;
                cancelButton.localPosition += (cancelButtonNight - cancelButton.localPosition) * easing;

                if (!WaveManager.instance.waveEnded)
                {
                    skipButton.localPosition += (skipButtonNight - skipButton.localPosition) * easing;
                }
                else
                {
                    skipButton.localPosition += (skipButtonDay - skipButton.localPosition) * easing;
                }
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