using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Refrences
{
    [CreateAssetMenu(menuName = "Refrences/Event")]
    public class EventRefrence : ScriptableObject
    {
        [SerializeField, Tooltip("A child event will call this event on trigger."), OnCollectionChanged(Before = "UnsubscribeFromChilds", After = "SubscribeToChilds")]
        private List<EventRefrence> _childEvents;
        [ReadOnly, Tooltip("Actions on trigger.")]
        public UnityRefrenceEvent eventRefrence = new UnityRefrenceEvent();
        public void TriggerEvent() => TriggerEvent(null);
        public void TriggerEvent(object obj) => eventRefrence?.Invoke(obj);
#if UNITY_EDITOR
        private void ResetChildSubscription()
        {
            UnsubscribeFromChilds();
            SubscribeToChilds();
        }
        private void UnsubscribeFromChilds() => _childEvents.ForEach((x) => UnityEditor.Events.UnityEventTools.RemovePersistentListener(x.eventRefrence, TriggerEvent));
        private void SubscribeToChilds() => _childEvents.ForEach((x) => UnityEditor.Events.UnityEventTools.AddPersistentListener(x.eventRefrence, TriggerEvent));
#endif
    }
    [Serializable]
    public class UnityRefrenceEvent : UnityEvent<object>
    {
        public static UnityRefrenceEvent operator +(UnityRefrenceEvent a, UnityAction<object> b)
        {
            a.AddListener(b);
            return a;
        }
        public static UnityRefrenceEvent operator +(UnityRefrenceEvent a, UnityRefrenceEvent b)
        {
            a.AddListener((x) => b?.Invoke(x));
            return a;
        }
        public static UnityRefrenceEvent operator -(UnityRefrenceEvent a, UnityAction<object> b)
        {
            a.RemoveListener(b);
            return a;
        }
        public static UnityRefrenceEvent operator -(UnityRefrenceEvent a, UnityRefrenceEvent b)
        {
            a.RemoveListener((x) => b?.Invoke(x));
            return a;
        }
    }
}
