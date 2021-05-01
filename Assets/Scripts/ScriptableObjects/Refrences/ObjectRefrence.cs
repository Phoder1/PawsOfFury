using System;
using UnityEngine;

namespace Refrences
{
    [CreateAssetMenu(fileName = "new Refrence", menuName = "Refrences/Object")]
    public class ObjectRefrence : ScriptableObject, IRefrenceScriptableObject
    {
        [SerializeField]
        protected object _value = default;

        public virtual object Value
        {
            get => _value;
            set
            {
                if ((value == null && _value != null) || (value != null && !value.Equals(_value)))
                {
                    var _oldValue = _value;
                        _value = value;

                    ValueChanged(_oldValue, _value);
                    TriggerOnValueChanged();
                }
            }
        }
        protected virtual void ValueChanged(object oldValue, object newValue) { }
        protected void TriggerOnValueChanged() => OnValueChanged?.Invoke(Value);
        public event Action<object> OnValueChanged = default;
    }
}
