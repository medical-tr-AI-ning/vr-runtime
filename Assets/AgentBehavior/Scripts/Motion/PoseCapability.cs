using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        partial class PatientMotionController : IPoseCapability
        {
            const int MaxLayers = 10;
            private bool[] LayerInTransientPose = new bool[MaxLayers];

            public virtual bool UpdatePose(BodyPart part, PoseState state, Reason failReason = null)
            {
                Reason reason = failReason ?? new Reason();
                PoseStateDescriptor descriptor = Pose.State(part);
                if (descriptor == null)
                {
                    reason.Message = "UpdatePose failed: Unknown body part";
                    return false;
                }

                // some checks
                if (Walking && (part == BodyPart.LeftLeg || part == BodyPart.RightLeg))
                {
                    reason.Message = "UpdatePose failed: I'm walking right now...";
                    return false;
                }

                if (GraspingState != GraspingState.Idle && part == BodyPart.RightHand)
                {
                    reason.Message = "UpdatePose failed: I'm trying to grasp that thing.";
                    return false;
                }

                if (state == PoseState.Up)
                {
                    if (part == BodyPart.LeftPalm)
                        if (Pose.State(BodyPart.LeftHand).State != PoseState.Forward)
                        {
                            reason.Message = "UpdatePose failed: Palm up not supported if the hand is not forward";
                            return false;
                        }

                    if (part == BodyPart.RightPalm)
                        if (Pose.State(BodyPart.RightHand).State != PoseState.Forward)
                        {
                            reason.Message = "UpdatePose failed: Palm up not supported if the hand is not forward";
                            return false;
                        }

                    if (part == BodyPart.LeftLeg)
                        if (Pose.State(BodyPart.RightLeg).State == PoseState.Up)
                        {
                            reason.Message = "UpdatePose failed: Cannot lift my left leg while the right is up";
                            return false;
                        }

                    if (part == BodyPart.RightLeg)
                        if (Pose.State(BodyPart.LeftLeg).State == PoseState.Up)
                        {
                            reason.Message = "UpdatePose failed: Cannot lift my right leg while the left is up";
                            return false;
                        }
                }

                // everything else should work
                descriptor.State = state;
                return true;
            }
            
            public virtual bool PoseUpdated
            {
                get
                {
                    for (int i = 1; i < Animator.layerCount && i < MaxLayers; ++i)
                    {
                        if (Animator.IsInTransition(i) || IsInTransientPose(i))
                            return false;
                    }
                    return true;
                }
            }

            public virtual bool IsInTransientPose (int layer)
            {
                layer = Mathf.Clamp(layer, 0, MaxLayers);
                return LayerInTransientPose[layer];
            }
            
            public virtual void SetInTransientPose(int layer, bool transient)
            {
                if (layer >= 0 && layer < MaxLayers)
                    LayerInTransientPose[layer] = transient;
            }

            protected virtual void UpdatePoseState()
            {
                if (Pose.Dirty)
                {
                    foreach (var (bodyPart, descriptor) in Pose.States)
                    {
                        if (descriptor.Dirty)
                        {
                            switch (bodyPart)
                            {
                                case BodyPart.Head:
                                    break;

                                case BodyPart.Face:
                                    Animator.SetBool("mouth opened", descriptor.State == PoseState.MouthOpen);
                                    break;

                                case BodyPart.Body:
                                    Animator.SetBool("bent forward", descriptor.State == PoseState.Bent);
                                    break;

                                case BodyPart.Buttocks:
                                    Animator.SetBool("grasp-buttocks", descriptor.State == PoseState.Spread);
                                    break;

                                case BodyPart.LeftHand:
                                    //Animator.SetBool("left hand up side", descriptor.State == PoseState.Side);
                                    //Animator.SetBool("left hand up", descriptor.State == PoseState.Forward);

                                    if (descriptor.State != PoseState.Down)
                                    {
                                        float lap = (descriptor.State == PoseState.Forward ? 0.0f
                                                    : descriptor.State == PoseState.Up ? 1.0f
                                                    : descriptor.State == PoseState.Side ? 2.0f
                                                    : 0.0f
                                        );
                                        Animator.SetFloat("left arm position", lap);
                                        Animator.SetBool("left arm up", true);
                                    }
                                    else
                                    {
                                        Animator.SetBool("left arm up", false);
                                    }
                                    break;

                                case BodyPart.RightHand:
                                    //Animator.SetBool("right hand up side", descriptor.State == PoseState.Side);
                                    //Animator.SetBool("right hand up", descriptor.State == PoseState.Forward);

                                    if (descriptor.State != PoseState.Down) 
                                    {
                                        float rap = (descriptor.State == PoseState.Forward ? 0.0f
                                                    : descriptor.State == PoseState.Up ? 1.0f
                                                    : descriptor.State == PoseState.Side ? 2.0f
                                                    : 0.0f
                                        );
                                        Animator.SetFloat("right arm position", rap);
                                        Animator.SetBool("right arm up", true);
                                    }
                                    else
                                    {
                                        Animator.SetBool("right arm up", false);
                                    }                                 
                                    break;

                                case BodyPart.LeftPalm:
                                    //Animator.SetBool("left palm up", descriptor.State == PoseState.Up);

                                    Animator.ResetTrigger("rotate left palm");
                                    bool requestLeftPalmUp = descriptor.State == PoseState.Up;
                                    bool isLeftPalmUp      = Animator.GetFloat("left palm position") > 0.5f;
                                    if (requestLeftPalmUp != isLeftPalmUp)
                                        Animator.SetTrigger("rotate left palm");

                                    break;

                                case BodyPart.RightPalm:
                                    //Animator.SetBool("right palm up", descriptor.State == PoseState.Up);

                                    Animator.ResetTrigger("rotate right palm");
                                    bool requestRightPalmUp = descriptor.State == PoseState.Up;
                                    bool isRightPalmUp = Animator.GetFloat("right palm position") > 0.5f;
                                    if (requestRightPalmUp != isRightPalmUp)
                                        Animator.SetTrigger("rotate right palm");

                                    break;

                                case BodyPart.LeftLeg:
                                    Animator.SetBool("left foot up", descriptor.State == PoseState.Up);
                                    Debug.Log($"Left foot up: {descriptor.State == PoseState.Up}");
                                    break;

                                case BodyPart.RightLeg:
                                    Animator.SetBool("right foot up", descriptor.State == PoseState.Up);
                                    Debug.Log($"Right foot up: {descriptor.State == PoseState.Up}");
                                    break;

                                default:
                                    break;
                            }
                        }
                    }

                    Pose.ClearDirty();
                }
            }
        }
    }
}
