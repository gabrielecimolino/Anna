using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class Testbench {

	public NeuralNetwork network;
	public Dataset dataset;
	private int trainingIndex = 0;
	private int numberOfInputs = 0;
	private List<int> hiddenLayerSizes;
	private int numberOfOutputs = 0;

	public Testbench(NeuralNetwork network, Dataset dataset){
		this.network = network;
		this.dataset = dataset;
	}

	public Testbench(Dataset dataset, List<int> hiddenLayerSizes = null, float learningRate = 0.1f){
		this.numberOfInputs = dataset.numberOfColumns;
		this.numberOfOutputs = (dataset.columns[dataset.targetAttribute].GetType() == typeof(StringColumn)) ? dataset.getTargetStringClasses().Length : ((dataset.columns[dataset.targetAttribute].GetType() == typeof(BoolColumn)) ? 2 : 1);
		
		if(hiddenLayerSizes == null){
			this.hiddenLayerSizes = new List<int>(){Mathf.Max(Mathf.RoundToInt(Mathf.Sqrt(dataset.includeIndices.Count)), (int)Mathf.Round(Mathf.Sqrt(numberOfOutputs)))};
		}
		else{
			this.hiddenLayerSizes = hiddenLayerSizes;
		}
		this.dataset = dataset;

		this.network = new NeuralNetwork(numberOfInputs, this.hiddenLayerSizes, numberOfOutputs, dataset.numberOfRecords / 10);

		network.setLearningRate(learningRate);
		network.randomizeWeights();
		network.randomizeBiases();
	}

	public void setLearningRate(float learningRate){
		network.setLearningRate(learningRate);
	}

	public ConfusionMatrix train(int iterations = 1){
		int truePositives = 0;
		int incorrect = 0;
		float averageProblemDifficulty = 0.0f;
		Matrix<float> confusion = Matrix<float>.Build.Dense(numberOfOutputs, numberOfOutputs, 0.0f);

		int numberOfTrainingRecords = dataset.numberOfTrainingRecords();
		network.ruminate();

		for(int i = 0; i < iterations; i++){
			dataset.shuffleTrainingRecords();
			if(dataset.columns[dataset.targetAttribute].GetType() == typeof(StringColumn)){
				int targetIndex;
				for(int index = 0; index < numberOfTrainingRecords; index++){
				 	targetIndex = dataset.getTrainingIndex(index);
					TestResult result = trainRecord(dataset.getFloatRecord(targetIndex), desiredOutput(dataset.getTargetStringValue(targetIndex)));
					confusion[result.answer, result.choice] += 1.0f;
					averageProblemDifficulty += result.problemDifficulty;

					if(result.correct){
						truePositives++;	
					}
					else{
						incorrect++;
					}
				}
			}
			else if(dataset.columns[dataset.targetAttribute].GetType() == typeof(FloatColumn)){
				int targetIndex;
				for(int index = 0; index < numberOfTrainingRecords; index++){
					targetIndex = dataset.getTrainingIndex(index);
					TestResult result = trainRecord(dataset.getFloatRecord(targetIndex), desiredOutput(dataset.getTargetFloatValue(targetIndex)));
					averageProblemDifficulty += result.problemDifficulty;
				}
			}
			else if(dataset.columns[dataset.targetAttribute].GetType() == typeof(BoolColumn)){
				int targetIndex;
				for(int index = 0; index < numberOfTrainingRecords; index++){
					targetIndex = dataset.getTrainingIndex(index);
					TestResult result = trainRecord(dataset.getFloatRecord(targetIndex), desiredOutput(dataset.getTargetBoolValue(targetIndex)));
					confusion[result.answer, result.choice] += 1.0f;
					averageProblemDifficulty += result.problemDifficulty;

					if(result.correct){
						truePositives++;	
					}
					else{
						incorrect++;
					}
				}
			} 
		}

		if(dataset.columns[dataset.targetAttribute].GetType() == typeof(StringColumn)){	
			averageProblemDifficulty = (incorrect == 0) ? 0.0f : averageProblemDifficulty / incorrect;
			return new ConfusionMatrix(confusion, ((float) truePositives) / (truePositives + incorrect), averageProblemDifficulty);	
		}
		else if(dataset.columns[dataset.targetAttribute].GetType() == typeof(BoolColumn)){	
			averageProblemDifficulty = (incorrect == 0) ? 0.0f : averageProblemDifficulty / incorrect;
			return new ConfusionMatrix(confusion, ((float) truePositives) / (truePositives + incorrect), averageProblemDifficulty);	
		}
		else{
			averageProblemDifficulty = averageProblemDifficulty / (numberOfTrainingRecords * iterations);
			return new ConfusionMatrix(confusion, 1.0f - averageProblemDifficulty, averageProblemDifficulty);		
		}
	}

	public void nextTrain(){
		int numberOfTrainingRecords = dataset.numberOfTrainingRecords();
		if(trainingIndex >= numberOfTrainingRecords){
			trainingIndex = 0;
			dataset.shuffleTrainingRecords();
		}
		else{
			int index = dataset.getTrainingIndex(trainingIndex);
			if(dataset.columns[dataset.targetAttribute].GetType() == typeof(StringColumn)){
				trainRecord(dataset.getFloatRecord(index), desiredOutput(((StringColumn) dataset.columns[dataset.targetAttribute]).values[index]));
			}
			else if(dataset.columns[dataset.targetAttribute].GetType() == typeof(FloatColumn)){
				trainRecord(dataset.getFloatRecord(index), desiredOutput(((FloatColumn) dataset.columns[dataset.targetAttribute]).values[index]));
			}
			else if(dataset.columns[dataset.targetAttribute].GetType() == typeof(BoolColumn)){
				trainRecord(dataset.getFloatRecord(index), desiredOutput(((BoolColumn) dataset.columns[dataset.targetAttribute]).values[index]));
			}
			trainingIndex++;
		}
	}

	private TestResult trainRecord(float[] inputs, float[] desiredOutputs){

		network.train(inputs, desiredOutputs);
		int favored = network.favoredNeuron();
		int answer = Functions.maxIndex(desiredOutputs);
		float[] outputs = network.getOutputs();
		if(favored != answer && outputs.Length > 1){
			((NeuralNetwork)network).addMemory(inputs, desiredOutputs);
		}
		if(desiredOutputs.Length > 1){
			return new TestResult(favored == answer, favored, answer, problemDifficulty(outputs[favored], outputs[answer]));
		}
		else{
            try{
                float columnMax = 1.0f;
                float columnMid = 1.0f;
                if(dataset.columns[dataset.targetAttribute].GetType() == typeof(FloatColumn)){
                    columnMax = ((FloatValueSet) ((FloatColumn) dataset.columns[dataset.targetAttribute]).getValueSet()).max;
                    columnMid = ((FloatValueSet) ((FloatColumn) dataset.columns[dataset.targetAttribute]).getValueSet()).mid;
                    columnMax -= columnMid;
                }
			    return new TestResult(favored == answer, favored, answer, Mathf.Abs(desiredOutputs[0] - outputs[0]) / 2.0f);
            }
            catch(System.IndexOutOfRangeException){
                throw new System.IndexOutOfRangeException("Testbench::trainRecord ~ either index of desired outputs or network outputs is out of range\ndesiredOutputs: " + 
                Functions.print(desiredOutputs) + "[0], outputs: " + Functions.print(outputs) + "[" + favored.ToString() + "]");
            }
		}
	}

	public ConfusionMatrix test(){
		int truePositives = 0;
		int incorrect = 0;
		float averageProblemDifficulty = 0.0f;
		Matrix<float> confusion = Matrix<float>.Build.Dense(numberOfOutputs, numberOfOutputs, 0.0f);

		int numberOfTestingRecords = dataset.numberTestingRecords();

		if(dataset.columns[dataset.targetAttribute].GetType() == typeof(StringColumn)){
			for(int index = 0; index < numberOfTestingRecords; index++){
				int testingIndex = dataset.getTestingIndex(index);
				TestResult result = testRecord(dataset.getFloatRecord(testingIndex), desiredOutput(((StringColumn) dataset.columns[dataset.targetAttribute]).values[testingIndex]));
				confusion[result.answer, result.choice] += 1.0f;
				averageProblemDifficulty += result.problemDifficulty;
				if(result.correct){
					truePositives++;	
				}
				else{
					incorrect++;
				}
			}
		}
		else if(dataset.columns[dataset.targetAttribute].GetType() == typeof(FloatColumn)){
			incorrect = 1;
			for(int index = 0; index < numberOfTestingRecords; index++){
				int testingIndex = dataset.getTestingIndex(index);
				TestResult result = testRecord(dataset.getFloatRecord(testingIndex), desiredOutput(((FloatColumn) dataset.columns[dataset.targetAttribute]).values[testingIndex]));
				averageProblemDifficulty += result.problemDifficulty;
			}
			averageProblemDifficulty = averageProblemDifficulty / numberOfTestingRecords;
		} 
		else if(dataset.columns[dataset.targetAttribute].GetType() == typeof(BoolColumn)){
			int targetIndex;
			for(int index = 0; index < numberOfTestingRecords; index++){
				targetIndex = dataset.getTestingIndex(index);
				TestResult result = testRecord(dataset.getFloatRecord(targetIndex), desiredOutput(dataset.getTargetBoolValue(targetIndex)));
				confusion[result.answer, result.choice] += 1.0f;
				averageProblemDifficulty += result.problemDifficulty;

				if(result.correct){
					truePositives++;	
				}
				else{
					incorrect++;
				}
			}
		} 
		if(dataset.columns[dataset.targetAttribute].GetType() == typeof(StringColumn)){	
			averageProblemDifficulty = (incorrect == 0) ? 0.0f : averageProblemDifficulty / incorrect;
			return new ConfusionMatrix(confusion, ((float) truePositives) / numberOfTestingRecords, averageProblemDifficulty);	
		}
		else if(dataset.columns[dataset.targetAttribute].GetType() == typeof(BoolColumn)){	
			averageProblemDifficulty = (incorrect == 0) ? 0.0f : averageProblemDifficulty / incorrect;
			return new ConfusionMatrix(confusion, ((float) truePositives) / (truePositives + incorrect), averageProblemDifficulty);	
		}
		else{
			return new ConfusionMatrix(confusion, 1.0f - averageProblemDifficulty, averageProblemDifficulty);		
		}
	}

	private TestResult testRecord(float[] inputs, float[] desiredOutputs){
		int favored = 0;

		network.feedForward(inputs);
		favored = network.favoredNeuron();
		float[] outputs = network.getOutputs();
		int answer = Functions.maxIndex(desiredOutputs);
		if(desiredOutputs.Length > 1){
			return new TestResult(favored == answer, favored, answer, problemDifficulty(outputs[favored], outputs[answer]));
		}
		else{
			return new TestResult(favored == answer, favored, answer, Mathf.Abs(desiredOutputs[0] - outputs[favored]));
		}
	}

	private float[] desiredOutput(string target){
        float[] desiredOutputs = new float[numberOfOutputs];
        string[] classes = dataset.getTargetStringClasses();
		
		if(dataset.columns[dataset.targetAttribute].GetType() == typeof(StringColumn)){
            desiredOutputs = Functions.map((x => (x == target) ? 1.0f : -1.0f), classes);
		}


		return desiredOutputs;
	}

	private float[] desiredOutput(float target){
		return new float[]{ target };
	}

	private float[] desiredOutput(bool target){
		return new float[]{ (target ? 1.0f : -1.0f), (!target ? 1.0f : -1.0f) };
	}
	private float problemDifficulty(float favored, float target){
		return (favored == target) ? 0.0f : 1.0f / (favored - target);
	}
}

public class ConfusionMatrix{

	public Matrix<float> confusionMatrix;

	public float predictionAccuracy;

	public float averageProblemDifficulty;

	public ConfusionMatrix(Matrix<float> confusionMatrix, float predictionAccuracy, float averageProblemDifficulty){
		this.confusionMatrix = confusionMatrix;
		this.predictionAccuracy = predictionAccuracy;
		this.averageProblemDifficulty = averageProblemDifficulty;
	}
}

public class TestResult{

	public bool correct;

	public int choice;

	public int answer;

	public float problemDifficulty;

	public TestResult(bool correct = false, int choice = 0, int answer = 0, float problemDifficulty = 0.0f){
		this.correct = correct;
		this.choice = choice;
		this.answer = answer;
		this.problemDifficulty = problemDifficulty;
	}
}