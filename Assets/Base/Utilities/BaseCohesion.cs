using UnityEngine;
using System.Collections;

public abstract class BaseCohesion {

    public static ArrayList visitedNodes = new ArrayList();
    
	public static void FindAllNeighbors(Transform startingFrom = null)
    {
        //excluding self, can all other pieces connect to the center?
        if (startingFrom == null) startingFrom = GameObject.FindGameObjectWithTag("Player").transform;

        //start at center
        RaycastHit2D[] hits;
        for (int i = 0; i < BuildDirection.Directions.Count; i++)
        {
            
            hits = Physics2D.LinecastAll(new Vector2(startingFrom.position.x + BuildDirection.ToDirFromSpot(i)[0], startingFrom.position.y + BuildDirection.ToDirFromSpot(i)[1]),
                                         new Vector2(startingFrom.position.x + (BuildDirection.ToDirFromSpot(i)[0] * 1.5f), startingFrom.position.y + (BuildDirection.ToDirFromSpot(i)[1]* 1.5f)));

            for (var j = 0; j < hits.Length; j++)
            {
                Transform attachment = hits[j].collider.transform.parent; // 'parent' because it's hitting the 2D collider (I believe)
                if (attachment.GetComponent<Attachments>() != null &&
                    !visitedNodes.Contains(attachment.GetComponent<Attachments>().GetInstanceID()))
                {
                    visitedNodes.Add(attachment.GetComponent<Attachments>().GetInstanceID());
                    attachment.GetComponent<Attachments>().wasFound = true;

                    FindAllNeighbors(attachment); 
                }
            }
        
        }
    }
}
