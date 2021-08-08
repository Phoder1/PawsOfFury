using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using MoreMountains.NiceVibrations;

[CreateAssetMenu(menuName = Database.SOFolder + "Vibration profile")]
public class VibrationProfile : ScriptableObject
{
    [SerializeField]
    private List<Profile> _profiles;

    private bool VibrationsOn => Settings_UI.Vibrations;
    private bool VibrationsSupported => MMVibrationManager.Android() && MMVibrationManager.HapticsSupported();
    public void Trigger(int milliseconds) => Vibrate(milliseconds);
    public void Trigger(long milliseconds) => Vibrate(milliseconds);
    public void Trigger(string profileName) => Trigger(_profiles.Find((x) => x.name == profileName));
    private void Trigger(Profile profile) => Vibrate(profile.CachedPattern, profile.repeat);

    private void Vibrate(long[] pattern, int repeat)
    {
        if (VibrationsOn && VibrationsSupported)
            MMNVAndroid.AndroidVibrate(pattern, repeat);
    }
    private void Vibrate(long milliseconds)
    {
        if(VibrationsOn && VibrationsSupported)
            MMNVAndroid.AndroidVibrate(milliseconds);
    }
    [Serializable, InlineProperty]
    private class Profile
    {
        public string name;
        [SerializeField, TableList(AlwaysExpanded = true, ShowIndexLabels = true), OnCollectionChanged("CachePattern")]
        private List<PatternStep> pattern;

        [SerializeField, HideInInspector]
        private long[] _cachedPattern;
        public int repeat = 1;
        public long[] CachedPattern => _cachedPattern;

        [Button]
        private void CachePattern()
        {
            _cachedPattern = new long[pattern.Count * 2];
            for (int i = 0; i < pattern.Count; i++)
            {
                _cachedPattern[i * 2] = pattern[i].delay;
                _cachedPattern[i * 2 + 1] = pattern[i].duration;
            }
        }
        [Serializable]
        public class PatternStep
        {
            [Tooltip("In milliseconds")]
            public long delay;
            [Tooltip("In milliseconds")]
            public long duration;
        }
    }
}
