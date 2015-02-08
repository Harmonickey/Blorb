using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

	public static float fireDelay = 1f;
	public static float range = 100f;
	public static float damage = 25f;
	private Transform myTarget;
	private float nextFireTime;
    private SpriteRenderer gun;
    public float publicAngle;

	// Use this for initialization
	void Start () {
        gun = this.transform.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		// decide if its time to fire
		if (nextFireTime < Time.time) {
			if (myTarget) {
				myTarget.gameObject.SendMessage("takeDamage", damage);
			}

			myTarget = GetNearestTaggedObject();
			nextFireTime = Time.time + fireDelay;
		}

		if (myTarget) {
			Vector3 moveDirection = myTarget.position - gun.transform.position; 
			if (moveDirection != Vector3.zero) 
			{
				float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
                
				gun.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
			}
		}
	}

	Transform GetNearestTaggedObject() {
		
		float nearestDistanceSqr = Mathf.Infinity;
		GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag("Enemy"); 
		Transform nearestObj = null;
		
		// loop through each tagged object, remembering nearest one found
        // this can be massively slow with lots of enemies
		foreach (GameObject obj in taggedGameObjects) {
			
			Vector3 objectPos = obj.transform.position;
			float distanceSqr = (objectPos - transform.position).sqrMagnitude;
			
			if (distanceSqr < range && distanceSqr < nearestDistanceSqr) {
				nearestObj = obj.transform;
				nearestDistanceSqr = distanceSqr;
			}
		}
		
		return nearestObj;
	}
}
