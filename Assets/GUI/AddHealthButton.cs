using UnityEngine;
using System.Collections;

public class AddHealthButton : MonoBehaviour {
	void OnMouseDown () {
		Center.Instance.AddHealthButton ();
	}
}
