using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics;

public class Anna : MonoBehaviour {

	public GameObject uiTestbenchPrefab;

	public GameObject datasetViewPrefab;

	public GameObject viewColumnPrefab;
	public GameObject attributeValuePrefab;


	public GUIStyle style;

	float[][] array = Functions.initArray(100, Functions.initArray(10, 5.0f));

	Vector2 scrollPosition;

    private float sliderValue = 1.0f;
    private float maxSliderValue = 10.0f;

	Dataset newDataset;

	private MenuStack menuStack;
	List<GameObject> columns;
	List<List<GameObject>> columnValues; 

	int recordsToShow = 100;

	int boxes = 500;
    
	WWW www; 
    void OnGUI()
    {
        // // Wrap everything in the designated GUI Area
        // GUILayout.BeginArea (new Rect (100,100,200,60));
    
        // // Begin the singular Horizontal Group
        // GUILayout.BeginHorizontal();
    
        // // Place a Button normally
        // if (GUILayout.RepeatButton ("Increase max\nSlider Value"))
        // {
        //     maxSliderValue += 3.0f * Time.deltaTime;
        // }
    
        // // Arrange two more Controls vertically beside the Button
        // GUILayout.BeginVertical();
        // GUILayout.Box("Slider Value: " + Mathf.Round(sliderValue));
        // sliderValue = GUILayout.HorizontalSlider (sliderValue, 0.0f, maxSliderValue);
    
        // // End the Groups and Area
        // GUILayout.EndVertical();
        // GUILayout.EndHorizontal();
        // GUILayout.EndArea();
		//GUILayout.BeginArea(new Rect(200, 200, 600, 400));

		// this.style = new GUIStyle(GUI.skin.box);
		// style.normal.background = boxTexture;
		// style.normal.textColor = Color.black;
		// scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(600), GUILayout.Height(600));
		// GUILayout.BeginVertical();

		// for(int i = 0; i < array.Length; i++){
		// 	GUILayout.BeginHorizontal();

		// 	for(int j = 0; j < array[i].Length; j++){
		// 		GUILayout.Box(array[i][j].ToString(), style);
		// 		//GUILayout.Label(array[i][j].ToString());
		// 	}

		// 	GUILayout.EndHorizontal();
		// }

		// GUILayout.EndVertical();
		// GUILayout.EndScrollView();


		//GUILayout.VerticalScrollbar(0.0f, 1.0f, 0.0f, 10.0f);
		//GUILayout.EndArea();
    }

	void Start () {
		this.menuStack = new MenuStack();
		openDataset();

		//www = new WWW("https://gabrielecimolino.github.io/supertux/");

		// using (WWW www = new WWW("https://gabrielecimolino.github.io/supertux/")){
		// 	yield return www;
		// 	string text = www.text;
		// 	Debug.Log(text);
		// }

		// Debug.Log(Functions.print(newDataset));
		// //List<int> includeIndices = newDataset.getIncludeIndices(); 
		// recordsToShow = boxes / newDataset.columns.Count();
		// Debug.Log("Records to show: " + recordsToShow.ToString());
		// datasetViewPrefab = Instantiate(datasetViewPrefab, Vector3.zero, Quaternion.identity);
		// Camera mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
		// datasetViewPrefab.GetComponent<Canvas>().worldCamera = mainCamera;
		// Vector2 viewPosition = new Vector2(2.0f, 5.0f);
		// Vector2 viewDimensions = new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight);
		// //GameObject panel = datasetViewPrefab.transform.Find("Panel").gameObject;
		// //GameObject scrollPanel = panel.transform.Find("Scroll Panel").gameObject;
		// GameObject scrollView = datasetViewPrefab.transform.Find("Value View").gameObject;
		// //GameObject attributeNames = panel.transform.Find("Attribute Names").gameObject;
		// GameObject attributeScrollView = datasetViewPrefab.transform.Find("Attribute View").gameObject;
		// GameObject attributeViewport = attributeScrollView.transform.Find("Viewport").gameObject;
		// GameObject attributeContent = attributeViewport.transform.Find("Content").gameObject;
		// //viewDimensions = new Vector2(scrollView.GetComponent<RectTransform>().rect.width, scrollView.GetComponent<RectTransform>().rect.height);
		// Debug.Log(viewDimensions);
		// //scrollView.GetComponent<RectTransform>().localPosition = viewPosition;
		// //scrollView.GetComponent<RectTransform>().sizeDelta = viewDimensions;
		// GameObject viewPort = scrollView.transform.Find("Viewport").gameObject;
		// GameObject content = viewPort.transform.Find("Content").gameObject;

		// int numberOfColumns = newDataset.columns.Count;
		// int numberOfRecords = Mathf.Min(newDataset.numberOfRecords, recordsToShow);
		// float cellWidth = (viewDimensions.x - 20.0f) / Mathf.Min(numberOfColumns, 8.0f);
		// float phish = ((1 + Mathf.Sqrt(5.0f)) / 2.0f) + 1.0f;
		// Vector2 cellSize = new Vector2(cellWidth, cellWidth / phish);
		// //attributeScrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.x * numberOfColumns, cellSize.y);
		// attributeScrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.x * (Mathf.Min(numberOfColumns, 8.0f)), cellSize.y);
		// attributeContent.GetComponent<GridLayoutGroup>().cellSize = cellSize;
		// attributeContent.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.x * numberOfColumns, cellSize.y);
		// scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.x * (Mathf.Min(numberOfColumns, 8.0f)), viewDimensions.y - cellSize.y);
		// content.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.x * numberOfColumns, cellSize.y * numberOfRecords);
		// content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize.x, cellSize.y * numberOfRecords);

		// columns = new List<GameObject>();
		// columnValues = new List<List<GameObject>>();
		// string[] attributeNames = newDataset.getAttributeNames();

		// //----------------------------------------
		// for(int i = 0; i < numberOfColumns; i++){
		// 	GameObject newColumn = Instantiate(viewColumnPrefab, Vector3.zero, Quaternion.identity, content.transform);
		// 	newColumn.GetComponent<GridLayoutGroup>().cellSize = cellSize;
		// 	GameObject columnLabel = Instantiate(attributeValuePrefab, Vector3.zero, Quaternion.identity, attributeContent.transform);
		// 	columnLabel.transform.Find("Text").GetComponent<Text>().text = attributeNames[i];
		// 	columns.Add(newColumn);
		// 	List<GameObject> values = new List<GameObject>();
		// 	columnValues.Add(values);
		// }

		// for(int i = 0; i < numberOfRecords; i++){
		// 	string[] record = newDataset.getRecord(i);

		// 	for(int j = 0; j < record.Length; j++){
		// 		GameObject columnValue = Instantiate(attributeValuePrefab, Vector3.zero, Quaternion.identity, columns[j].transform);
		// 		//columns[j].GetComponent<Text>().text += record[j] + "\n----------------------------------------\n";
		// 		columnValue.transform.Find("Text").GetComponent<Text>().text = record[j];
		// 		if((j % 2) == (i % 2)){
		// 			//columnValue.GetComponent<Image>().color = Color.yellow;
		// 		}
		// 		columnValues[j].Add(columnValue);
		// 	}
		// }
	}

	void OnApplicationQuit(){
		Debug.Log("Quitting");
		// foreach(List<GameObject> column in columnValues){
		// 	foreach(GameObject value in column){
		// 		DestroyImmediate(value);
		// 	}
		// }
		// DestroyImmediate(this);
	}
	
	void Update () {
		GameObject currentMenu = menuStack.peek();

		if(currentMenu.transform.name == "DatasetView(Clone)"){
			if(currentMenu.GetComponent<DatasetView>().close()){
				menuStack.closeMenu();
			}
			else{
				if(currentMenu.GetComponent<DatasetView>().testbench != null){
					GameObject uiTestbench = Instantiate(uiTestbenchPrefab, Vector3.zero, Quaternion.identity);
					uiTestbench.GetComponent<UITestbench>().Initialize(currentMenu.GetComponent<DatasetView>().testbench);
					menuStack.push(uiTestbench);
				}
			}
		}
		else if(currentMenu.transform.name == "UITestbench(Clone)"){
			if(currentMenu.GetComponent<UITestbench>().close()){
				menuStack.closeMenu();
			}
		}
	}

	Texture2D makeTexture(int x, int y, Color c){
		Texture2D texture = new Texture2D(x,y);
		
		for(int i = 0; i < x; i++){
			for(int j = 0; j < y; j++){
				texture.SetPixel(i, j, c);
			}
		}
		
		texture.Apply();
		return texture;
	}

	private string cleanPath(string path){
		return path.Trim(Path.GetInvalidPathChars()).Trim(Path.GetInvalidFileNameChars());
	}

	private void openDataset(){
		string path = EditorUtility.OpenFilePanel("Open CSV", "Assets/TicTacToe/Datasets", "csv");
		//CSVReader reader = new CSVReader(path);


		// AssetBundle pokemonBundle = AssetBundle.LoadFromFile("AssetBundles/pokemon");
		// TextAsset pokemonDescription = pokemonBundle.LoadAsset("Assets/Datasets/Descriptions/pokemon.txt") as TextAsset;
		// TextAsset pokemonData = pokemonBundle.LoadAsset("Assets/Datasets/pokemon.csv") as TextAsset;
		// File.WriteAllText("Assets/Resources/Datasets/Descriptions/pokemon.txt", pokemonDescription.text);
		// File.WriteAllText("Assets/Resources/Datasets/pokemon.csv", pokemonData.text);


		// Object[] textAssets = Resources.LoadAll("Datasets/Descriptions/", typeof(TextAsset));
		// string path = "Assets/TicTacToe/Datasets/mushrooms.csv";
		
		// foreach(TextAsset textAsset in textAssets){
		// 	string description = textAsset.text;
		// 	string[] lines = description.Split('\n');
		// 	path = cleanPath(lines[5]);
		// }

		//BuildPipeline.BuildAssetBundles("Assets/TicTacToe/Resources/Datasets", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
		CSVReader reader = new CSVReader(path);
		//GameObject uiTestbench;
		newDataset = reader.getDataset(new List<int>(){});

		GameObject datasetView = Instantiate(datasetViewPrefab, Vector3.zero, Quaternion.identity);
		datasetView.GetComponent<DatasetView>().setDataset(newDataset, GameObject.Find("Main Camera").GetComponent<Camera>(), 500);
		menuStack.push(datasetView);
		// newDataset.setTargetAttribute(4);

		// for(int i = 0; i < newDataset.columns.Count; i++){
		// 	if(i != newDataset.targetAttribute) newDataset.convertColumn(i, "MINMAX");
		// }
		
		// newDataset.buildMatrix();

		// newDataset.reduceDimensions(2);

		// newDataset.partitionDataset(0.7f);

		// Debug.Log(Functions.print(newDataset));

		// Testbench newTestbench = new Testbench(newDataset);

		// uiTestbench = Instantiate(uiTestbenchPrefab, Vector3.zero, Quaternion.identity);
		// uiTestbench.GetComponent<UITestbench>().Initialize(newTestbench);
	}

	// private void iris(){
	// 	//string path = EditorUtility.OpenFilePanel("Open CSV", "Assets/TicTacToe/Datasets", "csv");
	// 	CSVReader reader = new CSVReader("Assets/TicTacToe/Datasets/iris.csv");
	// 	GameObject uiTestbench;
	// 	//CSVReader reader = new CSVReader(path);
	// 	// MatrixDataset newDataset = reader.GetMatrixDataset(new List<int>(){2,7,9});
	// 	MatrixDataset newDataset = reader.GetMatrixDataset(new List<int>(){});
	// 	newDataset.setTargetAttribute(4);
		
	// 	newDataset.buildMatrix();

	// 	// MathNet.Numerics.LinearAlgebra.Factorization.Svd<float> svd = newDataset.GetMatrix().Svd();
	// 	// Debug.Log("C:\n" + Functions.print(svd.U));
	// 	// Debug.Log("S:\n" + Functions.print(svd.S));
	// 	// Debug.Log("F:\n" + Functions.print(svd.VT));

	// 	newDataset.partitionDataset(0.7f);

	// 	//PartitionDataset partition = new PartitionDataset(newDataset);

	// 	//partition.partition(0.7f);
	// 	//partition.setTargetAttribute(0);

	// 	//testbench = new Testbench(new List<int>(){partition.valueSets[partition.targetAttribute].count()}, partition);
	// 	NewTestbench newTestbench = new NewTestbench(newDataset);
	// 	//testbench.setTargetIndex(0);
	// 	//testbench.getPartition().normalize();
	// 	//Debug.Log(Functions.print(testbench.getPartition()));

	// 	uiTestbench = Instantiate(uiTestbenchPrefab, Vector3.zero, Quaternion.identity);
	// 	uiTestbench.GetComponent<UITestbench>().Initialize(newTestbench);
	// }
}
class MenuStack {

	private Stack stack;

	public MenuStack(){
		this.stack = new Stack();
	}

	public void push(GameObject menu){
		if(stack.Count > 0){
			GameObject currentMenu = stack.Peek() as GameObject;
			currentMenu.SetActive(false);
		}
		stack.Push(menu);
	}

	public GameObject peek(){
		return stack.Peek() as GameObject;
	}

	public void closeMenu(){
		if(stack.Count > 0){
			GameObject currentMenu = stack.Pop() as GameObject;
			currentMenu.SetActive(false);
			GameObject.Destroy(currentMenu);
		}
	}
}

public interface Menu{
	bool close();
	void closeMenu();
}