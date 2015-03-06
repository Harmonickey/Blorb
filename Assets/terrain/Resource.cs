using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour {
	private int initial = 500;
	private int remaining;
	private SpriteRenderer mountain;
    
	void Start(){
		remaining = initial;
		mountain = transform.GetComponent<SpriteRenderer> ();
	}

	public int deplete(int amount)
	{
		int give;

		if (amount > remaining) {
			give = remaining;
			remaining = 0;
		} else {
			give = amount;
			remaining -= amount;
		}

		mountain.color = new Color (1f, 1f, 1f, (initial - remaining) / initial);

		return give;
	}
}
