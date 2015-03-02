using UnityEngine;
using System.Collections;

public class BlorbManager : MonoBehaviour {
	public TextMesh blorbAmountText;
	public Transform map;
	private static BlorbManager instance;
	private float blorbAmount;

	public static BlorbManager Instance
	{
		get { return instance; }
	}

	public float BlorbAmount {
		get { return blorbAmount; }
	}

	void Start ()
	{
		instance = this;
		blorbAmountText.renderer.sortingLayerName = "UI";
		blorbAmountText.renderer.sortingOrder = 2;
	}

	public float Set(float amount, Vector3 position)
	{
		blorbAmount = 0f;
		return Transaction (amount, position);
	}

	public float Transaction(float diff, Vector3 position)
	{
		blorbAmount += diff;
		blorbAmountText.text = ((int)blorbAmount).ToString();
		GUIManager.UpdateTowerGUI(blorbAmount);
		
		if (Mathf.Abs(diff) > 0f) {
			// Add blorb indicator
			GameObject g = ObjectPool.instance.GetObjectForType("BlorbIndicator", true);
			g.transform.position = position;
			g.transform.parent = map;
			BlorbIndicator b = g.GetComponent<BlorbIndicator>();
			b.SetDiff(diff);
		}

		return blorbAmount;
	}
}
