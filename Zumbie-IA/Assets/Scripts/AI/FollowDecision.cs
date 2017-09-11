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

public class FollowDecision:MonoBehaviour {

	private const string DATA_PATCH = "/AIData/";

	public string fileName = "followData.xlsx";

	// Use this for initialization
	void Start() {
		//Classifier c = new Classifier();
		Learning();
	}

	private void Learning() {
		FileStream stream = File.Open(Application.dataPath + DATA_PATCH + fileName, FileMode.Open, FileAccess.Read);

		//2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
		IExcelDataReader excelReader;

		if(fileName.Contains(".xlsx"))
			excelReader=ExcelReaderFactory.CreateOpenXmlReader(stream);
		else
			excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
		
		//excelReader.IsFirstRowAsColumnNames = true;
		DataSet result = excelReader.AsDataSet();

		//5. Data Reader methods
		while (excelReader.Read()) {
			//excelReader.GetInt32(0);
			Debug.Log(excelReader.GetValue(0));
		}
		//6. Free resources (IExcelDataReader is IDisposable)
		excelReader.Close();

	}

	// Update is called once per frame
	void Update() {

	}
}

public class Classifier {

	public Classifier() {
		DataTable data = new DataTable("Fruits");
		data.Columns.Add("Color");
		data.Columns.Add("Format");
		data.Columns.Add("Class");

		data.Rows.Add("red", "rounded", "apple");
		data.Rows.Add("yellow", "rounded", "limon");
		data.Rows.Add("yellow", "lenghty", "banana");

		var codebook = new Codification(data);

		// Translate our training data into integer symbols using our codebook:
		DataTable symbols = codebook.Apply(data);
		int[][] inputs = symbols.ToArray<int>("Color", "Format");
		int[] outputs = symbols.ToArray<int>("Class");

		var id3learning = new ID3Learning()
		{
				// Now that we already have our learning input/ouput pairs, we should specify our
				// decision tree. We will be trying to build a tree to predict the last column, entitled
				// “PlayTennis”. For this, we will be using the “Outlook”, “Temperature”, “Humidity” and
				// “Wind” as predictors (variables which will we will use for our decision). Since those
				// are categorical, we must specify, at the moment of creation of our tree, the
				// characteristics of each of those variables. So:

				new DecisionVariable("Color",     2), // 3 possible values (Sunny, overcast, rain)
				new DecisionVariable("Rounded", 2), // 3 possible values (Hot, mild, cool)  

				// Note: It is also possible to create a DecisionVariable[] from a codebook:
				// DecisionVariable[] attributes = DecisionVariable.FromCodebook(codebook);
			};

		DecisionTree tree = id3learning.Learn(inputs, outputs);

		// Compute the training error when predicting training instances
		double error = new ZeroOneLoss(outputs).Loss(tree.Decide(inputs));

		// The tree can now be queried for new examples through 
		// its decide method. For example, we can create a query

		int[] query = codebook.Transform(new[,]
			{
					{ "Color",   "yellow"  },
					{ "Format",  "rounded"    }
				});

		// And then predict the label using
		int predicted = tree.Decide(query);  // result will be 0

		// We can translate it back to strings using
		string answer = codebook.Revert("Class", predicted); // Answer will be: "No"
		Debug.Log(answer);

	}
}
