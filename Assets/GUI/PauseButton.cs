using UnityEngine;
using System.Collections;

public class PauseButton : MonoBehaviour {
	void OnMouseDown () {
		WorldManager.instance.PauseButton ();
	}

	void OnMouseEnter () {
		GUIManager.Instance.MouseOverUI = true;
	}
	
	void OnMouseExit () {
		GUIManager.Instance.MouseOverUI = false;
	}
}
