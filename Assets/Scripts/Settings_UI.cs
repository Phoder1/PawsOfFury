using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Settings_UI : MonoBehaviour
{
    [SerializeField]
    AudioMixer Mixer;
   

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Volume")) 
        {
            Mixer.SetFloat("Volume", PlayerPrefs.GetFloat("Volume"));
        }
     
    }
    public void SoundToggle() 
    {
            Mixer.GetFloat("Volume", out float value);
            if (value >= 0) 
            {
                Mixer.SetFloat("Volume", -80);
                PlayerPrefs.SetFloat("Volume", -80);
            }
            else 
            {
                Mixer.SetFloat("Volume", 0);
                PlayerPrefs.SetFloat("Volume", 0);
            }
    }

    public void StopGame()
    {
        StartCoroutine(StopGameInumarator());
        
    }
    IEnumerator StopGameInumarator()
    {
        if (Time.timeScale != 0)
        {
        yield return new WaitForSeconds(0.5f);
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        yield return new WaitForSeconds(0.5f);
        }
    }
}
