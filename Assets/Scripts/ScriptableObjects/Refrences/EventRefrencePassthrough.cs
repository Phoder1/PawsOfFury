using Sirenix.OdinInspector;
using UnityEngine;

namespace Refrences
{
    public class EventRefrencePassthrough : MonoBehaviour
    {
        [SerializeField, Required]
        private EventRefrence _eventRefrence;
        [SerializeField]
        private UnityRefrenceEvent _passThroughEvent;

        private void Start()
        {
            if (_eventRefrence != null)
                _eventRefrence.eventRefrence += _passThroughEvent;
        }

    }
}
