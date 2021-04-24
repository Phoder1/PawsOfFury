using UnityEngine;

public class ShadowController : MonoBehaviour
{
    [SerializeField] Material pulsatingMaterial;
    [SerializeField] Color color;
    public Color Color
    {
        get => pulsatingMaterial.color;
        set
        {
            if (Color != value)
                pulsatingMaterial.color = value;
        }
    }
    public void ChangeColor()
    {
        Color = color;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ChangeColor();
    }
}
