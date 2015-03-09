using UnityEngine;
using System.Collections;

public class FastForwardButton : MonoBehaviour {
	void OnMouseDown () {
		WorldManager.instance.FastForwardButton ();
	}

	void OnMouseEnter () {
		GUIManager.Instance.MouseOverUI = true;
	}

	void OnMouseExit () {
		GUIManager.Instance.MouseOverUI = false;
	}
}

