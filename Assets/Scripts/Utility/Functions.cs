using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class Functions {

	public static T[] initArray<T>(int length, T value){
		T[] array = new T[length];

		for(int i = 0; i < length; i++){
			array[i] = value;
		}

		return array;
	}
	

	public static int maxIndex<T>(T[] array) where T : System.IComparable{
		if(array.Length > 0){
			T max = array[0];
			int maxIndex = 0;

			for(int i = 0; i < array.Length; i++){
				if(array[i].CompareTo(max) > 0){
					max = array[i];
					maxIndex = i;
				} 
				
			}

			return maxIndex;
		}
		else{
			return -1;
		}
	}

	// public static int max(int[] array){
	// 	if(array.Length > 0){
	// 		int max = array[0];

	// 		for(int i = 0; i < array.Length; i++){
	// 			if(array[i].CompareTo(max) > 0){
	// 				max = array[i];
	// 			} 
				
	// 		}

	// 		return max;
	// 	}
	// 	else{
	// 		return 0;
	// 	}
	// }

	public static T max<T>(T[] array) where T : System.IComparable{
		if(array.Length > 0){
			T max = array[0];

			for(int i = 0; i < array.Length; i++){
				if(array[i].CompareTo(max) > 0){
					max = array[i];
				} 
				
			}

			return max;
		}
		else{
			throw new System.ArgumentException("Functions::max ~ argument array is empty");
		}
	}

	public static int minIndex<T>(T[] array) where T : System.IComparable{
		if(array.Length > 0){
			T min = array[0];
			int minIndex = 0;

			for(int i = 0; i < array.Length; i++){
				if(array[i].CompareTo(min) < 0){
					min = array[i];
					minIndex = i;
				} 
				
			}

			return minIndex;
		}
		else{
			return -1;
		}
	}

	public static T min<T>(T[] array) where T : System.IComparable{
		if(array.Length > 0){
			T min = array[0];

			for(int i = 0; i < array.Length; i++){
				if(array[i].CompareTo(min) < 0){
					min = array[i];
				} 
				
			}

			return min;
		}
		else{
			throw new System.ArgumentException("Functions::min ~ argument array is empty");
		}
	}

	public static float[] normalizeMinMax(float[] values){
		float[] newValues = new float[values.Length];
		float m = mean(values);
		float s = standardDeviation(values, m);

		for(int i = 0; i < values.Length; i++){
			newValues[i] = (values[i] - m) / s;
		}

		return newValues;
	}

	public static float mean(int[] values){
		return (values.Length > 0) ? ((float) sum(values)) / values.Length : 0.0f;
	}
	public static float mean(float[] values){
		return (values.Length > 0) ? sum(values) / values.Length : 0.0f;
	}

	public static float standardDeviation(float[] values, float mean){
		float s = 0.0f;

		s = foldl(((x,y) => x + Mathf.Pow(y - mean, 2.0f)), 0.0f, values);

		return (values.Length > 0) ? s / values.Length : 0.0f;		
	}

	public static List<T> shuffle<T>(List<T> set){
		int count = set.Count;
		for(int i = 0; i < count; i++){
			int index = Mathf.FloorToInt(Random.Range(0.0f, (float) count - 1));
			T value = set[i];

			set[i] = set[index];
			set[index] = value;
		}

		return set;
	}

	public static string print<T>(Matrix<T> matrix) where T : struct, System.IEquatable<T>, System.IFormattable{
		string temp = "";

		for(int i = 0; i < matrix.RowCount; i++){
			temp += print(matrix.Row(i)) + "\n";
		}

		return temp;
	}
	public static string print(System.Type type){
		string typeString = type.ToString();

		if(typeString == "System.Single"){
			typeString = "Float";
		}
		else if(typeString == "System.String"){
			typeString = "String";
		}
		else if(typeString == "System.Int32"){
			typeString = "Int";
		}
		else if(typeString == "System.Bool"){
			typeString = "Bool";
		}
		
		return typeString;
	}

	public static string print<T>(T printable) where T : IPrintable{
		return printable.print();
	}

	public static string print<T>(Vector<T> vector) where T : struct, System.IEquatable<T>, System.IFormattable{
		if(vector == default(Vector<T>)) return "Empty";
		List<T> list = new List<T>(vector.ToArray());
		return print(list);
	}

	public static string print<T>(T[][] arrays){
		string arrayDescription = "{";

		for(int i = 0; i < arrays.Length; i++){
			arrayDescription += print(arrays[i]) + ((i == arrays.Length - 1) ? "}" : ",\n");
		}

		return arrayDescription;
	}

	public static string print<T>(T[] array){
		string arrayDescription = "[";

		for(int i = 0; i < array.Length; i++){
			if(array[i] != null){
				if(i == array.Length - 1){
					arrayDescription += array[i].ToString();
				}
				else{
					arrayDescription += array[i].ToString() + ", ";
				}
			}
			else{
				arrayDescription += "Null item";
			}
		}

		return arrayDescription + "]";
	}

	public static string print<T>(List<T>[] array){
		string returnString = "{";
		for(int i = 0; i < array.Length; i++){
			if(i == array.Length - 1){
				returnString += print(array[i]) + "}";
			}
			else{
				returnString += print(array[i]) + "\n";
			}
		}

		return returnString;
	}

	public static string print<T>(List<T> list){
		return print(list.ToArray());
	}

	public static string printPrintable<T>(T printable) where T : IPrintable{
		return printable.print();
	}
	public static string printPrintable<T>(List<T> list) where T : IPrintable{
		string listDescription = "{";

		for(int i = 0; i < list.Count; i++){
			if(list[i] != null){
				if(i == list.Count - 1){
					listDescription += list[i].print() + "}";
				}
				else{
					listDescription += list[i].print() + ", ";
				}
			}
			else{
				listDescription += "Null item";
			}
		}

		return listDescription;
	}

	public static bool anyNull<T>(T[] nullableSet){
		foreach(T item in nullableSet){
			if(item == null) return true;
		}

		return false;
	}

	public static bool anyNaN(float[] array){
		foreach(float item in array){
			if(item == float.NaN){
				return true;
			}
		}

		return false;
	}

	public static TU[] zipWith<T,TU>(System.Func<T, T, TU> f, T[] array1, T[] array2) 
		where TU : struct, System.IEquatable<TU>, System.IFormattable
		where T : struct, System.IEquatable<TU>, System.IFormattable
	{
		if(array1.Length != array2.Length){
			throw new System.ArgumentException("Functions::zipWith ~ argument arrays of different sizes\nLength of array1: " + array1.Length.ToString() + ", Length of array2: " + array2.Length.ToString());
		}
		TU[] newArray = new TU[array1.Length];

		for(int i = 0; i < newArray.Length; i++){
			newArray[i] = f(array1[i], array2[i]);
		} 

		return newArray;
	}

	public static TU[] map<T,TU>(System.Func<T,TU> f, T[] array){
		TU[] newArray = new TU[array.Length];

		for(int i = 0; i < array.Length; i++){
			newArray[i] = f(array[i]);
		}

		return newArray;
	}

	public static bool all<T>(System.Func<T,bool> f, T[] array){
		for(int i = 0; i < array.Length; i++){
			if(!f(array[i])){
				return false;
			}
		}

		return true;
	}

	public static T[] filter<T>(System.Predicate<T> f, T[] array){
		List<T> newArray = new List<T>();

		for(int i = 0; i < array.Length; i++){
			if(f(array[i])){
				newArray.Add(array[i]);
			}
		}

		return newArray.ToArray();
	}

	public static TU foldl<T,TU>(System.Func<TU,T,TU> f, TU init, T[] array){
		TU foldValue = init;

		for(int i  = 0; i < array.Length; i++){
			foldValue = f(foldValue, array[i]);
		}

		return foldValue;
	} 

	public static int count<T>(T item, T[] array) where T : System.IEquatable<T>{
		int occurences = 0;

		for(int i = 0; i < array.Length; i++){
			if(item.Equals(array[i])){
				occurences++;
			}
		}

		return occurences;
	}

	//This really sucks
	public static int sum(int[] array){
		return foldl(((x,y) => x + y), 0, array);
	}

	public static float sum(float[] array){
		return foldl(((x,y) => x + y), 0.0f, array);
	}
}
