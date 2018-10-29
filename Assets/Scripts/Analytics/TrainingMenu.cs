using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingMenu : MonoBehaviour, Menu {
	public Transform UIButton;
	public Transform UITextInput;
	private Transform trainRecordButton;
	private Transform trainButton;
	private Transform testButton;
	private Transform plusButton;
	private Transform minusButton;
	private Transform iterationsButton;
	private bool train = false;
	private bool trainRecord = false;
	private bool test = false;
	private int trainingIterations = 1;
	private bool closeTrainingMenu;

	void Awake () {
		this.closeTrainingMenu = false;
		trainRecordButton = Instantiate(UIButton, gameObject.transform);
		trainRecordButton.GetComponent<Image>().color = Color.green;
		trainRecordButton.GetComponent<RectTransform>().localPosition = new Vector3(-350, 250, 10);
		trainRecordButton.GetComponent<RectTransform>().sizeDelta = new Vector2(100.0f, 50.0f);
		UIButtonSetText(trainRecordButton, "Train Record");
		trainButton = Instantiate(UIButton, gameObject.transform);
		trainButton.GetComponent<Image>().color = Color.yellow;
		trainButton.GetComponent<RectTransform>().localPosition = new Vector3(-200, 250, 10);
		trainButton.GetComponent<RectTransform>().sizeDelta = new Vector2(100.0f, 50.0f);
		UIButtonSetText(trainButton, "Train");
		testButton = Instantiate(UIButton, gameObject.transform);
		testButton.GetComponent<Image>().color = Color.cyan;
		testButton.GetComponent<RectTransform>().localPosition = new Vector3(300, 250, 10);
		testButton.GetComponent<RectTransform>().sizeDelta = new Vector2(100.0f, 50.0f);
		UIButtonSetText(testButton, "Test");
		minusButton = Instantiate(UIButton, gameObject.transform);
		minusButton.GetComponent<Image>().color = Color.red;
		minusButton.GetComponent<RectTransform>().localPosition = new Vector3(-50, 250, 10);
		minusButton.GetComponent<RectTransform>().sizeDelta = new Vector2(50.0f, 50.0f);
		UIButtonSetText(minusButton, "-");
		iterationsButton = Instantiate(UITextInput, gameObject.transform);
		iterationsButton.GetComponent<Image>().color = Color.gray;
		iterationsButton.GetComponent<RectTransform>().localPosition = new Vector3(25, 250, 10);
		iterationsButton.GetComponent<RectTransform>().sizeDelta = new Vector2(50.0f, 50.0f);
		UIButtonSetText(iterationsButton, trainingIterations.ToString());
		plusButton = Instantiate(UIButton, gameObject.transform);
		plusButton.GetComponent<Image>().color = Color.red;
		plusButton.GetComponent<RectTransform>().localPosition = new Vector3(100, 250, 10);
		plusButton.GetComponent<RectTransform>().sizeDelta = new Vector2(50.0f, 50.0f);
		UIButtonSetText(plusButton, "+");
	}
	

	void Update () {
		trainRecord = trainRecordButton.GetComponent<TestbenchButton>().getClicked();
		train = trainButton.GetComponent<TestbenchButton>().getClicked();
		test = testButton.GetComponent<TestbenchButton>().getClicked();

		if(plusButton.GetComponent<TestbenchButton>().getClicked()){
			trainingIterations++;
			UIButtonSetText(iterationsButton, trainingIterations.ToString());
		}
		if(minusButton.GetComponent<TestbenchButton>().getClicked() && trainingIterations > 1){
			trainingIterations--;
			UIButtonSetText(iterationsButton, trainingIterations.ToString());
		}
	}

	public bool close(){
		return this.closeTrainingMenu;
	}

	public void closeMenu(){
		this.closeTrainingMenu = true;
	}

	void UIButtonSetText(Transform button, string text){
		button.transform.Find("Text").GetComponent<Text>().text = text;
	}

	public bool getTrainRecord(){
		if(trainRecord){
			trainRecord = false;
			return true;
		}
		return false;
	}
	
	public bool getTrain(){
		if(train){
			train = false;
			return true;
		}
		return false;
	}

	public bool getTest(){
		if(test){
			test = false;
			return true;
		}
		return false;
	}

	public int getTrainingIterations(){
		int its = 0;
		try{
			its = int.Parse(iterationsButton.transform.Find("Text").GetComponent<Text>().text);
		}
		catch{
			return trainingIterations;
		}

		this.trainingIterations = its;
		return trainingIterations;
	}
}
