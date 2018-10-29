using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableSelector : MonoBehaviour {

	public int value = 1;
	[SerializeField]private Text selectorName;
	[SerializeField]private InputField inputField;
	[SerializeField]private Text inputFieldText;
	private int incorrectInputs = 0;

	public void Initialize(Text selectorName, InputField inputField){
		this.value = 1;
		this.incorrectInputs = 0;
		this.selectorName = selectorName;
		this.inputField = inputField;
	}


	public void setSelectorName(string name){
		this.selectorName.text = name;
	}

	public void setSelectorValue(int value){
		if(value > 0){
			this.value = value;
			setInputText(value.ToString());
		}
	}

	public void getInputText(){
		string inputText = inputField.text;
		int input = 0;
		try{
			input = int.Parse(inputText);
			this.incorrectInputs = 0;
			if(input > 0){
				this.value = input;
			}
		}
		catch{
			switch(incorrectInputs){
				case 0:
					setInputText("No");
					break;
				case 1: 
					setInputText("Pls");
					break;
				case 2:
					setInputText("Stop");
					break;
				default:
					setInputText("やめる");//"止める");
					break;
			}

			this.incorrectInputs++;
		}
	}

	public void setInputText(string InputText){
		this.inputField.text = InputText;
	}

	public void setTextColor(Color color){
		this.inputFieldText.color = color;
	}

	public void inc(){
		this.value++;
		setInputText(value.ToString());
	}

	public void dec(){
		if(this.value > 1){
			this.value--;
			setInputText(value.ToString());
		}
	}
}
