using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorSetter : MonoBehaviour
{
    [SerializeField]
    private Color _color;

    [SerializeField]
    private List<Graphic> _graphics;

    [SerializeField]
    private UnityEvent<Color> OnSetColor;

    public void SetColor() => SetColor(_color);
    public void SetColor(Color color)
    {
        if (color == null)
            return;

        _graphics.ForEach((x) => x.color = color);

        OnSetColor?.Invoke(color);
    }
}
