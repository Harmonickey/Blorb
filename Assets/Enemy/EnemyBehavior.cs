using UnityEngine;
using System.Collections.Generic;

public class EnemyBehavior : Pathfinding {

    //public Transform prefab;
    //public int numberOfObjects;
    //private Queue<Transform> objectQueue;
    /*
	// Use this for initialization
	void Start () {
        //objectQueue = new Queue<Transform>(numberOfObjects);
        //for (int i = 0; i < numberOfObjects; i++)
        //{
        //    objectQueue.Enqueue((Transform)Instantiate(prefab,
        //        new Vector3(0f, 0f, -100f), Quaternion.identity));
        //}


	}*/
	
    Vector3 endPosition = new Vector3(5, 0, 5);
	// Update is called once per frame
	void Update () {
        //If i hit the P key i will get a path from my position to my end position
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("PRESSED P");
            FindPath(transform.position, endPosition);
        }
        //If path count is bigger than zero then call a move method
        if (Path.Count > 0)
        {
            Move();
        }
	}

    public void Move()
    {
        if (Path.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, Path[0], Time.deltaTime * 30F);
            if (Vector3.Distance(transform.position, Path[0]) < 0.1F)
            {
                Path.RemoveAt(0);
            }
        }
    }
}
