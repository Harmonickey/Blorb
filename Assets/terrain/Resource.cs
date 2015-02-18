using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour {
    
	public float value = 1000f;
    public float depletionRate = 0f;
    
	void Start(){

	}
    
	public float collectBlorb(){
		if (value > depletionRate){ //if can remove depletionRate amount 
	        value -= depletionRate;
	        return depletionRate;
		}
		else if (value > 0){
			Debug.Log ("Case B");
			float remainder = value;
			value = 0;
			return remainder;
		}
		else{
			Debug.Log ("Case C");
			return 0f;
		}
    }
    
    public void EmptyResource(){
        value = 0;
    }

	public void SetResource(int x){
		value = x;
	}
    
}
