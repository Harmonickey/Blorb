using UnityEngine;
using System.Collections;

public class CancelButton : MonoBehaviour {

    void OnMouseDown()
    {
        //may need to pass the sprite renderer through so that I can make the color change back opaque
        WorldManager.instance.CancelButton(this.GetComponent<SpriteRenderer>());

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

        // just in case the mouse up event is not over the button
        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }
}
