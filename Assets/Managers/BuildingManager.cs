using UnityEngine;
using System.Collections;

public abstract class BuildingManager
{
    //would like to improve these data structures into Vector objects
    //      then we could access the 'x' value instead of the [0] value
    public static float[] Up = new float[3] { 0, 1.0f, 0 };  //0
    public static float[] UpMid = new float[3] { 0.5f, 1.0f, 1f }; //1
    public static float[] UpRightMid = new float[3] { 1.0f, 0.5f, 2f }; //2
    public static float[] Right = new float[3] { 1.0f, 0, 3f };  //3
    public static float[] RightMid = new float[3] { 1.0f, -0.5f, 4f }; //4
    public static float[] RightDownMid = new float[3] { 0.5f, -1.0f, 5f }; //5
    public static float[] Down = new float[3] { 0, -1.0f, 6f };  //6
    public static float[] DownMid = new float[3] { -0.5f, -1.0f, 7f }; //7
    public static float[] DownLeftMid = new float[3] { -1.0f, -0.5f, 8f }; //8
    public static float[] Left = new float[3] { -1.0f, 0, 9f };  //9
    public static float[] LeftMid = new float[3] { -1.0f, 0.5f, 10f }; //10
    public static float[] LeftUpMid = new float[3] { -0.5f, 1.0f, 11f }; //11

    public static ArrayList Directions =
        new ArrayList() { Up, UpMid, UpRightMid,
                          Right, RightMid, RightDownMid,
                          Down, DownMid, DownLeftMid, 
                          Left, LeftMid, LeftUpMid };

    public static int OppositeSpot(int spot)
    {
        return (spot + 6) % 12;
    }

    public static int ToSpotFromDir(float[] dir)
    {
        return (int)dir[2];
    }

    public static float[] ToDirFromSpot(int spot)
    {
        return (float[])Directions[spot];
    }


    //we are only testing in one direction at a time
    public static bool DetectOtherObjects(float[] dir, Transform startingFrom)
    {

        float[] localDir = new float[2] { dir[0], dir[1] }; //since we are passing by reference we need to make our own local
        localDir[0] *= 0.9f; //scale the distance
        localDir[1] *= 0.9f;
        RaycastHit2D[] hits = Physics2D.LinecastAll(new Vector2(startingFrom.position.x + localDir[0], startingFrom.position.y + localDir[1]),
                                     new Vector2(startingFrom.position.x + (localDir[0] * 1.1f), startingFrom.position.y + (localDir[1] * 1.1f)));

        //Debug.DrawLine(new Vector3(startingFrom.position.x + localDir[0], startingFrom.position.y + localDir[1]),
        //               new Vector3(startingFrom.position.x + (localDir[0] * 1.1f), startingFrom.position.y + (localDir[1] * 1.1f)), Color.blue, 30.0f, false);

        
        //blacklist pseudo-children, and other placement pieces
        ArrayList blackList = new ArrayList();
        for (int j = 0; j < hits.Length; j++)
        {
            if (hits[j].collider.GetComponent<PlacementBottom>() != null &&
                hits[j].collider.GetComponent<PlacementBottom>().pseudoParent == startingFrom)
            {
                blackList.Add(j);
            }
        }

        for (int j = 0; j < hits.Length; j++)
        {
            if (blackList.Contains(j)) continue; //skip pseudo-children and placement pieces

            //okay, we found one, don't place here!
            return true;
        }

        return false;
    }
}
