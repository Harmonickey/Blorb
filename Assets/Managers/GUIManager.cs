using UnityEngine;

public class GUIManager : MonoBehaviour {
	
	public GUIText gameOverText, instructionsText, titleText;
	public Camera maincamera;
	public GameObject HUD;
	public Transform GUI;
	public SpriteRenderer IntroTutorial, HUDTutorial, NightTutorial;

	private static float easing = 2f;
	public Transform towers, cancelButton, skipButton, moveIndicator, addHealthButton;
	private Vector3 towersDay, moveIndicatorDay, skipButtonDay, cancelButtonActive, cancelButtonInactive,
		towersNight, moveIndicatorNight, skipButtonNight;

    public static GUIManager Instance
    {
        get { return instance; }
    }

    private static GUIManager instance;

    public bool OnTutorialScreen
    {
        get { return (viewStage == 1); }
    }

	public int ViewStage
	{
		get { return viewStage; }
	}

	private int viewStage = 0; // 0- title screen, 1- tutorial screens, 2- main game, 3- game over screen
	private bool shownIntroTutorial = false;
	private bool shownHUDTutorial = false; // skip stage 1 after seeing it once
	private bool firstNight = true;
	private float originalTimeScale = -1;

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

	public void UpdateTowerGUI(Placement selectedTower = null)
	{

		Color col;
		float blorbAmount = BlorbManager.Instance.BlorbAmount;
		Placement[] placements = GUIManager.instance.towers.gameObject.GetComponentsInChildren<Placement>();
		foreach (Placement placement in placements)
		{
            if (placement.cost > blorbAmount) {
                col = new Color(0.3f, 0f, 0f);
            }
            else if (placement == selectedTower) {
                continue;
            }
            else  {
                col = new Color(1f, 1f, 1f);
            }

			placement.GetComponent<SpriteRenderer>().color = col;
			placement.transform.parent.GetComponent<SpriteRenderer>().color = col;
		}

		// add health button
		if (blorbAmount < AddHealthButton.cost || Center.Instance.health > 90f) {
            addHealthButton.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0f, 0f);
		} else {
			addHealthButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
		}
	}

	public void HelpButton() {
		IntroTutorial.enabled = true;
		originalTimeScale = Time.timeScale;
		Time.timeScale = 0;
		viewStage = 1;
	}

	void Start () {
		instance = this;
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;
		GameEventManager.NightStart += OnNightStart;
		gameOverText.enabled = false;
		instance.setHUD (false);

		towersDay = towersNight = towers.localPosition;
		skipButtonDay = skipButtonNight = skipButton.localPosition;
		moveIndicatorDay = moveIndicatorNight = moveIndicator.localPosition;
        cancelButtonActive = cancelButtonInactive = cancelButton.localPosition;

		// magic numbers for how much to move each of the elements to be out of frame
		cancelButtonInactive.y -= 1.64f;
		towersNight.x += 5f;
		skipButtonNight.y -= 1.64f;
		moveIndicatorNight.y -= 1.64f;
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
				IntroTutorial.enabled = false;
				HUDTutorial.enabled = false;
				NightTutorial.enabled = false;

				if (!shownHUDTutorial) {
					HUDTutorial.enabled = true;
					shownHUDTutorial = true;
				} else {
					viewStage = 2;

					if (originalTimeScale != -1) {
						Time.timeScale = originalTimeScale;
						originalTimeScale = -1;
					} else {
						Time.timeScale = 1;
					}
				}
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
                towers.localPosition += (towersDay - towers.localPosition) * easing * Time.deltaTime;
				skipButton.localPosition += (skipButtonDay - skipButton.localPosition) * easing * Time.deltaTime;
				moveIndicator.localPosition += (moveIndicatorDay - moveIndicator.localPosition) * easing * Time.deltaTime;

                cancelButton.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
				if (Placement.isPlacingTowers) {
					cancelButton.localPosition += (cancelButtonActive - cancelButton.localPosition) * easing * Time.deltaTime;
				} else {
					cancelButton.localPosition += (cancelButtonInactive - cancelButton.localPosition) * easing * Time.deltaTime;
				}
            }
            else
            {
				towers.localPosition += (towersNight - towers.localPosition) * easing * Time.deltaTime;
				moveIndicator.localPosition += (moveIndicatorNight - moveIndicator.localPosition) * easing * Time.deltaTime;
				cancelButton.localPosition += (cancelButtonInactive - cancelButton.localPosition) * easing * Time.deltaTime;

                if (!WaveManager.instance.waveEnded)
                {
					skipButton.localPosition += (skipButtonNight - skipButton.localPosition) * easing * Time.deltaTime;
                }
                else
                {
					skipButton.localPosition += (skipButtonDay - skipButton.localPosition) * easing * Time.deltaTime;
                }
            }
        }
	}

    void OnGUI()
    {
        //catch trackpad scrolling
        if (viewStage == 2 && Event.current.type == EventType.ScrollWheel)
        {
            maincamera.orthographicSize += (Event.current.delta.y / 100) * 5f;

            if (maincamera.orthographicSize < 4f)
            {
                maincamera.orthographicSize = 4f;
            }
            else if (maincamera.orthographicSize > 24f)
            {
                maincamera.orthographicSize = 24f;
            }

            float scale = maincamera.orthographicSize / 12f;

            GUI.localScale = new Vector2(scale, scale);
        }
    }

	void OnNightStart () {
		if (firstNight) {
			NightTutorial.enabled = true;
			originalTimeScale = Time.timeScale;
			Time.timeScale = 0;
			viewStage = 1;
			firstNight = false;
		}
	}
	
	private void GameStart () {
		gameOverText.enabled = false;
		instructionsText.enabled = false;
		titleText.enabled = false;
		instance.setHUD (true);
		viewStage = 2;

		if (!shownIntroTutorial) {
			IntroTutorial.enabled = true;
			Time.timeScale = 0; // pause time
			shownIntroTutorial = true;
			viewStage = 1;
		}
	}
}