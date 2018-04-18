using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NMeCab;

public class Parser : MonoBehaviour {

	public InputField inputField;

	public Text result;

	public void ButtonOnClick() {
		Debug.Log (inputField.text);
//		result.text = parse (inputField.text);
//		result.text = getWeightedDistribution (inputField.text);
//		result.text = parseBook("bocchan.txt");
		result.text = generateString();
	}

	private string parse(string sentence) {
		MeCabParam mecabParam = new MeCabParam ();
		mecabParam.DicDir = @"Assets/dic/ipadic";

		MeCabTagger t = MeCabTagger.Create (mecabParam);
		MeCabNode node = t.ParseToNode (sentence);

		string result = "";
		while (node != null) {
			if (node.CharType > 0) {
				result += node.Surface + "\t" + node.Feature + "\n";
			}
			node = node.Next;
		}

		return result;
	}

	private string getWeightedDistribution(string sentence) {
		Markov markov = new Markov ();
		Dictionary<string, Dictionary<string, int>> weightedDistribution = markov.getWeightedDistribution (sentence);

		BinaryFormatter bf = new BinaryFormatter ();

		MarkovDict markovDict = null;

		if (File.Exists (Application.dataPath + MarkovDict.DataPath)) {
			using (FileStream fs = new FileStream (Application.dataPath + "/dict.dat", FileMode.Open)) {
				markovDict = bf.Deserialize (fs) as MarkovDict;
			}
		} else {
			markovDict = new MarkovDict ();
			markovDict.initialize ();
		}

		markovDict.mergeWeightedDistribution (weightedDistribution);

		using (FileStream fs = new FileStream (Application.dataPath + "/dict.dat", FileMode.Create)) {
			bf.Serialize (fs, markovDict);
		}

		string result = "";
		foreach (KeyValuePair<string, Dictionary<string, int>> dict in markovDict.weightedDistribution) {
			result += "\"" + dict.Key + "\" \t ";
			foreach (KeyValuePair<string, int> pair in dict.Value) {
				result += "[\"" + pair.Key + "\" : " + pair.Value.ToString () + "] ";
			}
			result += "\n";
		}

		return result;
	}

	private string parseBook(string bookFileName) {
		MarkovDict markovDict = new MarkovDict ();
		markovDict.initialize ();

		using (StreamReader sr = new StreamReader (Application.dataPath + "/Books/" + bookFileName)) {
			string line = null;
			while ((line = sr.ReadLine ()) != null) {
				Markov markov = new Markov ();
				markovDict.mergeWeightedDistribution (markov.getWeightedDistribution(line));
			}
		}

		BinaryFormatter bf = new BinaryFormatter ();

		using (FileStream fs = new FileStream (Application.dataPath + "/dict.dat", FileMode.Create)) {
			bf.Serialize (fs, markovDict);
		}

		string result = "";
		foreach (KeyValuePair<string, Dictionary<string, int>> dict in markovDict.weightedDistribution) {
			result += "\"" + dict.Key + "\" \t ";
			foreach (KeyValuePair<string, int> pair in dict.Value) {
				result += "[\"" + pair.Key + "\" : " + pair.Value.ToString () + "] ";
			}
			result += "\n";
		}

		return result;
	}

	private string generateString() {
		MarkovDict markovDict = null;

		if (File.Exists (Application.dataPath + MarkovDict.DataPath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			using (FileStream fs = new FileStream (Application.dataPath + "/dict.dat", FileMode.Open)) {
				markovDict = bf.Deserialize (fs) as MarkovDict;
			}
		} else {
			return "";
		}

		return markovDict.generarte ();
	}

}

[Serializable]
public class MarkovDict {
	public static readonly string DataPath = "/dict.dat";

	public static readonly string SentencePrefix = "xxxSTARTxxx";

	public static readonly string SentenceSuffix = "xxxENDxxx";

	public static readonly int MaxChain = 20;

	public Dictionary<string, Dictionary<string, int>> weightedDistribution;

	public void initialize() {
		weightedDistribution = new Dictionary<string, Dictionary<string, int>> ();
	}

	public void mergeWeightedDistribution(Dictionary<string, Dictionary<string, int>> weightedDistribution) {
		foreach (KeyValuePair<string, Dictionary<string, int>> wordDict in weightedDistribution) {
			if (this.weightedDistribution.ContainsKey (wordDict.Key)) {
				foreach (KeyValuePair<string, int> suffixWord in wordDict.Value) {
					if (this.weightedDistribution [wordDict.Key].ContainsKey (suffixWord.Key)) {
						int count = this.weightedDistribution [wordDict.Key] [suffixWord.Key];
						count += suffixWord.Value;
						this.weightedDistribution [wordDict.Key] [suffixWord.Key] = count;
					} else {
						this.weightedDistribution [wordDict.Key] [suffixWord.Key] = suffixWord.Value;
					}
				}
			} else {
				this.weightedDistribution [wordDict.Key] = wordDict.Value;
			}
		}
	}

	public string generarte() {
		string key = SentencePrefix;

		string generated = "";
		for (int i = 0; i < MaxChain; i++) {
			Debug.Log (key);
			Dictionary<string, int> dict = weightedDistribution [key];
			string[] nextKeys = new string[dict.Keys.Count];
			dict.Keys.CopyTo (nextKeys, 0);
			System.Random random = new System.Random();
			key = nextKeys [random.Next (dict.Keys.Count)];
			if (SentenceSuffix.Equals (key)) {
				break;
			}
			generated += key;
		}

		return generated;
	}
}
