using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	public Transform health;

	public void Reset()
	{
		Set (1f);
	}

	public void Set(float healthPercent)
	{
		float scale = health.localScale.y;
		health.localScale = new Vector2 (healthPercent * scale, scale);
	}
}
