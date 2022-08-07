using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Command_PlayMusic("UrbanJazz");
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
