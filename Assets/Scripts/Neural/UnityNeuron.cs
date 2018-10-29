using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityNeuron : MonoBehaviour {

	public Material lineMaterial;
	public GameObject neuronConnectionPrefab;
	private MaterialPropertyBlock block;

	public List<UnityNeuronConnection> connections;

	public List<float> weights;

	private float lineWidth = 0.1f;

	public float neuronValue = 0.0f;

	public float neuronBias = 0.0f;

	private bool isCognitiveNeuron = false;

	public string name = "anon";

	public string activationFunction = "none";

	// Use this for initialization
	void Awake () {
		block = new MaterialPropertyBlock();
		connections = new List<UnityNeuronConnection>();
		weights = new List<float>();
	}

	public void setColor(float value){
		gameObject.GetComponent<Renderer>().GetPropertyBlock(block);
		block.SetColor("_Color", neuronValueToColor(value));
		gameObject.GetComponent<Renderer>().SetPropertyBlock(block);
	}

	private Color neuronValueToColor(float value){
		return new Color((value > 0.0f) ? value : 0f, 0f, (value < 0.0f) ? -value : 0f, Mathf.Abs(value));
	}

	public void connectNeuron(GameObject neuron, float weight){
		GameObject tempObject = Instantiate(neuronConnectionPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
		LineRenderer tempRenderer = tempObject.AddComponent<LineRenderer>();
		tempRenderer.SetPosition(0, gameObject.transform.position);
		tempRenderer.SetPosition(1, neuron.transform.position);
		tempRenderer.material = lineMaterial;
		tempRenderer.SetWidth(lineWidth, lineWidth);
		tempRenderer.SetColors(neuronValueToColor(weight), neuronValueToColor(weight));
		UnityNeuronConnection tempConnection = new UnityNeuronConnection(neuron, tempRenderer, weight);
		connections.Add(tempConnection);
		weights.Add(weight);
		isCognitiveNeuron = true;
	}

	public void updateValue(float value){
		neuronValue = value;
		setColor(value);
	}

	public void updateValue(float value, float[] weights){
		neuronValue = value;
		this.weights = new List<float>(weights);
		setColor(value);
	}

	public void updateConnections(float[] weights){
		// if(isCognitiveNeuron){
		// 	for(int i = 0; i < ((CognitiveNeuron) neuron).neurons.Count; i++){
		// 		connections[i].weight = ((CognitiveNeuron) neuron).neurons[i].weight;
		// 		connections[i].setColor(neuronValueToColor(connections[i].weight));
		// 	}
		// }
		for(int i = 0; i < connections.Count; i++){
			connections[i].weight = weights[i];
			connections[i].setColor(neuronValueToColor(weights[i]));
		}
	}

	public void updateBias(float value){
		neuronBias = value;
	}
}

public class UnityNeuronConnection {

	public GameObject neuron;

	public LineRenderer lineRenderer;
	public float weight;
	public UnityNeuronConnection(GameObject neuron, LineRenderer lineRenderer, float weight){
		this.neuron = neuron;
		this.lineRenderer = lineRenderer;
		this.weight = weight;
	}

	public void setColor(Color color){
		this.lineRenderer.SetColors(color, color);
	}
}
