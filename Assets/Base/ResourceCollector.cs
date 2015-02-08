using UnityEngine;
using System.Collections;

public class ResourceCollector : MonoBehaviour {

	public SpriteRenderer collectorSprite;

	private Resource currentResource;
	private TileMap map;
	private Center center;

	// Use this for initialization
	void Start () {
		map = GameObject.FindWithTag("Map").GetComponent<TileMap>();
		center = GameObject.FindWithTag("Player").GetComponent<Center>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.LeftAlt) && (currentResource != null || FindResource() != null)){
			Debug.Log ("Toggling Resource Attachment");
			ToggleResourceAttachment();
		}

		if (currentResource != null){
			CollectFromResource();
		}
	}

	bool NextToResource(){
		Debug.Log ("Checking if next to resource");
		//Ideally I'd prefer to replace the contents of this function with some sort of message broadcast
		Vector3 currentTile = map.PositionToTile(transform.position);

		int x = (int) currentTile.x;
		int y = (int) currentTile.y;
		Debug.Log ("currentTile = [" + x.ToString() + ", " + y.ToString() + "]\n");

		return (map.IsResource(x+1, y) || map.IsResource(x-1, y) || map.IsResource(x, y+1) || map.IsResource(x, y-1));
	}

	Resource FindResource(){
		Debug.Log ("Checking if next to resource");
		Vector3 currentTile = map.PositionToTile(transform.position);
		Debug.Log ("currentTile = " + currentTile.ToString() + "\n");
		/*Debug.Log ("left = " + (currentTile + Vector3.left).ToString() + "\n");
		Debug.Log ("up = " + (currentTile + Vector3.up).ToString() + "\n");
		Debug.Log ("right = " + (currentTile + Vector3.right).ToString() + "\n");
		Debug.Log ("down = " + (currentTile + Vector3.left).ToString() + "\n");*/

		if (map.IsResource(currentTile + Vector3.left)){
			return map.GetResource(currentTile + Vector3.left);
		} 
		else if (map.IsResource(currentTile + Vector3.up)){
			return map.GetResource(currentTile + Vector3.up);
		} 
		else if (map.IsResource(currentTile + Vector3.right)){
			return map.GetResource(currentTile + Vector3.right);
		}
		else if (map.IsResource(currentTile + Vector3.down)){
			return map.GetResource(currentTile + Vector3.down);
		}

		Debug.Log ("Not next to any resource!");
		return null;

	}

	void ToggleResourceAttachment(){
		if (center.collectingFromResource) {
			currentResource = null;
			center.collectingFromResource = false;
		}
		else {
			currentResource = FindResource();
			center.collectingFromResource = true;
		}
	}

	void CollectFromResource(){
		if (currentResource != null){
			center.resourcePool += currentResource.collectResources();
		}
		else {
			Debug.LogWarning("ResourceCollector: Attempt to collect from resource while currentResource is null!");
		}

		Debug.Log ("resourcePool = " + center.resourcePool.ToString() + "\n");
	}
}
