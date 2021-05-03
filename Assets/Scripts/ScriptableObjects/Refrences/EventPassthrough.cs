using Sirenix.OdinInspector;
using UnityEngine;

namespace Refrences
{
    public class EventPassthrough : MonoBehaviour
    {
        [SerializeField]
        private EventRefrence _eventRefrence;
        public EventRefrence EventRefrence
        {
            get => _eventRefrence;
            set
            {
                if (_eventRefrence == value)
                    return;

                if (_eventRefrence != null)
                    _eventRefrence.eventRefrence -= _passThroughEvent;

                _eventRefrence = value;

                if (_eventRefrence != null)
                    _eventRefrence.eventRefrence += _passThroughEvent;
                _eventRefrence.eventRefrence += _passThroughEvent;

            }
        }
        public Object DefaultData = default;
        [SerializeField, TabGroup("Default", true,AnimateVisibility = false)]
        private UnityRefrenceEvent _passThroughEvent;
        [SerializeField, TabGroup("Early", true, AnimateVisibility = false)]
        private UnityRefrenceEvent _earlyPassthroughEvent;
        [SerializeField, TabGroup("Late", true, AnimateVisibility = false)]
        private UnityRefrenceEvent _latePassthroughEvent;

        private void Start()
        {
            if (_eventRefrence != null)
                _eventRefrence.eventRefrence += Trigger;
        }
        [Button]
        public void TriggerDefault() => Trigger(DefaultData);
        public void Trigger(object data)
        {
            if (isActiveAndEnabled)
            {
                _earlyPassthroughEvent?.Invoke(data);
                _passThroughEvent?.Invoke(data);
                _latePassthroughEvent?.Invoke(data);
            }
        }
        public void SetDefaultData(object data) => DefaultData = (Object)data;

    }
}
