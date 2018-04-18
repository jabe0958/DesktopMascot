using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NMeCab;

public class Markov {

	private static readonly string DictionaryPath = @"Assets/dic/ipadic";

	private MeCabParam mecabParam;

	private MeCabTagger mecabTagger;

	public Markov() : this(DictionaryPath){
	}

	public Markov(string dictionaryPath) {
		mecabParam = new MeCabParam ();
		mecabParam.DicDir = @"Assets/dic/ipadic";
		mecabTagger = MeCabTagger.Create (mecabParam);
	}

	public Dictionary<string, Dictionary<string, int>> getWeightedDistribution(string sentence) {
		Dictionary<string, Dictionary<string, int>> dict = new Dictionary<string, Dictionary<string, int>> ();

		string targetSentence = "xxxSTARTxxx " + sentence + " xxxENDxxx";

		MeCabNode node = mecabTagger.ParseToNode (targetSentence);

		string preWord = null;
		while (node != null) {
			if (node.CharType > 0) {
				if (preWord != null) {
					Dictionary<string, int> wordDict = null;
					if (dict.ContainsKey (preWord)) {
						wordDict = dict [preWord];
					} else {
						wordDict = new Dictionary<string, int> ();
						dict [preWord] = wordDict;
					}
					if (wordDict.ContainsKey (node.Surface)) {
						int count = wordDict [node.Surface];
						count++;
						wordDict [node.Surface] = count;
					} else {
						wordDict [node.Surface] = 1;
					}
				}
				preWord = node.Surface;
			}
			node = node.Next;
		}

		return dict;
	}

}
