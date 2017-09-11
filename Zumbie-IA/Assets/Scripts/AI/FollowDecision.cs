using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.IO;
using Accord.Statistics.Filters;
using Accord.Math;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.MachineLearning.DecisionTrees;
using Accord.Math.Optimization.Losses;
using Accord.MachineLearning;
using System.Threading;
using ExcelDataReader;
using System;

public class FollowDecision:MonoBehaviour {

	private const string DATA_PATCH = "/AIData/";
	public string fileName = "followData.xlsx";

	DecisionTree followTree;
	Codification codebook;

	// Use this for initialization
	void Start() {
		Learning();
	}

	private void Learning() {
		DataTable data = GetDataTable(Application.dataPath + DATA_PATCH + fileName);
		DebugTable(data);
		codebook = new Codification(data);
		DataTable symbols = codebook.Apply(data);
		int[][] inputs = symbols.ToArray<int>("NAME", "LIFE","DISTANCE","ATTACKING");
		int[] outputs = symbols.ToArray<int>("NAME");

		var id3learning = new ID3Learning();
		id3learning.Attributes = DecisionVariable.FromCodebook(codebook);

		followTree = id3learning.Learn(inputs, outputs);

		double error = new ZeroOneLoss(outputs).Loss(followTree.Decide(inputs));
	}

	private void Classifier(string name, string life, string distance, string attacking) {
		int[] query = codebook.Transform(new[,]
			{
					{ "NAME",		name },
					{ "LIFE",		life },
					{ "DISTANCE",	distance},
					{ "ATTACKING",  attacking }
				});

		int predicted = followTree.Decide(query);

		string answer = codebook.Revert("NAME", predicted);
		Debug.Log(answer);
	}

	public void DebugTable(DataTable table) {
		string a = "";
		a+=("--- DebugTable(" + table.TableName + ") ---\n");
		int zeilen = table.Rows.Count;
		int spalten = table.Columns.Count;

		// Header
		for (int i = 0; i < table.Columns.Count; i++) {
			string s = table.Columns[i].ToString();
			a+=(String.Format("{0,-20} | ", s));
		}
		a+=(Environment.NewLine);
		for (int i = 0; i < table.Columns.Count; i++) {
			a+=("---------------------|-");
		}
		a+=(Environment.NewLine);

		// Data
		for (int i = 0; i < zeilen; i++) {
			DataRow row = table.Rows[i];
			//Debug.WriteLine("{0} {1} ", row[0], row[1]);
			for (int j = 0; j < spalten; j++) {
				string s = row[j].ToString();
				if (s.Length > 20) s = s.Substring(0, 17) + "...";
				a += (String.Format("{0,-20} | ", s));
			}
			a += (Environment.NewLine);
		}
		for (int i = 0; i < table.Columns.Count; i++) {
			a += ("---------------------|-");
		}
		a += (Environment.NewLine);
		Debug.Log(a);
	}

	private DataTable GetDataTable(string patch) {
		DataTable dt = new DataTable();

		FileStream stream = File.Open(patch, FileMode.Open, FileAccess.Read);
		IExcelDataReader excelReader;

		if (fileName.Contains(".xlsx"))
			excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
		else
			excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

		DataSet result = excelReader.AsDataSet();

		excelReader.Close();

		return result.Tables[0];
	}
}