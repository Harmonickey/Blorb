using UnityEngine;
using System.Collections;

public class AddHealthButton : MonoBehaviour {
	
    void OnMouseDown () 
    {
		Center.Instance.AddHealthButton ();
    }

    void OnMouseEnter()
    {
        GUIManager.Instance.MouseOverUI = true;
    }

    void OnMouseExit()
    {
        GUIManager.Instance.MouseOverUI = false;
    }
}
