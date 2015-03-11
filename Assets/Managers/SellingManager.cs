using UnityEngine;
using System.Collections;

public class SellingManager : MonoBehaviour {

    public Transform sellWindow;
    public TextMesh amountText;

    public SpriteRenderer sellButton, cancelButton;

    public bool isNight = false;

	void Start ()
	{
		amountText.renderer.sortingLayerName = "UI";
		amountText.renderer.sortingOrder = 2;

        isNight = false;

        GameEventManager.NightStart += NightStart;
	}

    void NightStart()
    {
        isNight = true;
    }

	void Update () {

        if (Input.GetMouseButtonDown(0) &&  //check if we clicked a tower
            !Placement.isPlacingTowers && 
            !GUIManager.Instance.MouseOverUI &&
            !isNight) 
        {
            Attachments[] attachments = GameObject.FindObjectsOfType<Attachments>();
            Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            mouse = new Vector3(mouse.x, mouse.y, 0);

            foreach (Attachments attachment in attachments)
            {
                if (attachment.collider.bounds.Contains(mouse))
                {
                    BaseCohesionManager.UnMarkAllAttachments();
                    BaseCohesionManager.FindAllNeighbors(attachment.transform); // find out our base cohesion network
                    BaseCohesionManager.MarkAllAttachments(attachment.transform);

                    //load up the selling GUI
	                amountText.text = "+" + ((int)attachment.sellBackAmount).ToString();

                    cancelButton.color = new Color(1f, 1f, 1f, 1f);
                    sellButton.color = new Color(1f, 1f, 1f, 1f);
	                sellWindow.transform.position = new Vector3(attachment.transform.position.x, attachment.transform.position.y + 2.5f);
                }
            }
        }
	}
}
