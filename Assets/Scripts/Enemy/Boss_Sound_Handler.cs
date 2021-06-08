using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Boss_Sound_Handler : MonoBehaviour
{
    bool Started_Music = false;
    [SerializeField]
    AudioSource AudioSource;
    [SerializeField]
    AudioClip Boos_Music;
  

    public void set_music_Boss()
    {
        if (!Started_Music)
        {
            Started_Music = true;
            AudioSource.Stop();
            AudioSource.PlayOneShot(Boos_Music);
        }
    }
}
