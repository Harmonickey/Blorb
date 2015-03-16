using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	public Transform health;
	private bool hideWhenFull = false;
	private SpriteRenderer front, back;

	public void Reset()
	{
		front = transform.FindChild ("Health Bar Front").GetComponent<SpriteRenderer> ();
		back = transform.FindChild ("Health Bar Back").GetComponent<SpriteRenderer> ();

		Set (1f);
	}

	public void HideWhenFull()
	{
		hideWhenFull = true;
	}

	public void Set(float healthPercent)
	{
		float scale = health.localScale.y;
		health.localScale = new Vector2 (healthPercent * scale, scale);

		if (hideWhenFull && healthPercent == 1f) {
			front.renderer.enabled = false;
			back.renderer.enabled = false;
		} else if (!front.renderer.enabled) {
			front.renderer.enabled = true;
			back.renderer.enabled = true;
		}
	}
}
