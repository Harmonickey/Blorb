using UnityEngine;
using System.Collections;

public abstract class BaseCohesion {

    public static ArrayList visitedNodes = new ArrayList();
    
	public static void FindAllNeighbors(Transform startingFrom = null, Transform deletingAttachment = null)
    {
        //excluding self, can all other pieces connect to the center?
        if (startingFrom == null) 
            startingFrom = GameObject.FindGameObjectWithTag("Player").transform; //set our starting node

        if (deletingAttachment != null && !visitedNodes.Contains(deletingAttachment.GetInstanceID())) 
            visitedNodes.Add(deletingAttachment.GetInstanceID()); //add the node we're trying to delete, so we skip over it
        
        //start at center at linecast in every direction
        RaycastHit2D[] hits;
        for (int i = 0; i < BuildDirection.Directions.Count; i++)
        {
           
            hits = Physics2D.LinecastAll(new Vector2(startingFrom.position.x + BuildDirection.ToDirFromSpot(i)[0], startingFrom.position.y + BuildDirection.ToDirFromSpot(i)[1]),
                                         new Vector2(startingFrom.position.x + (BuildDirection.ToDirFromSpot(i)[0] * 1.3f), startingFrom.position.y + (BuildDirection.ToDirFromSpot(i)[1]* 1.3f)));
            
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
}
