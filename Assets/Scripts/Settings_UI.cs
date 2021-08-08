using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings_UI : MonoBehaviour
{
    public static bool Vibrations = true;

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

    private bool _paused = false;
    private Tween _pauseTween = null;

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

        _pauseTween = TweenTimeScale(0, 0.4f).SetAutoKill(false).Pause().SetUpdate(true);

    }

    public void VibrateToggle()
    {
        Vibrations = !Vibrations;
        PlayerPrefs.SetInt("Vibration", Vibrations ? 1 : 0);
        Vibrate.sprite = Vibrations ? VibrateOn : VibrateOff;
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


    private void OnDestroy()
    {
        if (_pauseTween.IsActive())
            _pauseTween.Kill();
    }
    public void StopGame()
    {
        if (_paused)
        {
            _pauseTween.PlayBackwards();
        }
        else
        {
            _pauseTween.PlayForward();
        }

        _paused = !_paused;

    }
    private static Tween TweenTimeScale(float to, float duration, Ease ease = Ease.Linear) => DOTween.To(() => Time.timeScale, (x) => Time.timeScale = x, to, duration).SetEase(ease);
}
