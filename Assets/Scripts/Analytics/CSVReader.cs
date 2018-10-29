using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class CSVReader{

	private string path;
	private StreamReader reader;

	public CSVReader(string path){
		this.path = path;
	}

	public Dataset getDataset(){
		return getDataset(new List<int>(){});
	}

	public Dataset getDataset(List<int> ignoreIndices){
		reader = new StreamReader(path);
		List<string> attributeNames = getAttributes(reader.ReadLine());
		List<List<string>> stringColumns = new List<List<string>>();
		Dataset dataset = new Dataset();

		foreach(string name in attributeNames){
			stringColumns.Add(new List<string>());
		}

		List<int> includeIndices = new List<int>();

		for(int i = 0; i < attributeNames.Count; i++){
			if(!ignoreIndices.Contains(i)) includeIndices.Add(i);
		}

		string currentLine = reader.ReadLine();
		
		while(currentLine != null){
			List<string> attributes = getAttributes(currentLine);
			for(int i = 0; i < stringColumns.Count; i++){
				try{
					stringColumns[i].Add(attributes[i]);
				}
				catch(System.ArgumentOutOfRangeException){
					throw new System.ArgumentOutOfRangeException("CSVReader::getDataset ~ Array index out of range\nIndex: " + i.ToString());
				}
			}
			currentLine = reader.ReadLine();
		}

		for(int i = 0; i < stringColumns.Count(); i++){
			string[] stringColumn = stringColumns[i].ToArray();
			if(Functions.all((x => isBool(x)), stringColumn)){
				dataset.addColumn(new BoolColumn(attributeNames[i], Functions.map((x => isTrue(x)), stringColumn)));
			}
			else if(Functions.all((x => isNumeric(x)), stringColumn)){
				dataset.addColumn(new FloatColumn(attributeNames[i], Functions.map((x => (x == "") ? 0.0f : float.Parse(x)), stringColumn)));
			}
			else{
				dataset.addColumn(new StringColumn(attributeNames[i], stringColumn));
			}
		}

		dataset.setIncludeIndices(includeIndices);

		return dataset;
	}

	private List<string> getAttributes(string line){
		List<string> attributeNames = new List<string>();
		bool stringContext = false;
		string name = "";

		foreach(char character in (line + "\0")){
			if(character == '\0'){
				attributeNames.Add(name);
				name = "";
			}
			else if(character == '"'){
				stringContext = !stringContext;
			}
			else if(character == ',' && !stringContext){
				attributeNames.Add(name);
				name = "";
			}
			else{
				name += character;
			}
		}

		return attributeNames;
	}

	private bool isNumeric(string value){
		return (isFloat(value) || isInt(value) || value == "");
	}

	private bool isFloat(string value){
		//STOP DOTS!
		bool hasDot = false;

		foreach(char character in value){
			if(character == '.' && !hasDot){
				hasDot = true;
			}
			else if(character == '.' && hasDot){
				return false;
			}
			else if(!isNumber(character)){
				return false;
			}
		}

		return (value != "" && hasDot);
	}

	private bool isInt(string value){
		foreach(char character in value){
			if(!isNumber(character) && character != '-'){
				return false;
			}
		}

		return (value != "" && !value.Contains(".") && value != "-");
	}

	private bool isNumber(char character){
		List<char> numbers = new List<char>(){'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

		return numbers.Contains(character);
	}

	private bool isBool(string value){
		return isTrue(value) || isFalse(value) || value == "" || value == "?";
	}

	private bool isTrue(string value){
		return (value == "true" || value == "True" || value == "1" || value == "y" || value == "Y");
	}

	private bool isFalse(string value){
		return (value == "false" || value == "False" || value == "0" || value == "n" || value == "N");
	}
}