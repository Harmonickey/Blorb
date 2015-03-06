using UnityEngine;
using System.Collections;

public class SellButton : MonoBehaviour {

    void OnMouseDown()
    {
        WorldManager.instance.SellButton();

        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
    }

    void OnMouseUp()
    {
        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    void OnMouseEnter()
    {
        GUIManager.Instance.MouseOverUI = true;
    }

    void OnMouseExit()
    {
        GUIManager.Instance.MouseOverUI = false;

        //just in case the mouse up event doesn't happen over the object...
        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }
}
