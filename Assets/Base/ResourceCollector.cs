using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class ResourceCollector : MonoBehaviour {
	private float range = 50f;
	private Transform myTarget;
	private SpriteRenderer collector;
	private float nextCollect;

	public ParticleSystem theParticleSystem;
	public Transform mountain;
	private float collectionRate = 2f; //Time in seconds between collections
	private int collectionAmount = 2;
	private int length;
	private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[100];
	
	void LateUpdate () {
		if (!theParticleSystem) {
			return;
		}

		length = theParticleSystem.GetParticles(particles);
		for (int i = 0; i < particles.Length; i++) {
			Vector3 rp = particles[i].position + theParticleSystem.transform.localPosition;

			if (rp.magnitude < 0.2f) {
				particles[i].lifetime = -1f;
				if (myTarget && Time.time > nextCollect) {
					int amount = myTarget.GetComponent<Resource>().deplete (collectionAmount);
					BlorbManager.Instance.Transaction(amount, myTarget.position);
					nextCollect = Time.time + collectionRate;

					if (amount == 0) {
						myTarget.gameObject.tag = "Mountain";
						theParticleSystem.Stop();
					}
				}
			} else {
				particles[i].position = Vector3.MoveTowards(particles[i].position, -theParticleSystem.transform.localPosition, 10f * Time.deltaTime);
			}
		}
		theParticleSystem.SetParticles(particles, length);
	}

	void Start () {
		//collectorSprite = GetComponent<SpriteRenderer>();
		collector = this.transform.GetComponent<SpriteRenderer> ();
		theParticleSystem = this.transform.parent.GetComponentInChildren<ParticleSystem> ();
	}
	
	void Update () {
		// doing this the expensive way for now. TODO make this more efficient
		myTarget = GetNearestTaggedObject();

		if (myTarget) {
			Vector3 moveDirection = myTarget.position - collector.transform.position; 
			if (moveDirection != Vector3.zero) 
			{
				float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
				
				collector.transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
			}

			theParticleSystem.transform.position = myTarget.position;
            
			if (!theParticleSystem.isPlaying) {
				theParticleSystem.Play();
			}
		} else {
			theParticleSystem.Stop ();
		}
	}

	Transform GetNearestTaggedObject() {
		
		float nearestDistanceSqr = Mathf.Infinity;
		GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag("Resource"); 
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
