using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class Dataset : IPrintable {
	public List<Column> columns;
	public int numberOfColumns = 0;
	public int numberOfRecords = 0;
	public int targetAttribute = 0;
	public List<int> includeIndices;
	public List<float> singularValues;
	private List<Column> originalColumns;
	private Matrix<float> data;
	private List<int> trainingIndices;
	private List<int> testingIndices;

	public Dataset(){
		this.columns = new List<Column>();
		this.originalColumns = new List<Column>();
		this.includeIndices = new List<int>();
		this.trainingIndices = new List<int>();
		this.testingIndices = new List<int>();
		this.singularValues = null;
	}

	public void setIncludeIndices(List<int> includeIndices){
		this.includeIndices = includeIndices;
	}
	public void addColumn(Column column){
		if(columns.Count > 0){
			if(column.count() != columns[columns.Count - 1].count()){
				throw new System.Exception("MatrixDataset::addColumn ~ argument column record count mismatch\nExpected number of records: " + columns[columns.Count - 1].count().ToString() 
				+ ", Actual number of records: " + column.count().ToString());
			}
			else{
				this.columns.Add(column);
				this.originalColumns.Add(column);
				this.numberOfRecords = column.count();
				this.numberOfColumns = columns.Count;
				this.singularValues = null;
			}
		}
		else{
			this.columns.Add(column);
			this.originalColumns.Add(column);
			this.numberOfRecords = column.count();
			this.numberOfColumns = columns.Count;
			this.singularValues = null;
		}
	}

	public void includeColumn(int columnIndex){
		if(!this.includeIndices.Contains(columnIndex)){
			this.includeIndices.Add(columnIndex);
			this.includeIndices.Sort();
		}
	}

	public void removeColumn(int columnIndex){
		if(this.includeIndices.Contains(columnIndex)){
			this.includeIndices.Remove(columnIndex);
			this.includeIndices.Sort();
		}
	}

	public void convertColumn(int index, string conversionType){
		if(index < columns.Count()){
			if(conversionType == "MINMAX"){
				columns[index] = originalColumns[index].normalizeMinMax();
			}
			else if(conversionType == "ZSCORE"){
				columns[index] = originalColumns[index].normalizeZScore();
			}
			else if(conversionType == "ORIGINAL"){
				columns[index] = originalColumns[index];
			}
			else{
				throw new System.ArgumentException("Dataset::convertColumn ~ conversionType not a valid conversion\nconversionType: " + conversionType);
			}
		}
	}

	public void partitionDataset(float ratio){
		this.trainingIndices = new List<int>();
		this.testingIndices = new List<int>();
		int numberTrainingRecords = Mathf.FloorToInt(numberOfRecords * ratio);

		if(this.numberOfRecords > 0){
			List<int> recordIndices = new List<int>(Enumerable.Range(0,this.numberOfRecords));
			recordIndices = Functions.shuffle(recordIndices);

			for(int i = 0; i < numberOfRecords; i++){
				if(i < numberTrainingRecords){
					trainingIndices.Add(recordIndices[i]);
				}
				else{
					testingIndices.Add(recordIndices[i]);
				}
			}
		}
	}

	public string[] getAttributeNames(){
		string[] names = new string[columns.Count()];

		for(int i = 0; i < names.Length; i++){
			names[i] = columns[i].getValueSet().attributeName;
		}

		return names;
	}

	public List<string> getInputNames(){
		List<string> names = new List<string>();

		for(int i = 0; i < includeIndices.Count; i++){
			int index = includeIndices[i];

			if(index != targetAttribute){
				names.Add(columns[index].getValueSet().attributeName);
			}
		}

		return names;
	}

	public void shuffleTrainingRecords(){
		this.trainingIndices = Functions.shuffle(this.trainingIndices);
	}

	public int getNumberOfRecords(){
		return numberOfRecords;
	}
	public int numberOfTrainingRecords(){
		return this.trainingIndices.Count();
	}

	public int numberTestingRecords(){
		return this.testingIndices.Count();
	}

	public void setTargetAttribute(int target){
		if(columns[target].GetType() == typeof(FloatColumn)){
			columns[target] = columns[target].normalizeMinMax();
		}
		this.targetAttribute = target;
	}

	public void buildMatrix(){
		int matrixColumns = includeIndices.Count - 1;
		float[][] columnArrays = Functions.initArray(matrixColumns, new float[numberOfRecords]);

		int columnIndex = 0;

		for(int i = 0; i < columns.Count; i++){
			if(i != targetAttribute && includeIndices.Contains(i)){
				if(columns[i].GetType() == typeof(BoolColumn)){
					columns[i] = columns[i].normalizeMinMax();
				}
				else if(columns[i].GetType() == typeof(StringColumn)){
					columns[i] = columns[i].normalizeMinMax();
				}
				columnArrays[columnIndex] = ((FloatColumn) columns[i]).values.ToArray();
				columnIndex++;
			}
		}	

		this.data = Matrix<float>.Build.DenseOfColumnArrays(columnArrays);
		this.numberOfColumns = data.ColumnCount;
		this.singularValues = null;
	}

	public void reduceDimensions(int dimensions){
		if(dimensions < data.ColumnCount){
			MathNet.Numerics.LinearAlgebra.Factorization.Svd<float> svd = this.data.Svd();
			float[][] columnArrays = Functions.initArray(dimensions, new float[svd.U.RowCount]);
			
			this.singularValues = new List<float>();

			for(int i = 0; i < dimensions; i++){
				columnArrays[i] = svd.U.Column(i).ToArray();
				this.singularValues.Add(svd.S[i]);
			}

			this.data = Matrix<float>.Build.DenseOfColumnArrays(columnArrays);
			this.numberOfColumns = data.ColumnCount;
		}
	}

	public string[] getRecord(int index){
		string[] recordValues = new string[columns.Count];

		for(int i = 0; i < columns.Count; i++){
			if(columns[i].GetType() == typeof(FloatColumn)){
				recordValues[i] = ((FloatColumn) columns[i]).values[index].ToString();
			}
			else if(columns[i].GetType() == typeof(BoolColumn)){
				recordValues[i] = (((BoolColumn) columns[i]).values[index]) ? "True" : "False";
			}
			else if(columns[i].GetType() == typeof(StringColumn)){
				recordValues[i] = ((StringColumn) columns[i]).values[index];
			}
			else{
				throw new System.Exception("Dataset::getStringRecord ~ Column of non-Column type\nColumn " + i.ToString() + "(" + columns[i].GetType().ToString() + ")");
			}
		}

		//Debug.Log("Record " + index.ToString() + ": " + Functions.print(recordValues));
		return recordValues;
	}

	public string[] getIncludedRecord(int index){
		string[] recordValues = new string[includeIndices.Count];

		for(int i = 0; i < includeIndices.Count; i++){
			if(columns[includeIndices[i]].GetType() == typeof(FloatColumn)){
				recordValues[i] = ((FloatColumn) columns[includeIndices[i]]).values[index].ToString();
			}
			else if(columns[includeIndices[i]].GetType() == typeof(BoolColumn)){
				recordValues[i] = (((BoolColumn) columns[includeIndices[i]]).values[index]) ? "True" : "False";
			}
			else if(columns[includeIndices[i]].GetType() == typeof(StringColumn)){
				recordValues[i] = ((StringColumn) columns[includeIndices[i]]).values[index];
			}
			else{
				throw new System.Exception("Dataset::getStringRecord ~ Column of non-Column type\nColumn " + i.ToString() + "(" + columns[includeIndices[i]].GetType().ToString() + ")");
			}
		}

		return recordValues;
	}

	public float[] getFloatRecord(int index){
		return data.Row(index).ToArray();
	}

	public int getTrainingIndex(int index){
		return trainingIndices[index];
	}

	public int getTestingIndex(int index){
		return testingIndices[index];
	}

	public System.Type getTargetColumnType(){
		return this.columns[targetAttribute].GetType();
	}

	public string getTargetStringValue(int index){
		return ((StringColumn) columns[targetAttribute]).values[index];
	}

	public string[] getTargetStringClasses(){
		if(columns[targetAttribute].GetType() == typeof(StringColumn)){
			return ((StringValueSet) ((StringColumn) columns[targetAttribute]).getValueSet()).values.ToArray();
		}
		else if(columns[targetAttribute].GetType() == typeof(BoolColumn)){
			return new string[]{ "True", "False" };
		}
		else{
			return new string[]{ columns[targetAttribute].attributeName };
		}
	}

	public bool getTargetBoolValue(int index){
		return ((BoolColumn) columns[targetAttribute]).values[index];
	}

	public float getTargetFloatValue(int index){
		return ((FloatColumn) columns[targetAttribute]).values[index];
	}

	public string print(){
		string returnString = "Dataset\n===========\n\n";

		for(int i = 0; i < this.numberOfRecords; i++){
			returnString += "Record " + i.ToString() + ": " + Functions.print(getRecord(i)) + "\n";
		}

		return returnString;
	}
}

public abstract class Column : IPrintable{

	public string attributeName;

	protected ValueSet valueSet;

	public abstract void addValue(object value);

	public abstract ValueSet getValueSet();

	public abstract FloatColumn normalizeMinMax();
	public abstract FloatColumn normalizeZScore();

	public virtual int count(){return -1;}
	public virtual string print(){
		return "Column ~ " + attributeName + "(" + this.GetType().ToString() + ")\nNumber of records: " + this.count().ToString() + "\n";
	}
}

public class StringColumn : Column {
	public List<string> values;

	public StringColumn(string attributeName, string[] values){
		this.attributeName = attributeName;
		this.valueSet = new StringValueSet(attributeName);
		this.values = new List<string>();
		foreach(string value in values){
			addValue(value);
		}
	}

	public StringColumn(string attributeName){
		this.attributeName = attributeName;
		this.valueSet = new StringValueSet(attributeName);
		this.values = new List<string>();
	}
	public override void addValue(object value){
		valueSet.addValue(value);
		values.Add((string) value);
	}

	public override ValueSet getValueSet(){
		return this.valueSet;
	}

	public override FloatColumn normalizeMinMax(){
		float[] newValues = new float[values.Count];
		List<string> columnValues = ((StringValueSet) valueSet).values;
		float max = columnValues.Count - 1;
		float mid = max / 2.0f;
		max = max - mid;

		newValues = Functions.map((x => (float) columnValues.IndexOf(x)), values.ToArray());
		if(max != 0.0f){
			newValues = Functions.map((x => (x - mid) / max), newValues);	
		}
		else{
			newValues = Functions.initArray(newValues.Length, 0.0f);
		}

		FloatValueSet normalizedValueSet = new FloatValueSet(attributeName);
		normalizedValueSet.max = 1.0f;
		normalizedValueSet.mid = 0.0f;
		normalizedValueSet.min = -1.0f;
		normalizedValueSet.range = 2.0f;
		return new FloatColumn(attributeName, newValues, normalizedValueSet);
	}

	public  FloatColumn normalizeFrequencyMinMax(){
		float[] newValues = new float[values.Count];
		float[] frequencies = Functions.map((x => (float) x),  ((StringValueSet) valueSet).valueOccurences.ToArray());
		float max = (float) Functions.max(frequencies);
		float min = (float) Functions.min(frequencies);
		float mid = (max + min) / 2.0f;

		max = max - mid;
		
		if(max != 0.0f){
 			frequencies = Functions.map((x => (x - mid) / max), frequencies);
		}
		else{
			frequencies = Functions.initArray(frequencies.Length, 0.0f);
		}

		newValues = Functions.map((x => frequencies[((StringValueSet) valueSet).values.IndexOf(x)]), values.ToArray());

		FloatValueSet normalizedValueSet = new FloatValueSet(attributeName);
		normalizedValueSet.max = 1.0f;
		normalizedValueSet.mid = 0.0f;
		normalizedValueSet.min = -1.0f;
		normalizedValueSet.range = 2.0f;
		return new FloatColumn(attributeName, newValues, normalizedValueSet);
	}

	public override FloatColumn normalizeZScore(){
		List<string> columnValues = ((StringValueSet) valueSet).values;
		float[] frequencies = Functions.map((x => (float) x), (((StringValueSet) valueSet).valueOccurences.ToArray()));
		float mean = Functions.mean(frequencies);
		float standardDeviation = Functions.standardDeviation(frequencies, mean);
		float[] zscores = Functions.map((x => (x - mean) / standardDeviation), frequencies);

		FloatValueSet normalizedValueSet = new FloatValueSet(attributeName);
		normalizedValueSet.max = Functions.max(zscores);
		normalizedValueSet.min = Functions.min(zscores);
		normalizedValueSet.mid = (normalizedValueSet.max + normalizedValueSet.min) / 2.0f;
		normalizedValueSet.range = normalizedValueSet.max - normalizedValueSet.min;
		normalizedValueSet.mean = mean;
		normalizedValueSet.standardDeviation = standardDeviation;
		return new FloatColumn(attributeName, Functions.map((x => zscores[columnValues.IndexOf(x)]), values.ToArray()), normalizedValueSet);
	}

	public override int count(){
		return this.values.Count();
	}

	public override string print(){
		string returnString = "StringColumn ~ " + attributeName + "\nNumber of records: " + count() + "\nValues\n====\n";

		for(int i = 0; i < values.Count; i++){
			returnString += "record " + i.ToString() + ": " + values[i] + "\n";
		}

		return returnString;
	}
}

public class BoolColumn : Column {
	public List<bool> values;

	public BoolColumn(string attributeName){
		this.attributeName = attributeName;
		this.values = new List<bool>();
		this.valueSet = new BoolValueSet(attributeName);
	}

	public BoolColumn(string attributeName, bool[] values){
		this.attributeName = attributeName;
		this.values = new List<bool>();
		this.valueSet = new BoolValueSet(attributeName);
		foreach(bool value in values){
			this.addValue(value);
		}
	}

	public override void addValue(object value){
		this.values.Add((bool) value);
		((BoolValueSet) this.valueSet).addValue(value);
	}

	public override ValueSet getValueSet(){
		return this.valueSet;
	}

	public override FloatColumn normalizeMinMax(){
		float[] newValues = Functions.map((x => x ? 1.0f : -1.0f), values.ToArray());
		FloatValueSet normalizedValueSet = new FloatValueSet(attributeName);

		normalizedValueSet.max = 1.0f;
		normalizedValueSet.mid = 0.0f;
		normalizedValueSet.min = -1.0f;
		normalizedValueSet.range = 2.0f;
		normalizedValueSet.mean = Functions.mean(newValues);
		normalizedValueSet.standardDeviation = Functions.standardDeviation(newValues, normalizedValueSet.mean);
	
		return new FloatColumn(attributeName, newValues, normalizedValueSet);
	}

	public override FloatColumn normalizeZScore(){
		float[] newValues = Functions.map((x => x ? 1.0f : -1.0f), values.ToArray());
		FloatValueSet normalizedValueSet = new FloatValueSet(attributeName);

		normalizedValueSet.mean = Functions.mean(newValues);
		normalizedValueSet.standardDeviation = Functions.standardDeviation(newValues, normalizedValueSet.mean);
		float trueZscore = (1.0f - normalizedValueSet.mean) / normalizedValueSet.standardDeviation;
		float falseZscore = (-1.0f - normalizedValueSet.mean) / normalizedValueSet.standardDeviation;

		newValues = Functions.map((x => (x) ? trueZscore : falseZscore), values.ToArray());
		return new FloatColumn(attributeName, newValues, normalizedValueSet);
	}

	public override int count(){
		return this.values.Count();
	}
	
	public override string print(){
		string returnString = "BoolColumn ~ " + attributeName + "\nNumber of records: " + count() + "\nValues\n====\n";

		for(int i = 0; i < values.Count; i++){
			returnString += "record " + i.ToString() + ": " + ((values[i]) ? "True" : "False") + "\n";
		}

		return returnString;
	}
}

public class FloatColumn : Column {
	public Vector<float> values;

	public FloatColumn(string attributeName, float[] values, FloatValueSet valueSet){
		this.attributeName = attributeName;
		this.valueSet = valueSet;
		this.values = Vector<float>.Build.DenseOfArray(values);
	}
	public FloatColumn(string attributeName, float[] values){
		this.attributeName = attributeName;
		FloatValueSet arrayValueSet = new FloatValueSet(attributeName);
		arrayValueSet.max = Functions.max(values);
		arrayValueSet.min = Functions.min(values);
		arrayValueSet.mid = (arrayValueSet.max + arrayValueSet.min) / 2.0f;
		arrayValueSet.range = arrayValueSet.max - arrayValueSet.min;
		arrayValueSet.mean = Functions.mean(values);
		arrayValueSet.standardDeviation = Functions.standardDeviation(values, arrayValueSet.mean);
		this.valueSet = arrayValueSet;
		this.values = Vector<float>.Build.DenseOfArray(values);
	}
	public FloatColumn(string attributeName){
		this.attributeName = attributeName;
		this.valueSet = new FloatValueSet(attributeName);
		this.values = Vector<float>.Build.DenseOfArray(new float[0]);
	}
	public override void addValue(object value){
		this.values.Add((float) value);
	}

	public override ValueSet getValueSet(){
		return this.valueSet;
	}
	public override FloatColumn normalizeMinMax(){
		FloatValueSet normalizedValueSet = new FloatValueSet(attributeName);
		float[] newValues = values.ToArray();
		float max = Functions.max(newValues);
		float min = Functions.min(newValues);
		float mid = (max + min) / 2.0f;
		max = max - mid;

		newValues = Functions.map((x => (x - mid) / max), newValues);

		normalizedValueSet.max = 1.0f;
		normalizedValueSet.min = -1.0f;
		normalizedValueSet.mid = 0.0f;
		normalizedValueSet.range = 2.0f;
		normalizedValueSet.mean = Functions.mean(newValues);
		normalizedValueSet.standardDeviation = Functions.standardDeviation(newValues, normalizedValueSet.mean);
		return new FloatColumn(attributeName, newValues, normalizedValueSet);
	}
	public override FloatColumn normalizeZScore(){
		FloatValueSet normalizedValueSet = new FloatValueSet(attributeName);
		normalizedValueSet.mean = Functions.mean(values.ToArray());
		normalizedValueSet.standardDeviation = Functions.standardDeviation(values.ToArray(), normalizedValueSet.mean);

		float[] newValues = Functions.map((x => (x - normalizedValueSet.mean) / normalizedValueSet.standardDeviation), values.ToArray());

		normalizedValueSet.max = Functions.max(newValues);
		normalizedValueSet.min = Functions.min(newValues);
		normalizedValueSet.mid = (normalizedValueSet.max + normalizedValueSet.min) / 2.0f;
		normalizedValueSet.range = normalizedValueSet.max - normalizedValueSet.min;

		return new FloatColumn(attributeName, newValues, normalizedValueSet);
	}

	public override int count(){
		return this.values.Count();
	}

	public override string print(){
		string returnString = "FloatColumn ~ " + attributeName + "\nNumber of records: " + count() + "\nValues\n====\n";

		for(int i = 0; i < values.Count; i++){
			returnString += "record " + i.ToString() + ": " + values[i].ToString() + "\n";
		}

		return returnString;
	}
}

public abstract class ValueSet : IPrintable{
	public string attributeName;
	public virtual void addValue(object value){}
	public abstract string print();
}

public class StringValueSet : ValueSet {
	public List<string> values;
	public List<int> valueOccurences;
	public float baseRate = 0.0f;

	public StringValueSet(string attributeName){
		this.attributeName = attributeName;
		this.values = new List<string>();
		this.valueOccurences = new List<int>();
	}

	public override void addValue(object value){
		if(!values.Contains((string) value)){
			values.Add((string) value);
			valueOccurences.Add(1);
		}
		else{
			valueOccurences[values.IndexOf((string) value)] += 1;
		}
	}

	public void calculateBaseRate(){
		int[] occurences = valueOccurences.ToArray();
		this.baseRate = ((float) Functions.max(occurences)) / Functions.sum(occurences);
	}

	public string getMostCommon(){
		int[] occurences = valueOccurences.ToArray();
		return values[Functions.maxIndex(occurences)];
	}

	public override string print(){
		return "StringValueSet ~ " + attributeName + "\nValues: " + Functions.print(values);
	}
}

public class FloatValueSet : ValueSet {
	public float max = 0.0f;
	public float mid = 0.0f;
	public float min = 0.0f;
	public float range = 0.0f;
	public float mean = 0.0f;
	public float standardDeviation = 0.0f;

	public FloatValueSet(string attributeName){
		this.attributeName = attributeName;
	}	
	public override string print(){
		return "FloatValueSet ~ " + attributeName + "\n" + 
		"Max: " + max.ToString() + "\n" +
		"Mid: " + mid.ToString() + "\n" +
		"Min: " + min.ToString() + "\n" +
		"Range: " + range.ToString() + "\n" +
		"Mean: " + mean.ToString() + "\n" +
		"Standard Deviation: " + standardDeviation.ToString();
	}
}

public class BoolValueSet : ValueSet {
	public int trues = 0;
	public int falses = 0;
	public float baseRate = 0.0f;

	public BoolValueSet(string attributeName){
		this.attributeName = attributeName;
	}
	public override void addValue(object value){
		if((bool) value){
			trues++;
		}
		else{
			falses++;
		}
	}
	public void calculateBaseRate(){
		this.baseRate = Mathf.Max((float) trues, (float) falses) / (trues + falses);
	}

	public bool getMostCommon(){
		return (trues >= falses);
	}

	public override string print(){
		return "BoolValueSet ~ " + attributeName + "\nTrues: " + trues.ToString() + "\nFalses: " + falses.ToString();
	}
}