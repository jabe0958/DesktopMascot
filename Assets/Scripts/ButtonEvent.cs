using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEvent : MonoBehaviour {

	public Text result;

	public void ButtonOnClick() {
		StartCoroutine (download ());
	}

	IEnumerator download() {
		WWW www = new WWW ("https://api.tatsudoya.jp/talk/talk2");

		yield return www;

		TalkMessage talkMessage = JsonUtility.FromJson<TalkMessage> (www.text);

		Debug.Log (talkMessage.getMessage());

		result.text += talkMessage.getMessage () + "\n";
	}
}

[System.Serializable]
public class TalkMessage {

	[SerializeField]
	private string message;

	public string getMessage() {
		return message;
	}

}