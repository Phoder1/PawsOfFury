using System;
namespace UnityEngine.Events
{
    [Serializable]
    public class UnityEventWrap<T> : UnityEvent<T>
    {
        public static UnityEventWrap<T> operator +(UnityEventWrap<T> a, UnityAction<T> b)
        {
            a.AddListener(b);
            return a;
        }
        public static UnityEventWrap<T> operator +(UnityEventWrap<T> a, UnityEventWrap<T> b)
        {
            a.AddListener((x) => b?.Invoke(x));
            return a;
        }
        public static UnityEventWrap<T> operator -(UnityEventWrap<T> a, UnityAction<T> b)
        {
            a.RemoveListener(b);
            return a;
        }
        public static UnityEventWrap<T> operator -(UnityEventWrap<T> a, UnityEventWrap<T> b)
        {
            a.RemoveListener((x) => b?.Invoke(x));
            return a;
        }
    }
}
