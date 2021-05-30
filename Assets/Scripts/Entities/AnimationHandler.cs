using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private List<Animator> animators = default;
    public void SetSpeed(float speed) => animators.ForEach((x) => x.speed = speed);
    public void SetTrigger(string trigger) => animators.ForEach((x) => x.SetTrigger(trigger));
    private void Awake()
    {
        animators = new List<Animator>(GetComponentsInChildren<Animator>());
    }
}
