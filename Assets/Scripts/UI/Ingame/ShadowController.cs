using CustomAttributes;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShadowController : MonoBehaviour
{
    //[LocalComponent(false, true)]
    //[SerializeField] ParticleSystem particleSystem;
    [LocalComponent(hideProperty: false, getComponentFromChildrens: true)]
    [SerializeField] ParticleSystem particles;
    [SerializeField] Color color;
    public Color Color
    {
        get => particles.main.startColor.color;
        set
        {
            var main = particles.main.startColor;
            main.color = value;
        }
    }
    public void ChangeColor(Color color)
    {
        Color = color;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ChangeColor(color);
    }
}
