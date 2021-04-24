using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UiWindow : MonoBehaviour, IUiWindow
{
    public virtual void OnReset() { }
    public virtual void OnUpdate() { }

    //public void StartInTransition(Event callback = null)
    //{
    //    throw new System.NotImplementedException();
    //}

    //protected abstract IEnumerator InTransition();
    //public void StartOutTransition(Event callback = null)
    //{
    //    throw new System.NotImplementedException();
    //}
    //protected abstract IEnumerator OutTransition();


}
