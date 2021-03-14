using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.StateMachine;

public class StateMachineHandler : MonoSingleton<StateMachineHandler>
{
    List<StateMachine> stateMachines;

    public override void Awake()
    {
        base.Awake();
        stateMachines = new List<StateMachine>();
    }

    private void Update()
    {
        foreach (StateMachine stateMachine in stateMachines)
        {
            if (stateMachine != null)
                stateMachine.Update();
            else
                stateMachines.Remove(stateMachine);
        }
    }
    public void ResetAll()
    {
        foreach (StateMachine stateMachine in stateMachines)
        {
            if (stateMachine != null)
                stateMachine.Reset();
            else
                stateMachines.Remove(stateMachine);
        }
    }
    public void Subscribe(StateMachine stateMachine)
    {
        if (stateMachine != null)
            stateMachines.Add(stateMachine);
    }
    public void UnSubscribe(StateMachine stateMachine)
    {
        if (stateMachine != null)
            stateMachines.Remove(stateMachine);
    }
    public void Clear() => stateMachines.Clear();
}
