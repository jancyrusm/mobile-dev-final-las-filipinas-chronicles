using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    private static SoundEffect soundEffect;

    void Start()
    {
        if (soundEffect == null)
        {
            soundEffect = this;
            DontDestroyOnLoad(soundEffect);
            soundEffect = GetComponent<SoundEffect>();

            return;
        }

        else 
        {
            Destroy(soundEffect);
        }
    }
}
