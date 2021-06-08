using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Boss_Sound_Handler : MonoBehaviour
{
    [SerializeField]
    Entity Boss;
    float HP;
    bool Started_Music;
    [SerializeField]
    AudioSource AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        Boss = GetComponent<Entity>();
    }

   void set_music_Boss() 
    {
    }
}
