using MedicalTraining.AgentBehavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainPositionBehavior : StateMachineBehaviour
{
    public bool IsTransitionState = false; 
    public bool IsOpeningState = false;

    public IDoor Door { get; set; } = null;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Door == null)
            return;

        if (!IsTransitionState)
        {
            Door.RelativePosition = IsOpeningState ? 1.0f : 0.0f;
        }
        else
        {
            float t = stateInfo.normalizedTime;
            Door.RelativePosition = IsOpeningState ? t : 1 - t;
        }
    }
}
