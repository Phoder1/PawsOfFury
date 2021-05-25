using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class animationCallbacks : MonoBehaviour
{
    [SerializeField]
    UnityEvent AttackCallback = new UnityEvent();

  public void Attack() 
  {
        AttackCallback?.Invoke();
  }
}
