using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        // ...
        public abstract class BaseDecisionController : DecisionController
        {
            public BaseDecisionController(AgentComponent agent, MotionController controller)
            {
                MotionController = controller;
                Agent = agent;
                LookAtController = new LookAtController(agent);
            }

            public override void Think(IPerceptionModel model, ActionList actionList)
            {
                // first update the overall behavior, if any.
                UpdateBehavior(model, actionList);

                // handle blinks
                // TODO: dv: the blink itself should be handled by the motion controller (IBlinkCapability or similar)
                //           and we'd only decide if and when the agent needs to blink (e.g., w.r.t. its emotional state).
                HandleBlink();

                // handle lip sync
                // Note: dv handled externally be the uLipSync-Components

                // handle lookAt targets
                // TODO: dv: this should be handled by the motion controller
                //LookAtController.Update(model);

                // process all events
                ProcessEvents(model, actionList);
            }

            public override void LateUpdate(IPerceptionModel model)
            {
                base.LateUpdate(model);
                LookAtController.Update(model);
            }

            protected MotionController MotionController { get; }

            protected AgentComponent Agent { get; }

            protected LookAtController LookAtController { get; }

            protected virtual void ProcessEvents(IPerceptionModel model, ActionList actionList)
            {
                while (model.HasEvents)
                {
                    switch (model.DequeueEvent())
                    {
                        case ActionRequest request:
                            ProcessActionRequest(request, model, actionList);
                            break;

                        case SpeechEvent speech:
                            ProcessSpeechEvent(speech, model, actionList);
                            break;

                        case InterestingObjectEvent:
                            // already processed in the PerceptionController... 
                            break;

                        case PerceptionEvent:
                            // already processed in the PerceptionController... 
                            break;

                        case Notification notification:
                            ProcessNotification(notification, model, actionList);
                            break;

                        default:
                            Debug.LogWarning($"Unknown type of InteractionEvent");
                            break;

                        case null:
                            Debug.LogError("WTF: NULL as event!");
                            //throw new ArgumentNullException(nameof(interactionEvent));
                            break;
                    }
                }
            }

            protected virtual void ProcessActionRequest(ActionRequest request, IPerceptionModel model, ActionList actionList)
            {
                // check the request and either accept or reject it
                // then make Action from it and add it to the action list
            }
            
            protected virtual void ProcessNotification(Notification notification, IPerceptionModel model, ActionList actionList)
            {
                // process the Notifications here
                // These may generate Actions or ActionRequests, Update the current state
                // or setup some part of the internal behavior logic... 
            }
            
            protected virtual void ProcessSpeechEvent(SpeechEvent speech, IPerceptionModel model, ActionList actionList)
            {
                // process the SpeachEvents here
                // These may generate Actions or ActionRequests, Update the current state
                // or setup some part of the internal behavior logic...

                // No speech processing or generation is made here!!!
            }
            
            protected virtual void UpdateBehavior(IPerceptionModel model, ActionList actionList)
            {
                // Decide on a new global state/action or behavior and enqueue a new ActionRequest
                // This is mainly useful for the ambient agents, that may want to change between 'talking', 'look arroung', etc.
                // The Patient may change the current 'idle' animation, or similar here.
            }

            private float triggerTime = 0.0f;
            protected virtual void HandleBlink() 
            {
                if (!Agent || !Agent.BlinkSettings.BlinkAnimation)
                    return;

                float time = Time.time;
                bool force   = Random.value < Agent.BlinkSettings.ChanceEarlyBlink;
                bool regular = time > triggerTime;
                bool skip    = Random.value < Agent.BlinkSettings.ChanceSkipBlink;
                bool blink   = force || (regular && !skip);

                if (blink)
                {
                    triggerTime = time + Utilities.MathUtilities.RandInRange(Agent.BlinkSettings.TimeInterval);
                    Agent.BlinkSettings.BlinkAnimation.Play();
                }
            }
        }
    }
}
