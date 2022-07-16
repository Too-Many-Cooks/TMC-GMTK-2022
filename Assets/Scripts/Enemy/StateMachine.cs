using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateMachine<T>
{
    public T currentState;

    public delegate bool StateChangeConditionDelegate();

    public class StateChangeEvent : UnityEvent<T, T> { }
    public StateChangeEvent OnStateChange = new StateChangeEvent();

    public struct StateTransition
    {
        public T originState;
        public T targetState;
        public StateChangeConditionDelegate stateChangeCondition;
        public bool conditionInverted;
    }

    //public List<StateTransition> stateTransitions;

    public Dictionary<T, List<StateTransition>> stateTransitions;

    public StateMachine(T currentState)
    {
        this.currentState = currentState;
        stateTransitions = new Dictionary<T, List<StateTransition>>();
    }

    public void AddStateTransition(T originState, T targetState, StateChangeConditionDelegate stateChangeCondition, bool targetConditionValue = true, bool returnOnNegativeCondition = false)
    {
        StateTransition stateTransition;
        stateTransition.originState = originState;
        stateTransition.targetState = targetState;
        stateTransition.stateChangeCondition = stateChangeCondition;
        stateTransition.conditionInverted = false;
        
        if(!stateTransitions.ContainsKey(originState))
        {
            stateTransitions[originState] = new List<StateTransition>();
        }

        stateTransitions[originState].Add(stateTransition);

        if(returnOnNegativeCondition)
        {
            StateTransition reverseStateTransition;
            reverseStateTransition.originState = targetState;
            reverseStateTransition.targetState = originState;
            reverseStateTransition.stateChangeCondition = stateChangeCondition;
            reverseStateTransition.conditionInverted = true;

            if (!stateTransitions.ContainsKey(targetState))
            {
                stateTransitions[targetState] = new List<StateTransition>();
            }

            stateTransitions[targetState].Add(reverseStateTransition);
        }
    }

    public void UpdateState()
    {
        foreach(StateTransition s in stateTransitions[currentState])
        {
            if(s.stateChangeCondition.Invoke() != s.conditionInverted)
            {
                T oldState = currentState;
                currentState = s.targetState;
                OnStateChange.Invoke(oldState, currentState);
                break;
            }
        }
    }
}
