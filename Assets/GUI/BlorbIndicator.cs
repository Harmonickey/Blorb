using UnityEngine;
using System.Collections;

public class BlorbIndicator : MonoBehaviour {
	private Vector3 pos;
	private Color color;

	private TextMesh textMesh;

	public void setDiff (int d) {
		textMesh = this.GetComponent<TextMesh>();
		pos = transform.position;

		if (d > 0) {
			textMesh.text = "+" + d.ToString();
			color = new Color(0f, 1f, 0f, 1f);
		} else {
			textMesh.text = d.ToString();
			color = new Color(1f, 0f, 0f, 1f);
		}

	}
	
	void Update () {
		color.a -= Time.deltaTime / 2f;
		pos.y -= Time.deltaTime * 2f;

		if (color.a <= 0f) {
			Object.Destroy(gameObject);
		}

		textMesh.color = color;
		transform.position = pos;
	}
}
