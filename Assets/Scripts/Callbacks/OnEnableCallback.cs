using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableCallback : BaseCallback
{
    private void OnEnable() => CallEvent();
}
