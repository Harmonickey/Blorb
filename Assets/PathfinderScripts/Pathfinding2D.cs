using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding2D : MonoBehaviour
{
    public List<Vector3> Path = new List<Vector3>();
    public bool JS = false;

    private bool testing = false;

    public static bool hasPath = false;

    public void FindPath(Vector3 startPosition, Vector3 endPosition, bool isTesting)
    {
        //Debug.Log("Has Instance: " + (Pathfinder2D.Instance != null));
        testing = isTesting;
        hasPath = false;
        Pathfinder2D.Instance.InsertInQueue(startPosition, endPosition, SetList);
    }

    public void FindJSPath(Vector3[] arr)
    {
        if (arr.Length > 1)
        {
            Pathfinder2D.Instance.InsertInQueue(arr[0], arr[1], SetList);
        }
    }

    //A test move function, can easily be replaced
    public void Move()
    {
        if (Path.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, Path[0], Time.deltaTime * 30F);
            if (Vector3.Distance(transform.position, Path[0]) < 0.4F)
            {
                Path.RemoveAt(0);
            }
        }
    }

    protected virtual void SetList(List<Vector3> path)
    {
        if (path == null || path.Count == 0)
        {
            if (testing)
                GameObject.FindObjectOfType<Center>().SendMessage("SetHasPath", false);

            return;
        }

        if (!JS)
        {
            Path.Clear();
            Path = path;

			Vector3 previous = transform.position;
			foreach (Vector3 n in Path){
				Debug.DrawLine(previous, n, Color.red, .5f);
				previous = n;
			}

            Path[0] = new Vector3(Path[0].x, Path[0].y, Path[0].z);
            Path[Path.Count - 1] = new Vector3(Path[Path.Count - 1].x, Path[Path.Count - 1].y, Path[Path.Count - 1].z);

            if (testing)
                GameObject.FindObjectOfType<Center>().SendMessage("SetHasPath", true);

        }
        else
        {           
            Vector3[] arr = new Vector3[path.Count];
            for (int i = 0; i < path.Count; i++)
            {
                arr[i] = path[i];
            }

            arr[0] = new Vector3(arr[0].x, arr[0].y , arr[0].z);
            arr[arr.Length - 1] = new Vector3(arr[arr.Length - 1].x, arr[arr.Length - 1].y, arr[arr.Length - 1].z);
            gameObject.SendMessage("GetJSPath", arr);
        }
    }
}
