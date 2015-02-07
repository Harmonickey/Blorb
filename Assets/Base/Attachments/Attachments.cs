using UnityEngine;
using System.Collections;

public class Attachments : MonoBehaviour {

    private float health = 100F;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
	
	}

    public void Damage(float damage)
    {
        health -= damage;

        Debug.Log("ATTACHMENT HEALTH: " + health);
    }
}
