using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkView : MonoBehaviour, Menu {

	public GameObject neuronPrefab;

	public GameObject neuralNetworkViewCameraPrefab;

	public NeuralNetwork network;

	private List<List<GameObject>> neurons;

	public GameObject cameraObject;
	public Camera camera;

	private Vector2 cameraBounds;

	private Vector3 neuronScale;

	private bool closeView;

	void Awake () {
		neurons = new List<List<GameObject>>();
		this.closeView = false;
		// NeuralNetwork newNetwork = new NeuralNetwork(9, new List<int>(){3}, 9);
		// newNetwork.randomizeWeights();
		// newNetwork.randomizeBiases();
		// newNetwork.feedForward(Functions.initArray(9, 0.0f));
		// setNeuralNetwork(newNetwork, "full");
		//iris();
	}
	
	void Update () {
		updateNeurons();
	}

	private void updateNeurons(){
		if(neurons.Count > 0){
			for(int i = 0; i < neurons[0].Count; i++){
				neurons[0][i].GetComponent<UnityNeuron>().updateValue(network.getNeuronValue(0,i));
			}
		}
		for(int i = 1; i < neurons.Count; i++){
			for(int j = 0; j < neurons[i].Count; j++){
				neurons[i][j].GetComponent<UnityNeuron>().updateValue(network.getNeuronValue(i,j), network.getNeuronWeights(i,j));
				neurons[i][j].GetComponent<UnityNeuron>().updateConnections(network.getNeuronWeights(i,j));
				neurons[i][j].GetComponent<UnityNeuron>().updateBias(network.getNeuronBias(i,j));
			}
		}
	}

	private void connectNeurons(GameObject neuron1, GameObject neuron2, float weight){
		neuron2.GetComponent<UnityNeuron>().connectNeuron(neuron1, weight);
	}

	public bool close(){
		return this.closeView;
	}

	public void closeMenu(){
		// foreach(List<GameObject> layer in neurons){
		// 	foreach(GameObject neuron in layer){
		// 		Destroy(neuron);
		// 	}
		// }

		// Destroy(cameraObject);

		this.closeView = true;
	}

	public void setNeuralNetwork(NeuralNetwork network, string cameraPosition){
		int numberOfLayers = network.getNumberofLayers();
		cameraObject = Instantiate(neuralNetworkViewCameraPrefab, new Vector3(1000f,1000f,-10f), Quaternion.identity);
		camera = cameraObject.GetComponent<Camera>();

		if(cameraPosition == "left"){
			camera.rect = new Rect(0.0f, 0.0f, 0.34f, 1.0f);
		}
		else{
			camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
		}

		cameraBounds = new Vector2(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2);

		this.network = network;
		neurons = new List<List<GameObject>>();

		int[] neuronDimensions = new int[numberOfLayers + 1];

		for(int i = 0; i <= numberOfLayers - 1; i++){
			neuronDimensions[i] = network.getLayerSize(i);
		}

		neuronDimensions[neuronDimensions.Length - 1] = numberOfLayers;
		int largestDimension = Mathf.Max(neuronDimensions);
		largestDimension = largestDimension * 2 + 2;

		neuronScale = new Vector3(Mathf.Min(cameraBounds.x, cameraBounds.y) / largestDimension, Mathf.Min(cameraBounds.x, cameraBounds.y) / largestDimension, Mathf.Min(cameraBounds.x, cameraBounds.y) / largestDimension);

		List<GameObject> layer = new List<GameObject>();
		GameObject tempNeuron;
		Vector3 tempNeuronLocation;

		//The origin is the bottom left corner of camera space
		Vector3 origin = new Vector3(camera.transform.position.x - (cameraBounds.x / 2), camera.transform.position.y - (cameraBounds.y / 2), 0.0f);
		//The spacing between network layers whether separated along the x or y axis;
		float layerSpacing = (cameraBounds.x > cameraBounds.y) ? cameraBounds.x / (1 + numberOfLayers) : cameraBounds.y / (1 + numberOfLayers);

		for(int i = 0; i < numberOfLayers; i++){

			int layerSize = (i == 0) ? network.getNumberofInputs() : network.getLayerSize(i);
			float neuronSpacing = (cameraBounds.y / (1 + layerSize));
			for(int j = 0; j < layerSize; j++){
				tempNeuronLocation = (cameraBounds.x >= cameraBounds.y) ?
				new Vector3(origin.x + (i + 1) * layerSpacing, origin.y + (j + 1) * neuronSpacing, 0.0f) :
				new Vector3(origin.x + (j + 1) * neuronSpacing, origin.y + (i + 1) * layerSpacing, 0.0f) ;

				tempNeuron = (GameObject) Instantiate(neuronPrefab, tempNeuronLocation, Quaternion.identity, gameObject.transform);
				tempNeuron.transform.localScale = neuronScale;
				tempNeuron.GetComponent<UnityNeuron>().setColor(network.getNeuronValue(i, j));
				layer.Add(tempNeuron);
			}

			neurons.Add(layer);
			layer = new List<GameObject>();
		}

		for(int i = 1; i < neurons.Count; i++){
			for(int j = 0; j < neurons[i].Count; j++){
				float[] weights = new float[0];
				try{
					weights = network.getNeuronWeights(i, j);
				}
				catch(System.ArgumentOutOfRangeException){
					throw new System.ArgumentException("NeuralNetworkView::setNetwork ~ network weight set indices out of range\nLayer: " + i.ToString() + ", Neuron: " + j.ToString());
				}
				for(int k = 0; k < neurons[i - 1].Count; k++){
					connectNeurons(neurons[i - 1][k], neurons[i][j], weights[k]);
				}
			}
		}
	}

	public void setNeuronNames(List<string> inputs, List<string> outputs){
		for(int i = 0; i < neurons[0].Count; i++){
			neurons[0][i].GetComponent<UnityNeuron>().name = inputs[i];
		}
		for(int i = 0; i < neurons[neurons.Count - 1].Count; i++){
			neurons[neurons.Count - 1][i].GetComponent<UnityNeuron>().name = outputs[i];
		}
	}

	public void setNeuronActivationFunctions(List<string> functions){
		for(int i = 0; i < neurons.Count; i++){
			for(int j = 0; j < neurons[i].Count; j++){
				neurons[i][j].GetComponent<UnityNeuron>().activationFunction = functions[i];
			}
		}
	}
}
