using UnityEngine;
using System.Collections;

public class SkipButton : MonoBehaviour {
	void OnMouseDown () {
		WorldManager.instance.SkipButton ();
	}

	void OnMouseEnter () {
		GUIManager.Instance.MouseOverUI = true;
	}
	
	void OnMouseExit () {
		GUIManager.Instance.MouseOverUI = false;
	}
}
