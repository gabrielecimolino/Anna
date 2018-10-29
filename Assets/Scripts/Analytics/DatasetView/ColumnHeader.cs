using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColumnHeader : MonoBehaviour {

	[SerializeField]private int columnNumber = -1;
	[SerializeField]private Text attributeText;
	[SerializeField]private Text minmaxText;
	[SerializeField]private Text zscoreText;
	[SerializeField]private Text removeText;
	[SerializeField]private DatasetView view;

	[SerializeField]private bool active = true;

	public void Initialize(int columnNumber, DatasetView view){
		this.columnNumber = columnNumber;
		this.view = view;
		this.active = true;

		this.attributeText.color = Color.black;
		this.minmaxText.text = "Min-Max";
		this.minmaxText.color = Color.black;
		this.zscoreText.text = "ZScore";
		this.zscoreText.color = Color.black;
		this.removeText.text = "Remove";
		this.removeText.color = Color.black;
	}

	public void normalizeColumn(string normalization){
		if(active) view.normalizeColumn(normalization, columnNumber);
	}

	public void removeColumn(){
		if(active){
			view.removeColumn(columnNumber);
			removeText.text = "Removed";
			removeText.color = Color.red;
			active = false;
		}
		else{
			view.includeColumn(columnNumber);
			removeText.text = "Remove";
			removeText.color = Color.black;
			active = true;
		}
	}
}
