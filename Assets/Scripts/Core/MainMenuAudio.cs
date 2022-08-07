using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Command_PlayMusic("SkeletonWaltz");
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
}
