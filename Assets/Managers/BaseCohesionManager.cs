using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseCohesionManager {

    public static List<int> visitedNodes = new List<int>();
    private static List<Attachments> markedAttachments = new List<Attachments>();
    
	public static void FindAllNeighbors(Transform deletingAttachment, Transform startingFrom = null)
    {
        //excluding self, can all other pieces connect to the center?
        if (startingFrom == null) 
            startingFrom = GameObject.FindGameObjectWithTag("Player").transform; //set our starting node
        
        if (!visitedNodes.Contains(deletingAttachment.GetInstanceID()))
            visitedNodes.Add(deletingAttachment.GetInstanceID()); //add the node we're trying to delete, so we skip over it
        
        //start at center at linecast in every direction
        RaycastHit2D[] hits;
        float distance = 0.9f; // may want to play with this value, as it might reach too far and catch attachments that shouldn't be caught
        for (int i = 0; i < BuildingManager.Directions.Count; i++)
        {
            float[] localDir = new float[2] { BuildingManager.ToDirFromSpot(i)[0], BuildingManager.ToDirFromSpot(i)[1] };
            localDir[0] *= 0.9f;
            localDir[1] *= 0.9f;

            hits = Physics2D.LinecastAll(new Vector2(startingFrom.position.x + localDir[0], startingFrom.position.y + localDir[1]),
                                         new Vector2(startingFrom.position.x + (localDir[0] * distance), startingFrom.position.y + (localDir[1] * distance)));
            
            // see where the linecast is going
            //Debug.DrawLine(new Vector3(startingFrom.position.x + localDir[0], startingFrom.position.y + localDir[1]),
            //               new Vector3(startingFrom.position.x + (localDir[0] * distance), startingFrom.position.y + (localDir[1] * distance)), Color.blue, 10.0f, false);
            
            for (var j = 0; j < hits.Length; j++)
            {
                Transform attachment = hits[j].collider.transform.parent; // 'parent' because it's hitting the child 2D collider
                
                if (attachment.GetComponent<Attachments>() != null &&  // is actually an attachment
                    !visitedNodes.Contains(attachment.GetInstanceID())) // haven't visited it
                {
                    visitedNodes.Add(attachment.GetInstanceID());
                    attachment.GetComponent<Attachments>().wasFound = true; //set all the attachments to 'found' if they're part of our hit list

                    FindAllNeighbors(deletingAttachment, attachment);  //recurse into them
                }
            }
        }
    }

    //mark all the attachments that are not attached to the main body
    //     this is so the user knows which are going to be deleted, for example
    public static void MarkAllAttachments(Transform deletingAttachment)
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

        ResetAttachmentCohesionChecker();
    }

    private static void ResetAttachmentCohesionChecker()
    {
        foreach (Attachments attachment in GameObject.FindObjectsOfType<Attachments>())
        {
            attachment.wasFound = false;
        }

        markedAttachments.Clear();
        visitedNodes.Clear();
    }

    //delete all attachments that are not attached to main body
    public static int DeleteUnconnectedAttachments(bool useMarked)
    {
        int totalSellBack = 0;

        if (useMarked)
        {
            foreach (Attachments attachment in markedAttachments)
            {
                totalSellBack += attachment.sellBackAmount;
                Object.Destroy(attachment.gameObject);
            }

            ResetAttachmentCohesionChecker();
        }
        else
        {
            foreach (Attachments attachment in GameObject.FindObjectsOfType<Attachments>())
            {
                if (!attachment.wasFound)
                {
                    Object.Destroy(attachment.gameObject);
                }
            }
        }

        return totalSellBack;
    }

    private static void TurnRed(Transform targetPiece, bool red)
    {
        foreach (Transform child in targetPiece)
        {
            if (child.GetComponent<SpriteRenderer>() != null)
            {
                Color change = (red ? new Color(1f, 0f, 0f) : new Color(1f, 1f, 1f));
                child.GetComponent<SpriteRenderer>().color = change; //turn them red
            }
        }
    }
}
