using UnityEngine;
using System.Collections;

public class FastForwardButton : MonoBehaviour {
	void OnMouseDown () {
		WorldManager.instance.FastForwardButton ();
	}
}

