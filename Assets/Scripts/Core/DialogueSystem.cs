using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour 
{
	public static DialogueSystem instance;

	public ELEMENTS elements;

	void Awake()
	{
		instance = this;
	}

	/// <summary>
	/// Say something and show it on the speech box.
	/// </summary>
	public void Say(string speech, string speaker = "", bool additive = false)
	{
		StopSpeaking();

		if (additive)
			speechText.text = targetSpeech;

		speaking = StartCoroutine(Speaking(speech, additive, speaker));
	}

	public void StopSpeaking()
	{
		if (isSpeaking)
		{
			StopCoroutine(speaking);
		}
		speaking = null;
	}
		
	public bool isSpeaking {get{return speaking != null;}}
	[HideInInspector] public bool isWaitingForUserInput = false;

	public string targetSpeech = "";
	Coroutine speaking = null;
	IEnumerator Speaking(string speech, bool additive, string speaker = "")
	{
		speechPanel.SetActive(true);
		targetSpeech = speech;

		if (!additive)
			speechText.text = "";
		else
			targetSpeech = speechText.text + targetSpeech;

		speakerNameText.text = DetermineSpeaker(speaker);//temporary

		isWaitingForUserInput = false;

		while(speechText.text != targetSpeech)
		{
			speechText.text += targetSpeech[speechText.text.Length];
			yield return new WaitForEndOfFrame();
		}

		//text finished
		isWaitingForUserInput = true;
		while(isWaitingForUserInput)
			yield return new WaitForEndOfFrame();

		StopSpeaking();
	}

	string DetermineSpeaker(string s)
	{
		string retVal = speakerNameText.text;//default return is the current name
		if (s != speakerNameText.text && s != "")
			retVal = (s.ToLower().Contains("narrator")) ? "" : s;

		return retVal;
	}

	/// <summary
	/// Close the entire speech panel. Stop all dialogue.
	/// </summary>
	public void Close()
	{
		StopSpeaking();
		speechPanel.SetActive (false);
	}

	[System.Serializable]
	public class ELEMENTS
	{
		/// <summary>
		/// The main panel containing all dialogue related elements on the UI
		/// </summary>
		public GameObject speechPanel;
		public Text speakerNameText;
		public Text speechText;
	}
	public GameObject speechPanel {get{return elements.speechPanel;}}
	public Text speakerNameText {get{return elements.speakerNameText;}}
	public Text speechText {get{return elements.speechText;}}
}