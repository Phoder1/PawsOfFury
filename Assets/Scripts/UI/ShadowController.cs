using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowController : MonoBehaviour
{
    //[LocalComponent(false, true)]
    //[SerializeField] ParticleSystem particleSystem;
    [LocalComponent(false, true)]
    [SerializeField] Renderer renderer;
    public Color Color {
        get => renderer.material.color;
        set {
            renderer.material.color = value;
        }
    }
}
