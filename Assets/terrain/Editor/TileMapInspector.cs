using UnityEditor;
using UnityEngine;
using System.Collections;


[CustomEditor(typeof(TileMap))]
public class TileMapInspector : Editor {

	public override void OnInspectorGUI(){
		base.OnInspectorGUI();

		TileMap map = (TileMap) target;
		/*if (GUILayout.Button("Regenerate")){
			map.Regenerate();
		}*/
	}
}
