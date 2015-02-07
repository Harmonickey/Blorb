using UnityEngine;
using System.Collections;

public class ResourcePool : MonoBehaviour {

	public float contents;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Add(int val){
		contents += val;
	}
}
