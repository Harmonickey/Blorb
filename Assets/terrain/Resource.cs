using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour {
    
	public int value = 1000;
    public int depletionRate = 1;
    
	void Start(){

	}
    
	public int collectBlorb(){
		if (value > depletionRate){ //if can remove depletionRate amount 
	        value -= depletionRate;
	        return depletionRate;
		}
		else if (value > 0){
			Debug.Log ("Case B");
			int remainder = value;
			value = 0;
			return remainder;
		}
		else{
			Debug.Log ("Case C");
			return 0;
		}
    }
    
    public void EmptyResource(){
        value = 0;
    }

	public void SetResource(int x){
		value = x;
	}
    
}
