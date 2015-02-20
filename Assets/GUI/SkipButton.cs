using UnityEngine;
using System.Collections;

public class SkipButton : MonoBehaviour {
	void OnMouseDown () {
		WorldManager.instance.SkipButton ();
	}
}
