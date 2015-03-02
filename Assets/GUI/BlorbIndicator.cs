using UnityEngine;
using System.Collections;

public class BlorbIndicator : MonoBehaviour {
	private Vector3 pos;
	private Color color;
	private float startFade;

	private TextMesh textMesh;

	public void SetDiff (float d) {
		startFade = 0.5f;
		pos = transform.position;

		textMesh = this.GetComponent<TextMesh>();

		if (d > 0) {
			textMesh.text = "+" + ((int)d).ToString();
			color = new Color(0f, 1f, 0f, 1f);
		} else {
			textMesh.text = ((int)d).ToString();
			color = new Color(1f, 0f, 0f, 1f);
		}

	}
	
	void Update () {
		startFade -= Time.deltaTime;
		pos.y += Time.deltaTime * 2f;

		if (startFade <= 0f) {
			color.a -= Time.deltaTime / 2f;
		}

		if (color.a <= 0f) {
			ObjectPool.instance.PoolObject(gameObject);
		}

		textMesh.color = color;
		transform.position = pos;
	}
}
