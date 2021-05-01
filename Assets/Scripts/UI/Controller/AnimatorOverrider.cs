using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomAttributes;
using Sirenix.OdinInspector;

public class AnimatorOverrider : MonoBehaviour
{
    [SerializeField, LocalComponent]
    private Animator animator;
    [SerializeField]
    private AnimatorOverrideController overrider;



    [Button]
    public void OverrideAnimator(AnimationClip clip)
    {
        List<KeyValuePair<AnimationClip, AnimationClip>> overrides =  new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrider.GetOverrides(overrides);
        overrides.ForEach((x) => x = new KeyValuePair<AnimationClip, AnimationClip>(x.Key, clip));
        overrider.ApplyOverrides(overrides);
        animator.runtimeAnimatorController = null;
        animator.runtimeAnimatorController = overrider;
    }
}
