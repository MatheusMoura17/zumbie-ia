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

	public EnemyController[] enemyList;

	private const string DATA_PATCH = "/AIData/";

	public string treeFile = "follow.tree";
	public string fileName = "followData.xlsx";

	DecisionTree followTree;
	Codification codebook;

	// Use this for initialization
	void Start() {
		//followTree = DecisionTree.Load(Application.dataPath + DATA_PATCH + treeFile);
		Learning();
		foreach (EnemyController enemy in enemyList) {
			enemy.DefineTarget();
		}
	}

	private void Learning() {
		DataTable data = GetDataTable(Application.dataPath + DATA_PATCH + fileName);
		DebugTable(data);
		codebook = new Codification(data);
		DataTable symbols = codebook.Apply(data);
		int[][] inputs = symbols.ToArray<int>("CHARACTER_CLASS", "FRIEND", "LIFE", "DISTANCE");
		int[] outputs = symbols.ToArray<int>("FOLLOW");

		var id3learning = new ID3Learning();
		id3learning.Attributes = DecisionVariable.FromData(inputs);

		followTree = id3learning.Learn(inputs, outputs);

		double error = new ZeroOneLoss(outputs).Loss(followTree.Decide(inputs));
		followTree.Save(Application.dataPath + DATA_PATCH + treeFile);
	}

	public EnemyController FindEnemy(EnemyController currentEnemy) {
		foreach (EnemyController enemy in enemyList) {

			string distance = ">=0";
			float realDistance = UnityEngine.Vector3.Distance(currentEnemy.transform.position, enemy.transform.position);
			if (realDistance <= 60)
				distance = "<=60";

			string life = "<=0";
			if (enemy.life >= 50)
				life = ">=50";

			string friend = enemy.characterClass == currentEnemy.characterClass ? "True" : "False";

			if (Rank(enemy.characterClass.ToString(), friend, life, distance))
				return enemy;
		}
		return null;
	}

	private bool Rank(string characterClass, string friend, string life, string distance) {
		try {
			int[] query = codebook.Transform(new[,]
				{
				{ "CHARACTER_CLASS",	characterClass },
				{ "FRIEND", friend},
				{ "LIFE",				life },
				{ "DISTANCE",			distance}
			});

			int predicted = followTree.Decide(query);

			string answer = codebook.Revert("FOLLOW", predicted);
			Debug.Log(name + " : " + answer);
			return answer == "True" ? true : false;
		} catch (Exception) {
			return false;
		}
	}

	public void DebugTable(DataTable table) {
		string a = "";
		a += ("--- DebugTable(" + table.TableName + ") ---\n");
		int zeilen = table.Rows.Count;
		int spalten = table.Columns.Count;

		// Header
		for (int i = 0; i < table.Columns.Count; i++) {
			string s = table.Columns[i].ToString();
			a += (String.Format("{0,-20} | ", s));
		}
		a += (Environment.NewLine);
		for (int i = 0; i < table.Columns.Count; i++) {
			a += ("---------------------|-");
		}
		a += (Environment.NewLine);

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

		DataSet result = excelReader.AsDataSet(new ExcelDataSetConfiguration() {
			ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration() {
				UseHeaderRow = true
			}
		});
		excelReader.Close();

		return result.Tables[0];
	}
}