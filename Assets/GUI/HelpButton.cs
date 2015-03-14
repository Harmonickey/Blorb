using UnityEngine;
using System.Collections;

public class HelpButton : MonoBehaviour {
	void OnMouseDown () {
		GUIManager.Instance.HelpButton ();
	}
}
