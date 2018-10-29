using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITestbench : MonoBehaviour, Menu {

	public GameObject neuralNetworkViewPrefab;
	public GameObject trainingMenuPrefab;

	private GameObject trainingMenu = null;
	private GameObject view = null;

	private Testbench newTestbench = null;

	private float baseRate = 0.0f;

	private string mostCommonValue = "null";

	private bool closeTestbench;

	void Awake(){
		this.closeTestbench = false;
	}
	
	void Update () {
		if(trainingMenu != null){
			if(newTestbench != null){
				if(trainingMenu.GetComponent<TrainingMenu>().close()){
					closeMenu();
				}
				else{
					if(trainingMenu.GetComponent<TrainingMenu>().getTrainRecord()){
						newTestbench.nextTrain();
					}		
					if(trainingMenu.GetComponent<TrainingMenu>().getTrain()){
						float startTime = Time.realtimeSinceStartup;
						ConfusionMatrix confusion = newTestbench.train(trainingMenu.GetComponent<TrainingMenu>().getTrainingIterations());
						float endTime = Time.realtimeSinceStartup;
						Debug.Log("Training Time: " + (endTime - startTime).ToString("0.0") + " seconds");
						Debug.Log("Training prediction accuracy: " +  (confusion.predictionAccuracy * 100).ToString() + "%");
					}	
					if(trainingMenu.GetComponent<TrainingMenu>().getTest()){
						ConfusionMatrix confusion = newTestbench.test();
						Debug.Log("Testing prediction accuracy: " +  (confusion.predictionAccuracy * 100).ToString() + "%");
					}	
				}
			}	
		}
	}

	public bool close(){
		return this.closeTestbench;
	}

	public void closeMenu(){
		Destroy(this.trainingMenu);
		Destroy(this.view);

		this.closeTestbench = true;
	}

    IEnumerator trainCoroutine(float waitTime, float trainingTime){

        yield return new WaitForSeconds(waitTime);
        trainingTime += waitTime;
        Debug.Log("Training time: " + trainingTime.ToString());
        StartCoroutine(trainCoroutine(waitTime, trainingTime));
    }

	public void Initialize(Testbench testbench){
		this.newTestbench = testbench;
		this.view = Instantiate(neuralNetworkViewPrefab, Vector3.zero, Quaternion.identity, this.transform);
		this.view.GetComponent<NeuralNetworkView>().setNeuralNetwork(testbench.network, "full");
		List<string> inputNames = new List<string>();
        List<string> activationFunctions = new List<string>();
		if(testbench.dataset.singularValues != null){
			inputNames = new List<string>(Functions.map((x => "Singular value: " + x.ToString()), testbench.dataset.singularValues.ToArray()));
		}
		else{
			inputNames = testbench.dataset.getInputNames();
		}
		this.view.GetComponent<NeuralNetworkView>().setNeuronNames(inputNames, new List<string>(testbench.dataset.getTargetStringClasses()));
        this.view.GetComponent<NeuralNetworkView>().setNeuronActivationFunctions(testbench.network.getActivationFunctions());
		this.trainingMenu = Instantiate(trainingMenuPrefab, Vector3.zero, Quaternion.identity, this.transform);
		this.trainingMenu.GetComponent<Canvas>().worldCamera = view.GetComponent<NeuralNetworkView>().camera;
		this.trainingMenu.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera; 
		this.trainingMenu.GetComponent<Canvas>().planeDistance = 1.0f; 
	}
}
