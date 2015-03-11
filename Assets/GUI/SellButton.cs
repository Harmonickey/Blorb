using UnityEngine;
using System.Collections;

public class SellButton : MonoBehaviour {
	public Transform sellWindow;

    void Start()
    {
        GameEventManager.NightStart += NightStart;
    }

    void NightStart()
    {
        sellWindow.position = new Vector3(0f, 30f, 0f); //remove the window
    }

    void OnMouseDown()
    {
		// reap the money from towers that are marked and delete them
		
		int totalSellBackAmount = BaseCohesionManager.DeleteUnconnectedAttachments(true);
		sellWindow.position = new Vector3(0f, 30f, 0f); //remove the window
		
		//do something with the sell back amount
		BlorbManager.Instance.Transaction(totalSellBackAmount, sellWindow.position);

        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
    }

    void OnMouseUp()
    {
        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    void OnMouseEnter()
    {
        GUIManager.Instance.MouseOverUI = true;
        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    void OnMouseExit()
    {
        GUIManager.Instance.MouseOverUI = false;

        //just in case the mouse up event doesn't happen over the object...
        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }
}
