using UnityEngine;
using System.Collections;

public class PauseButton : MonoBehaviour {
	void OnMouseDown () {
		WorldManager.instance.PauseButton ();
	}
}
