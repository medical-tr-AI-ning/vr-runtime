// Ignore Spelling: collider

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.Configuration;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        internal class PatientAgentController : AgentController
        {
            // Inspector
            [Header("Settings")]
            [SerializeField] protected GameObject UserHeadComponent;

            // <debug
            [Header("Debug")]
            [SerializeField] protected string ActiveAgentIdentifier = "agent-0";
            [ReadOnly][SerializeField] private int  NumActionsInList = 0;
            [ReadOnly][SerializeField] private HLState HighLevelAction;
            [ReadOnly][SerializeField] private ClothingState Clothing;
            //[ReadOnly][SerializeField] private bool PoseIsTransient = false;
            [ReadOnly][SerializeField] private bool PoseUpdated = false;
            [ReadOnly][SerializeField] private GraspingState GraspingState;
            [ReadOnly][SerializeField] private MotionState MotionState;
            [ReadOnly][SerializeField] private MotionState AnimatorMotionState;
            // debug>

            public override ClothingState ClothingState
            {
                get
                {
                    if (MotionController is PatientMotionController pmc)
                        return pmc.Clothing;
                    else
                        return ClothingState.Dressed;
                }
            }

            public override HLState HLState
            {
                get
                {
                    if (DecisionController is PatientDecisionController dms)
                        return dms.HLState;
                    else
                        return HLState.Invalid;
                }
            }

            public override void notify(InteractionEvent interactionEvent)
                => PerceptionController?.EnqueueEvent(interactionEvent);

            public override IArmatureHandle GetArmatureHandle(ArmatureHandleType type)
                => !AgentComponent ? null : AgentComponent.GetArmatureHandle(type);

            protected virtual void Start() 
            {
                GameObject agentNode = ConfigurationContainer.Instance.Agent;
                if (agentNode)
                    AgentComponent = agentNode.GetComponentInChildren<PatientAgentComponent>();

                // as back-up use the Identifier in the Inspector settings and fit 'em all by hand.
                if (!AgentComponent) 
                {
                    Debug.LogWarning("[Behavior] No Agent found by the ConfigurationContainer. Falling back to setting it manually.");
                    PatientAgentComponent[] patients = GetComponentsInChildren<PatientAgentComponent>(true);
                    foreach (PatientAgentComponent patient in patients)
                    {
                        // set and activate the chosen one
                        if (patient.Identifier == ActiveAgentIdentifier)
                        {
                            AgentComponent = patient;
                            AgentComponent.gameObject.SetActive(true);
                        }
                        else // deactivate the rest
                        {
                            patient.gameObject.SetActive(false);
                        }
                    }
                }

                // did we found it?
                if (!AgentComponent)
                {
                    Debug.LogWarning("[Behavior] No active agent (AgentComponent) found.");
                    return;
                }

                // extract and check the animator & co.
                // TODO: dv: Should I move this to the MotionController?
                Animator animator = AgentComponent.GetComponent<Animator>(); 
                CharacterController controller = AgentComponent.GetComponent<CharacterController>();

                if ((!animator || !animator.enabled) || (!controller || !controller.enabled)) 
                {
                    Debug.LogError("[Behavior]: AgentComponent not properly configured. " +
                        "No Animator or Character controller found");
                    return;
                }

                // now create all sub-systems
                PerceptionController = new PatientPerceptionController(UserHeadComponent);
                PerceptionModel      = new BaseLIFOPerceptionModel();
                MotionController     = new PatientMotionController(AgentComponent, animator, controller);
                DecisionController   = new PatientDecisionController(AgentComponent, MotionController);
                ActionList           = new ActionList();

                // <hack 01.2
                AgentComponent.ControllerColliderHit.AddListener(MotionController.OnControllerColliderHit);
                AgentComponent.TriggerEnter.AddListener(MotionController.OnTriggerEnter);
                AgentComponent.TriggerExit.AddListener(MotionController.OnTriggerExit);
                // hack>

                // add WalkHome ActionRequest to ensure consistent agent position 
                notify(new WalkHome(null));
            }

            protected virtual void Update()
            {
                MotionController?.Update();
                PerceptionController?.Update(PerceptionModel);
                DecisionController?.Think(PerceptionModel, ActionList);
                ActionList?.Execute();

                // <debug
                if (MotionController != null)
                {
                    NumActionsInList = ActionList.NumPendingActions;
                    Clothing = ClothingState;
                    HighLevelAction = HLState;
                    //PoseIsTransient = ((PatientMotionController)MotionController).PoseUpdated;
                    PoseUpdated = ((PatientMotionController)MotionController).PoseUpdated;
                    GraspingState = ((PatientMotionController)MotionController).GraspingState;
                    MotionState = ((PatientMotionController)MotionController).MotionState;
                    AnimatorMotionState = ((PatientMotionController)MotionController).AnimatorMotionState;
                }
                // debug>
            }

            protected virtual void LateUpdate()
            {
                DecisionController?.LateUpdate(PerceptionModel);
            }

            // <hack 01.3
            // NOTE: dv: The events are dispatched to the motion controller from the
            //           PatientAgentComponent. The delegates are registered here in Start

            //protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
            //    => MotionController?.OnControllerColliderHit(hit);

            //protected virtual void OnTriggerEnter(Collider collider)
            //    => MotionController?.OnTriggerEnter(collider);

            //protected virtual void OnTriggerExit(Collider collider)
            //    => MotionController?.OnTriggerExit(collider);

            //protected virtual void OnAnimatorIK()
            //    => MotionController?.OnAnimatorIK();
            // hack>

            // ...
            protected virtual PerceptionController PerceptionController { get; set; }
            protected virtual IPerceptionModel PerceptionModel { get; set; }
            protected virtual DecisionController DecisionController { get; set; }
            protected virtual MotionController MotionController { get; set; }
            protected virtual ActionList ActionList { get; set; }
            protected virtual PatientAgentComponent AgentComponent { get; set; } = null;
        }
    }
}
