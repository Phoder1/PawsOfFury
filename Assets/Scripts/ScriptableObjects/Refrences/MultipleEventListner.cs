using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Refrences
{
    public class MultipleEventListner : MonoBehaviour
    {
        [SerializeField, ListDrawerSettings(ListElementLabelName = "name", Expanded = true, ShowItemCount = true)]
        private List<EventListner> listners = default;

        private void OnEnable()
        {
            listners.ForEach((x) => x.Enable());
        }
        private void OnDisable()
        {
            listners.ForEach((x) => x.Disable());
        }

        [Serializable]
        public class EventListner
        {
            [SerializeField, Required]
            private EventRefrence _eventRefrence = default;
            public EventRefrence EventRefrence
            {
                get => _eventRefrence;
                set
                {
                    Disable();

                    _eventRefrence = value;

                    Enable();
                }
            }
            public string name => (EventRefrence == null) ? "Unassigned event" : EventRefrence.name.SplitCamelCase();

            [SerializeField, TabGroup("Default", true, AnimateVisibility = false)]
            private UnityRefrenceEvent _passThroughEvent = default;
            [SerializeField, TabGroup("Early", true, AnimateVisibility = false)]
            private UnityRefrenceEvent _earlyPassthroughEvent = default;
            [SerializeField, TabGroup("Late", true, AnimateVisibility = false)]
            private UnityRefrenceEvent _latePassthroughEvent = default;

            private bool _enabled = false;
            public void Enable()
            {
                if (!_enabled && EventRefrence != null)
                {
                    EventRefrence.eventRefrence += Trigger;
                    _enabled = true;
                }
            }
            public void Disable()
            {
                if (_enabled && EventRefrence != null)
                {
                    EventRefrence.eventRefrence -= Trigger;
                    _enabled = false;
                }

            }
            [Button]
            public void Trigger() => Trigger(null);
            public void Trigger(object data)
            {
                if (_enabled)
                    ForceTrigger(data);
            }

            [Button]
            private void ForceTrigger() => ForceTrigger(null);
            private void ForceTrigger(object data)
            {
                _earlyPassthroughEvent?.Invoke(data);
                _passThroughEvent?.Invoke(data);
                _latePassthroughEvent?.Invoke(data);
            }
        }

    }
}
