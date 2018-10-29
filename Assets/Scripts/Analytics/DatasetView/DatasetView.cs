using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatasetView : MonoBehaviour, Menu {

	public GameObject viewColumnPrefab;
	public GameObject attributeValuePrefab;
	public GameObject columnHeaderPrefab;
	public GameObject variableSelectorPrefab;
	public Dataset dataset;
	public Testbench testbench;
	[SerializeField] private GameObject attributeView;
	[SerializeField] private GameObject attributeViewport;
	[SerializeField] private GameObject attributeContent;
	[SerializeField] private GameObject valueView;
	[SerializeField] private GameObject valueViewport;
	[SerializeField] private GameObject valueContent;
	[SerializeField] private GameObject horizontalScrollBar;
	[SerializeField] private GameObject verticalScrollBar;
	[SerializeField] private GameObject layerSizePanel;
	[SerializeField] private GameObject layerPanel;
	[SerializeField] private GameObject addHiddenLayerButton;
	[SerializeField] private GameObject removeHiddenLayerButton;
	[SerializeField] private GameObject partitionSlider;
	[SerializeField] private GameObject partitionText;
	[SerializeField] private GameObject startPanel;
	private Camera camera;
	private List<GameObject> columns;
	private List<List<GameObject>> columnValues; 
	private GameObject targetSelector;
	private GameObject inputSelector;
	private List<GameObject> hiddenLayerSelectors;
	private int columnsInView = 6;
	private List<int> includeIndices;
	private string[] attributeNames;
	private Color defaultColor = new Color((50.0f/255.0f), (50.0f/255.0f), (50.0f/255.0f));

	private bool closeView;

	void Awake(){
		this.testbench = null;
		this.inputSelector = null;
		this.targetSelector = null;
		this.closeView = false;
	}

	void Update(){
		if(targetSelector != null){
			if(attributeNames != default(string[])){
				int value = targetSelector.GetComponent<VariableSelector>().value - 1;

				if(value < includeIndices.Count){
					targetSelector.GetComponent<VariableSelector>().setInputText(attributeNames[includeIndices[value]]);
				}
				else{
					targetSelector.GetComponent<VariableSelector>().setSelectorValue(value);
				}
			}
		}
		if(inputSelector != null){
			if(includeIndices != default(List<int>)){
				int value = inputSelector.GetComponent<VariableSelector>().value;

				if(value > includeIndices.Count - 1){
					inputSelector.GetComponent<VariableSelector>().setSelectorValue(includeIndices.Count - 1);
				}
			}
		}
	}

	public bool close(){
		return this.closeView;
	}

	public void closeMenu(){
		this.closeView = true;
	}

	public void setDataset(Dataset dataset, Camera camera, int boxes){
		this.dataset = dataset;
		this.camera = camera;
		this.gameObject.GetComponent<Canvas>().worldCamera = camera;
		this.columns = new List<GameObject>();
		this.columnValues = new List<List<GameObject>>();
		this.includeIndices = dataset.includeIndices;
		this.attributeNames = dataset.getAttributeNames();
		this.hiddenLayerSelectors = new List<GameObject>();

		Vector2 viewDimensions = new Vector2(camera.pixelWidth, camera.pixelHeight);
		Vector2 viewPosition = new Vector2(0.0f, -viewDimensions.y / 4);

		
		int numberOfColumns = dataset.columns.Count;
		int recordsToShow = boxes / numberOfColumns;
		int numberOfRecords = Mathf.Min(dataset.numberOfRecords, recordsToShow);

		float cellWidth = (viewDimensions.x - verticalScrollBar.GetComponent<RectTransform>().rect.width) / Mathf.Min(numberOfColumns, columnsInView);
		float phish = ((1 + Mathf.Sqrt(5.0f)) / 2.0f) + 1.0f;
		Vector2 cellSize = new Vector2(cellWidth, cellWidth / phish);

		Vector2 attributeViewSize = new Vector2(cellSize.x * (Mathf.Min(numberOfColumns, columnsInView)), viewDimensions.y / 6);
		Vector2 valueViewSize = new Vector2(cellSize.x * (Mathf.Min(numberOfColumns, columnsInView)) + verticalScrollBar.GetComponent<RectTransform>().rect.width, viewDimensions.y - cellSize.y);

		attributeView.GetComponent<RectTransform>().localPosition = new Vector3((attributeViewSize.x - valueViewSize.x) / 4, valueViewSize.y / 4 + attributeViewSize.y / 4 + viewPosition.y, 0.0f);
		attributeView.GetComponent<RectTransform>().sizeDelta = attributeViewSize;
		attributeViewport.GetComponent<RectTransform>().localPosition = new Vector3(-attributeViewSize.x / 2, attributeViewSize.y / 2, 0.0f);
		attributeViewport.GetComponent<RectTransform>().sizeDelta = attributeViewSize;
		attributeContent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize.x, attributeViewSize.y);
		attributeContent.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.x * numberOfColumns, attributeViewSize.y);
		valueView.GetComponent<RectTransform>().localPosition = viewPosition;
		valueView.GetComponent<RectTransform>().sizeDelta = valueViewSize;
		valueContent.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize.x * numberOfColumns, cellSize.y * numberOfRecords);
		valueContent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(cellSize.x, cellSize.y * numberOfRecords);

		//Instantiate the column containers and headers
		for(int i = 0; i < numberOfColumns; i++){
			GameObject newColumn = Instantiate(viewColumnPrefab, Vector3.zero, Quaternion.identity, valueContent.transform);
			newColumn.GetComponent<GridLayoutGroup>().cellSize = cellSize;

			GameObject columnHeader = Instantiate(columnHeaderPrefab, Vector3.zero, Quaternion.identity, attributeContent.transform);
			columnHeader.GetComponent<ColumnHeader>().Initialize(i, this);
			columnHeader.transform.Find("Attribute Name").Find("Text").GetComponent<Text>().text = attributeNames[i];
			columns.Add(newColumn);

			List<GameObject> values = new List<GameObject>();
			columnValues.Add(values);
		}

		//Populate the columns
		for(int i = 0; i < numberOfRecords; i++){
			string[] record = dataset.getRecord(i);

			for(int j = 0; j < record.Length; j++){
				GameObject columnValue = Instantiate(attributeValuePrefab, Vector3.zero, Quaternion.identity, columns[j].transform);
				Text valueText = columnValue.transform.Find("Text").GetComponent<Text>();
				valueText.text = record[j];
				valueText.color = defaultColor;
				columnValues[j].Add(columnValue);
			}
		}

		Vector2 layerSizePosition = new Vector2(viewPosition.x - viewDimensions.x / 12, viewDimensions.y / 4);
		Vector2 layerSizeDimensions = new Vector2(viewDimensions.x / 3, viewDimensions.y / 6);
		Vector2 layerSizeCellSize = new Vector2(layerSizeDimensions.x / (hiddenLayerSelectors.Count + 2), layerSizeDimensions.y);
		Vector2 layerPanelDimensions = new Vector2(layerSizeDimensions.x / 3, layerSizeDimensions.y);
		Vector2 layerPanelPosition = new Vector2(layerSizePosition.x + layerSizeDimensions.x / 2 + layerPanelDimensions.x / 2, layerSizePosition.y);

		this.layerSizePanel.GetComponent<RectTransform>().localPosition = layerSizePosition;
		this.layerSizePanel.GetComponent<RectTransform>().sizeDelta = layerSizeDimensions;
		this.layerSizePanel.GetComponent<GridLayoutGroup>().cellSize = layerSizeCellSize;
		this.layerPanel.GetComponent<RectTransform>().localPosition = layerPanelPosition;
		this.layerPanel.GetComponent<RectTransform>().sizeDelta = layerPanelDimensions;
		this.targetSelector = Instantiate(variableSelectorPrefab, Vector3.zero, Quaternion.identity, layerSizePanel.transform);
		this.targetSelector.GetComponent<VariableSelector>().setSelectorName("Target");
		this.targetSelector.GetComponent<VariableSelector>().setSelectorValue(1);
		this.targetSelector.transform.Find("Buttons").GetComponent<GridLayoutGroup>().cellSize = new Vector2(layerSizeCellSize.x, (layerSizeCellSize.y * 0.75f) / 3);
		this.inputSelector = Instantiate(variableSelectorPrefab, Vector3.zero, Quaternion.identity, layerSizePanel.transform);
		this.inputSelector.GetComponent<VariableSelector>().setSelectorName("Inputs");
		this.inputSelector.GetComponent<VariableSelector>().setSelectorValue(numberOfColumns - 1);
		this.inputSelector.transform.Find("Buttons").GetComponent<GridLayoutGroup>().cellSize = new Vector2(layerSizeCellSize.x, (layerSizeCellSize.y * 0.75f) / 3);
		this.startPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(viewDimensions.x / 12, viewDimensions.y / 12);
		this.startPanel.GetComponent<RectTransform>().localPosition = new Vector2(viewDimensions.x / 2 - viewDimensions.x / 12, viewDimensions.y / 2 - viewDimensions.y / 12);

		updatePartitionValue();
		this.horizontalScrollBar.GetComponent<Scrollbar>().value = 0.0f;
	}

	public void fitSelectorsToPanel(){
		Vector2 panelSize = this.layerSizePanel.GetComponent<RectTransform>().sizeDelta;
		Vector2 cellSize = new Vector2(panelSize.x / (hiddenLayerSelectors.Count + 2), panelSize.y);
		Vector2 selectorCellSize = new Vector2(cellSize.x, (cellSize.y * 0.75f) / 3);

		this.layerSizePanel.GetComponent<GridLayoutGroup>().cellSize = cellSize;
		this.targetSelector.transform.Find("Buttons").GetComponent<GridLayoutGroup>().cellSize = selectorCellSize;
		this.inputSelector.transform.Find("Buttons").GetComponent<GridLayoutGroup>().cellSize = selectorCellSize;

		foreach(GameObject selector in hiddenLayerSelectors){
			selector.transform.Find("Buttons").GetComponent<GridLayoutGroup>().cellSize = selectorCellSize;
		}
	}

	public void addHiddenLayer(){
		GameObject hiddenLayerSelector = Instantiate(variableSelectorPrefab, Vector3.zero, Quaternion.identity, layerSizePanel.transform);
		hiddenLayerSelector.GetComponent<VariableSelector>().setSelectorName("Hidden");
		hiddenLayerSelector.GetComponent<VariableSelector>().setSelectorValue(1);
		this.hiddenLayerSelectors.Add(hiddenLayerSelector);

		fitSelectorsToPanel();
	}

	public void removeHiddenLayer(){
		if(this.hiddenLayerSelectors.Count > 0){
			GameObject hiddenLayerSelector = this.hiddenLayerSelectors[this.hiddenLayerSelectors.Count - 1];
			this.hiddenLayerSelectors.Remove(hiddenLayerSelector);
			Destroy(hiddenLayerSelector);

			fitSelectorsToPanel();
		}
	}

	public void normalizeColumn(string normalization, int columnNumber){
		dataset.convertColumn(columnNumber, normalization);

		resetColumn(columnNumber);
	}

	public void includeColumn(int columnNumber){
		dataset.includeColumn(columnNumber);
		this.includeIndices = dataset.includeIndices;
	}

	public void removeColumn(int columnNumber){
		dataset.removeColumn(columnNumber);
		this.includeIndices = dataset.includeIndices;
	}

	public void resetColumn(int columnNumber){
		int numberOfRecords = columnValues[columnNumber].Count;
		
		foreach(GameObject value in columnValues[columnNumber]){
			Destroy(value);
		}

		columnValues[columnNumber] = new List<GameObject>();

		for(int i = 0; i < numberOfRecords; i++){
			string[] record = dataset.getRecord(i);
			GameObject columnValue = Instantiate(attributeValuePrefab, Vector3.zero, Quaternion.identity, columns[columnNumber].transform);
			Text valueText = columnValue.transform.Find("Text").GetComponent<Text>();
			valueText.text = record[columnNumber];
			valueText.color = defaultColor;
			columnValues[columnNumber].Add(columnValue);
		}
	}

	public void updatePartitionValue(){
		this.partitionText.GetComponent<Text>().text = this.partitionSlider.GetComponent<Slider>().value.ToString("0.0");
	}

	public void createTestbench(){
		Debug.Log("Creating Testbench");
		dataset.setTargetAttribute(includeIndices[this.targetSelector.GetComponent<VariableSelector>().value - 1]);
		dataset.buildMatrix();

		int numberOfInputs = this.inputSelector.GetComponent<VariableSelector>().value;

		if(numberOfInputs < dataset.includeIndices.Count){
			dataset.reduceDimensions(numberOfInputs);
		}
		else{
			numberOfInputs = dataset.includeIndices.Count;
		}

		dataset.partitionDataset(this.partitionSlider.GetComponent<Slider>().value);

		List<int> hiddenLayerSizes = new List<int>();
		
		foreach(GameObject selector in hiddenLayerSelectors){
			hiddenLayerSizes.Add(selector.GetComponent<VariableSelector>().value);
		}

		this.testbench = new Testbench(dataset, hiddenLayerSizes, 0.1f);
	}
}