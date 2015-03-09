using UnityEngine;
using System.Collections;

public class PlacementBottom : MonoBehaviour {

    public Transform pseudoParent; //for detection logic

    public static Color selectedColor = new Color(0.2f, 0.2f, 0.882f, 1f);
    public static Color unSelectedColor = new Color(0.516f, 0.886f, 0.882f, 1f);

    void Start()
    {
        Placement.possiblePlacements.Add(this);
    }
}

