using UnityEngine;
using System.Collections;

public class BlorbManager : MonoBehaviour {
	public TextMesh blorbAmountText;
	public Transform map;
	private static BlorbManager instance;
	private int blorbAmount;

	public static BlorbManager Instance
	{
		get { return instance; }
	}

	public int BlorbAmount {
		get { return blorbAmount; }
	}

	void Start ()
	{
		instance = this;
		blorbAmountText.renderer.sortingLayerName = "UI";
		blorbAmountText.renderer.sortingOrder = 2;
		GameEventManager.GameStart += GameStart;
	}

	void GameStart ()
	{
		StartCoroutine (DelayBlorb ());
	}

	IEnumerator DelayBlorb()
	{       
		yield return new WaitForSeconds(0.01f);
		BlorbManager.Instance.Set (100, Center.Instance.transform.position);
	}

	public float Set(int amount, Vector3 position)
	{
		blorbAmount = 0;
		return Transaction (amount, position);
	}

	public float Transaction(int diff, Vector3 position)
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
