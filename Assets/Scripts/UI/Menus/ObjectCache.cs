using Refrences;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class ObjectCache : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private ObjectRefrence _cache;
    [SerializeField, FoldoutGroup("Event", AnimateVisibility = false, Expanded = false)]
    private UnityEvent<object> cacheEvent;
    public void Cache(object unitRef)
    {
        if (unitRef is ObjectRefrence _objRef)
            _cache = _objRef;
    }
    public void Set(object value) => _cache.Value = (Object)value;
}
