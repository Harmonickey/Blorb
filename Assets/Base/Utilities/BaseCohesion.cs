using UnityEngine;
using System.Collections;

public abstract class BaseCohesion {

    public static ArrayList visitedNodes = new ArrayList();
    private static ArrayList markedAttachments = new ArrayList();
    
	public static void FindAllNeighbors(Transform startingFrom = null, Transform deletingAttachment = null)
    {
        //excluding self, can all other pieces connect to the center?
        if (startingFrom == null) 
            startingFrom = GameObject.FindGameObjectWithTag("Player").transform; //set our starting node

        if (deletingAttachment != null && !visitedNodes.Contains(deletingAttachment.GetInstanceID())) 
            visitedNodes.Add(deletingAttachment.GetInstanceID()); //add the node we're trying to delete, so we skip over it
        
        //start at center at linecast in every direction
        RaycastHit2D[] hits;
        float distance = 1.3f; // may want to play with this value, as it might reach too far and catch attachments that shouldn't be caught
        for (int i = 0; i < BuildDirection.Directions.Count; i++)
        {
           
            hits = Physics2D.LinecastAll(new Vector2(startingFrom.position.x + BuildDirection.ToDirFromSpot(i)[0], startingFrom.position.y + BuildDirection.ToDirFromSpot(i)[1]),
                                         new Vector2(startingFrom.position.x + (BuildDirection.ToDirFromSpot(i)[0] * distance), startingFrom.position.y + (BuildDirection.ToDirFromSpot(i)[1] * distance)));
            
            /* // see where the linecast is going
            Debug.DrawLine(new Vector2(startingFrom.position.x + BuildDirection.ToDirFromSpot(i)[0], startingFrom.position.y + BuildDirection.ToDirFromSpot(i)[1]),
                           new Vector2(startingFrom.position.x + (BuildDirection.ToDirFromSpot(i)[0] * 1.3f), startingFrom.position.y + (BuildDirection.ToDirFromSpot(i)[1] * 1.3f)), Color.blue, 3.0f, false);
            */

            for (var j = 0; j < hits.Length; j++)
            {
                Transform attachment = hits[j].collider.transform.parent; // 'parent' because it's hitting the 2D collider
                if (attachment.GetComponent<Attachments>() != null &&
                    !visitedNodes.Contains(attachment.GetComponent<Attachments>().GetInstanceID()))
                {
                    visitedNodes.Add(attachment.GetComponent<Attachments>().GetInstanceID());
                    attachment.GetComponent<Attachments>().wasFound = true; //set all the attachments to 'found' if they're part of our hit list

                    FindAllNeighbors(attachment);  //recurse into them
                }
            }
        
        }
    }

    //mark all the attachments that are not attached to the main body
    //     this is so the user knows which are going to be deleted, for example
    public static void MarkAllBrokenAttachments(Transform deletingAttachment)
    {
        Attachments[] attachments = GameObject.FindObjectsOfType<Attachments>();
        TurnRed(deletingAttachment, true);

        foreach (Attachments attachment in attachments)
        {
            if (!attachment.wasFound)
            {
                TurnRed(attachment.transform, true);
                markedAttachments.Add(attachment);
            }
        }
    }

    public static void UnMarkAllAttachments()
    {
        foreach (Attachments attachment in markedAttachments)
        {
            TurnRed(attachment.transform, false);
        }
    }

    //delete all attachments that are not attached to main body
    public static void DeleteAllBrokenAttachments()
    {
        foreach (Attachments attachment in markedAttachments)
        {
            Object.Destroy(attachment.gameObject);
        }
    }

    private static void TurnRed(Transform targetPiece, bool red)
    {
        foreach (Transform child in targetPiece)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                child.GetComponent<SpriteRenderer>().color = new Color((red ? 1f : 0f), 0f, 0f); //turn them red
            }
        }
    }
}
