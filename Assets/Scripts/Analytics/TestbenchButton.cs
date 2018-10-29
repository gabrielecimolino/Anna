using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestbenchButton : MonoBehaviour {

	public bool clicked = false;

	public void click(){
		clicked = true;
	}

	public bool getClicked(){
		bool temp = clicked;
		clicked = false;
		return temp;
	}
}
