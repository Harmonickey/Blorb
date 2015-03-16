using UnityEngine;
using System.Collections;

public class BlorbIndicator : MonoBehaviour {
	private Vector3 pos;
	private Color color;
	private float startFade;
	public SpriteRenderer blorbSymbol;

	private TextMesh textMesh;

	public void SetDiff (int d) {
		startFade = 0.5f;
		pos = transform.position;

		textMesh = this.GetComponent<TextMesh>();
		textMesh.renderer.sortingLayerName = "UI";
		textMesh.renderer.sortingOrder = -1;

		if (d > 0) {
			textMesh.text = "+" + d.ToString();
			color = new Color(0f, 1f, 0f, 1f);
		} else {
			textMesh.text = d.ToString();
			color = new Color(1f, 0f, 0f, 1f);
		}

		if (GUIManager.Instance.OnTutorialScreen) {
			textMesh.renderer.enabled = false;
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
		blorbSymbol.color = color;
		transform.position = pos;

		if (!textMesh.renderer.enabled) {
			textMesh.renderer.enabled = true;
		}
	}
}
