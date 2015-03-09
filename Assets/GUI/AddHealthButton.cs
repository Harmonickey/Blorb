using UnityEngine;
using System.Collections;

public class AddHealthButton : MonoBehaviour {
	public int cost;
	public TextMesh nameText;
	public TextMesh costText;
	public SpriteRenderer pictograph;

	void setTowerDetail (bool enabled)
	{
		pictograph.renderer.enabled = enabled;
		pictograph.transform.parent.renderer.enabled = enabled;
		nameText.text = "Add Health";
		nameText.renderer.enabled = enabled;
		costText.text = cost.ToString ();
		costText.renderer.enabled = enabled;
	}
	
    void OnMouseDown () 
    {
		if (!GUIManager.Instance.OnTutorialScreen && Time.timeScale != 0f &&
		    BlorbManager.Instance.BlorbAmount >= cost && Center.Instance.health <= 90f) {
			BlorbManager.Instance.Transaction(-cost, transform.position);
			Center.Instance.health += 10f;
		}
    }

    void OnMouseEnter()
    {
		if (GUIManager.Instance.ViewStage == 2) {
	        GUIManager.Instance.MouseOverUI = true;
			setTowerDetail (true);
		}
    }

    void OnMouseExit()
    {
        GUIManager.Instance.MouseOverUI = false;
		setTowerDetail (false);
    }
}
