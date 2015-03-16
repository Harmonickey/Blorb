using UnityEngine;
using System.Collections;

public class PauseButton : MonoBehaviour {
	void OnMouseDown () {
		WorldManager.instance.PauseButton ();

        if (!GUIManager.Instance.OnTutorialScreen)
		{
			Placement.StopPlacement();
			GUIManager.Instance.MouseOverUI = false;
			
			SellingManager.Instance.SetVisibility(false);
		}
	}

	void OnMouseEnter () {
		GUIManager.Instance.MouseOverUI = true;
	}
	
	void OnMouseExit () {
		GUIManager.Instance.MouseOverUI = false;
	}
}
