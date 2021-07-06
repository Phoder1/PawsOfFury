using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings_UI : MonoBehaviour
{
    public static bool Vibrations;

    [SerializeField]
    AudioMixer Mixer;
    [SerializeField]
    Image Sound;
    [SerializeField]
    Image Vibrate;


    [SerializeField]
    Sprite AudioOn;
    [SerializeField]
    Sprite AudioOff;
    [SerializeField]
    Sprite VibrateOn;
    [SerializeField]
    Sprite VibrateOff;


    // Start is called before the first frame update
    void Awake()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            Mixer.SetFloat("Volume", PlayerPrefs.GetInt("Volume"));
        }

        if (PlayerPrefs.HasKey("Vibration"))
        {
            if (PlayerPrefs.GetInt("Vibration") == 1)
            {
                Vibrations = true;
                Vibrate.sprite = VibrateOn;
            }
            else
            {
                Vibrations = false;
                Vibrate.sprite = VibrateOff;
            }
        }

    }

    public void VibrateToggle()
    {
        if (Vibrations)
        {
            Vibrations = false;
            PlayerPrefs.SetInt("Vibration", 0);
            Vibrate.sprite = VibrateOff;
        }
        else
        {
            Vibrations = true;
            PlayerPrefs.SetInt("Vibration", 1);
            Vibrate.sprite = VibrateOn;
        }
    }


    public void SoundToggle()
    {
        Mixer.GetFloat("Volume", out float value);
        if (value >= 0)
        {
            Mixer.SetFloat("Volume", -80);
            PlayerPrefs.SetFloat("Volume", -80);
            Sound.sprite = AudioOff;
        }
        else
        {
            Mixer.SetFloat("Volume", 0);
            PlayerPrefs.SetFloat("Volume", 0);
            Sound.sprite = AudioOn;

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
