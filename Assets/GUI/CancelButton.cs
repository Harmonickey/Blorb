using UnityEngine;
using System.Collections;

public class CancelButton : MonoBehaviour {
	public Transform sellWindow;
	private SpriteRenderer sr;

	void Start ()
	{
		sr = this.GetComponent<SpriteRenderer> ();
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Press ();
		}
	}

	void Press()
	{
		if (!GUIManager.Instance.OnTutorialScreen)
		{
			Placement.StopPlacement();
			GUIManager.Instance.MouseOverUI = false;
			
			SellingManager.Instance.SetVisibility(false);
		}
		
		sr.color = new Color(1f, 1f, 1f, 0.5f);
	}

    void OnMouseDown()
    {
		Press ();
    }

    void OnMouseUp()
    {
        sr.color = new Color(1f, 1f, 1f, 1f);
    }

    void OnMouseEnter()
    {
        GUIManager.Instance.MouseOverUI = true;
    }

    void OnMouseExit()
    {
        GUIManager.Instance.MouseOverUI = false;

        // just in case the mouse up event is not over the button
        sr.color = new Color(1f, 1f, 1f, 1f);
    }
}
