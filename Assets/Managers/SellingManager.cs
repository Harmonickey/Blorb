using UnityEngine;
using System.Collections;

public class SellingManager : MonoBehaviour {

    public Transform sellWindow;
    public Transform amountTextBox;
    public TextMesh amountText;

	void Update () {

        if (Input.GetMouseButtonDown(0) && !Placement.isPlacingTowers) //check if we clicked a tower
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
                    if (sellWindow != null)
                    {
                        sellWindow.gameObject.SetActive(true);
                        amountText.text = "+" + ((int)attachment.sellBackAmount).ToString();
                        sellWindow.FindChild("Amount").renderer.sortingOrder = 1;
                        amountTextBox.renderer.sortingLayerName = "UI";
                        amountTextBox.renderer.sortingOrder = 2;
                        sellWindow.transform.position = new Vector3(attachment.transform.position.x, attachment.transform.position.y + 2.5f);
                    }
                }
            }
        }
	}
}
