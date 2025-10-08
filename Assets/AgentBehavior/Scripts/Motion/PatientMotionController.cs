using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ignore Spelling: Collider
namespace MedicalTraining
{
    namespace AgentBehavior
    {
        // ...
        partial class PatientMotionController : MotionController,
                                                IUseChangingRoomCapability
        {
            public PatientMotionController(PatientAgentComponent agent, Animator animator, CharacterController controller)
                : base(animator)
            {
                CharacterController = controller;
                Agent = agent;

                // ...
                // TODO: dv: Remake it in hierarchical state machine
                Pose = new PoseDescription();
                MotionState = MotionState.Standing;
                AnimatorMotionState = MotionState.Standing;
                Clothing = ClothingState.Dressed;

                // ...
                foreach (var behavior in Animator.GetBehaviours<AnimatorBehavior>())
                    behavior.Controller = this;

                // 
                InitializeGraspCapability();
                InitializeLookAtCapability();
            }

            public virtual ClothingState Clothing { get; set; }
            public virtual GraspingState GraspingState { get; set; } // TODO: dv: protected set
            public virtual PoseDescription Pose { get; set; }
            public virtual MotionState AnimatorMotionState { get; set; }
            protected virtual CharacterController CharacterController { get; private set; }
            protected virtual PatientAgentComponent Agent { get; private set; }

            public override void Update()
            {
                UpdateWalkingState();
                UpdateGraspState();
                UpdatePoseState();
            }

            public override void OnControllerColliderHit(ControllerColliderHit hit)
            {
                if (hit.collider.CompareTag("Ground"))
                    return;

                // TODO: dv: handle collisions and re-orientations here
                HandleWalkCollision();
            }

            public override void OnTriggerEnter(Collider collider)
            {
                // TODO: dv: handle collisions and re-orientations here 
            }

            public override void OnTriggerExit(Collider collider)
            {
                // TODO: dv: handle collisions and re-orientations here 
            }

            // <hack 03.1
            public void FixHeight(bool up)
            {
                Transform rigTransform = Agent.transform.Find("Rig");
                if (!rigTransform)
                    return;

                Vector3 p = rigTransform.localPosition;
                Vector3 offset = new Vector3(0.0f, Agent.MovementSettings.HeightOffset, 0.0f);
                rigTransform.SetLocalPositionAndRotation(p + (up ? 1.0f : -1.0f) * offset, Quaternion.identity);
            }
            // hack>
        }
    }
}