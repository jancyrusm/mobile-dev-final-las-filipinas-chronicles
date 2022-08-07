using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class NovelControllerChap4_2 : MonoBehaviour 
{
	/// <summary> The lines of data loaded directly from a chapter file.	/// </summary>
	List<string> data = new List<string>();
    /// <summary> the progress in the current data list.     /// </summary>
    int progress = 0;

    // Use this for initialization
    void Start () 
	{
        LoadChapterFile("Chapter4_2");
        Command_PlayMusic("Cafe");
	}
	
	// Update is called once per frame
	void Update () 
	{
		//testing
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			HandleLine(data[progress]);
            progress++;
		}
	}

	public void LoadChapterFile(string fileName)
	{
		data = FileManager.ReadTextAsset(Resources.Load<TextAsset>($"Story/{fileName}"));
		progress = 0;
	}

    void HandleLine(string line)
	{
		string[] dialogueAndActions = line.Split('"');
        //3 objects means there is dialogue.
        //1 object means there is no dialogue. Only actions.

        if (dialogueAndActions.Length == 3)
        {
            HandleDialogue(dialogueAndActions[0], dialogueAndActions[1]);
            HandleEventsFromLine(dialogueAndActions[2]);
        }
        else
        {
            HandleEventsFromLine(dialogueAndActions[0]);
        }
	}

    /// <summary?
    /// Used as a fallback when no speaker is given.
    /// </summary>
    string cachedLastSpeaker = "";
    void HandleDialogue(string dialogueDetails, string dialogue)
    {
        string speaker = cachedLastSpeaker;
        bool additive = dialogueDetails.Contains("+");

        //remove the additive sign from the speaker name area
        if (additive)
            dialogueDetails = dialogueDetails.Remove(dialogueDetails.Length-1);

        //if there are still details, there is a speaker.
        if (dialogueDetails.Length > 0)
        {
            //remove the space after the speaker's name if present.
            if (dialogueDetails[dialogueDetails.Length-1] == ' ')
                dialogueDetails = dialogueDetails.Remove(dialogueDetails.Length-1);

            speaker = dialogueDetails;
            cachedLastSpeaker = speaker;
        }

        //now speak
        //player should not be retrieved as a character.
        if (speaker != "Ako")
        {
            Character character = CharacterManager.instance.GetCharacter(speaker);
            character.Say(dialogue, additive);
        }
        else
        {
            DialogueSystem.instance.Say(dialogue, speaker, additive);
        }
    }

    void HandleEventsFromLine(string events)
    {
        string[] actions = events.Split(' ');

        foreach(string action in actions)
        {
            HandleAction(action);
        }
    }
    
    void HandleAction(string action)
    {
        print("Handle action [" + action + "]");
        string[] data = action.Split('(',')');

        if (data[0] == "playSound")
        {
            Command_PlaySound(data[1]);
            return;
        }
        if (data[0] == "playMusic")
        {
            Command_PlayMusic(data[1]);
            return;
        }    
        if (data[0] == "setBody")
        {
            Command_SetBody(data[1]);
            return;
        }    
        if (data[0] == "enter")
        {
            Command_Enter(data[1]);
            return;
        }
        if (data[0] == "exit")
        {
            Command_Exit(data[1]);
            return;
        }
    }

	void Command_PlaySound(string data)
	{
		AudioClip clip = Resources.Load("Audio/SFX/" + data) as AudioClip;

		if (clip != null)
			AudioManager.instance.PlaySFX(clip);
		else
			Debug.LogError("Clip does not exist - " + data);
	}

	void Command_PlayMusic(string data)
	{
		if (data.ToLower() == "null")
		{
			AudioManager.instance.PlaySong(null);
		}
		else
		{
			AudioClip clip = Resources.Load("Audio/Music/" + data) as AudioClip;

			AudioManager.instance.PlaySong(clip);
		}
	}

    void Command_SetBody(string data)
	{
		string[] parameters = data.Split(',');
		string character = parameters[0];
		string expression = parameters[1];
		float speed = parameters.Length == 3 ? float.Parse(parameters[2]) : 3f;

		Character c = CharacterManager.instance.GetCharacter(character);
		Sprite sprite = c.GetSprite(expression);

		c.TransitionBody(sprite, speed, false);
	}

    void Command_Exit(string data)
	{
		string[] parameters = data.Split(',');
		string[] characters = parameters[0].Split(';');
		float speed = 3;
		bool smooth = false;
		for(int i = 1; i < parameters.Length; i++)
		{
			float fVal = 0; bool bVal = false;
			if (float.TryParse(parameters[i], out fVal))
			{speed = fVal; continue;}
			if (bool.TryParse(parameters[i], out bVal))
			{smooth = bVal; continue;}
		}

		foreach(string s in characters)
		{
			Character c = CharacterManager.instance.GetCharacter(s);
			c.FadeOut(speed,smooth);
		}
	}

	void Command_Enter(string data)
	{
		string[] parameters = data.Split(',');
		string[] characters = parameters[0].Split(';');
		float speed = 3;
		bool smooth = false;
		for(int i = 1; i < parameters.Length; i++)
		{
			float fVal = 0; bool bVal = false;
			if (float.TryParse(parameters[i], out fVal))
			{speed = fVal; continue;}
			if (bool.TryParse(parameters[i], out bVal))
			{smooth = bVal; continue;}
		}

		foreach(string s in characters)
		{
			Character c = CharacterManager.instance.GetCharacter(s, true, false);
			if (!c.enabled)
			{
				c.renderers.bodyRenderer.color = new Color(1,1,1,0);
				c.enabled = true;

				c.TransitionBody(c.renderers.bodyRenderer.sprite,speed,smooth);
			}
			else
				c.FadeIn(speed,smooth);
		}
	}
}
