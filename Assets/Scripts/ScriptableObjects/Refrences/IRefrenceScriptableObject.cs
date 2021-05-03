using System;

namespace Refrences
{
    public interface IRefrenceScriptableObject
    {
        UnityEngine.Object Value { get; set; }

        //event Action<object> OnValueChanged;
    }
}