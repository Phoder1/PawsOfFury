using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Boss_Sound_Handler : MonoBehaviour
{
    bool Started_Music = false;
    [SerializeField]
    AudioSource AudioSource_ambiant;
    [SerializeField]
    AudioSource AudioSource_boss;
  


    public void set_music_Boss()
    {
        if (!Started_Music)
        {
            Started_Music = true;
            AudioSource_ambiant.Stop();
            AudioSource_boss.Play();
        }
    }

}
