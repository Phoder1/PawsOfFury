using System;

namespace Refrences
{
    public interface IRefrenceScriptableObject
    {
        object Value { get; set; }

        event Action<object> OnValueChanged;
    }
}