using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        // ...
        using Extensions;
        using System.Linq;

        public class PatientDecisionController : BaseDecisionController
        {
            public PatientDecisionController(AgentComponent agent, MotionController motionController, int maxActionsInQueue = 100)
                : base(agent, motionController) 
            {
                _maxActionsInQueue = maxActionsInQueue;
            }

            protected override void ProcessActionRequest(ActionRequest actionRequest, IPerceptionModel model, ActionList actionList)
            {
                if (actionList.NumPendingActions >= _maxActionsInQueue)
                {
                    actionRequest.Decline(new Reason($"I'm a bit busy right now. " +
                        $"I have {actionList.NumPendingActions} to work out!"));
                    
                    return;
                }

                if (actionRequest is ChangeClothes changeClothesRequest)
                {
                    if ((HLState != HLState.Idle) && (HLState != HLState.Posing))
                    {
                        if (HLState == HLState.WalkToChangingRoom)
                        {
                            // TODO: dv: answer that you'll do this, find the action and replace it
                            // then declare the previous request for aborted and the current one
                            // for accepted & started.
                            changeClothesRequest.Decline(new Reason("I'm on my way to the changing room."));
                        }
                        else if (HLState == HLState.ChangingClothes)
                        {
                            // TODO: dv: if ChangeClothingAction is still in the list, then do like in
                            // the previous case, if not postpone the execution for the walk-home state.
                            changeClothesRequest.Decline(new Reason("I'm changing my clothes already."));
                        }
                        else if (HLState == HLState.WalkHome)
                        {
                            // TODO: dv: declare the previous request for ready, abort the walk and
                            // push the new sequence in the actionList.
                            changeClothesRequest.Decline(new Reason("I've just changed my clothes, " +
                                "wait to go back to my home position and try again..."));
                        }
                        else
                        {
                            changeClothesRequest.Decline(new Reason("I'm in some funny state and " +
                                "don't want to change my clothes right now."));
                        }
                        return;
                    }

                    // state is fine - handle the request
                    HandleChangeClothesRequest(changeClothesRequest, model, actionList);
                }
                else if (actionRequest is PoseActionRequest poseActionRequest)
                {
                    if ((HLState != HLState.Idle) && (HLState != HLState.Posing))
                    {
                        poseActionRequest.Decline(new Reason("I'm busy with something else in the moment."));
                        return;
                    }
                    // state is fine - handle the request
                    HandlePoseRequest(poseActionRequest, model, actionList);
                }
                else if (actionRequest is WalkHome walkHomeRequest)
                {
                    if (HLState == HLState.WalkHome)
                    {
                        walkHomeRequest.Decline(new Reason("I'm already on my way!"));
                        return;
                    }

                    GameObject target = model.ObjectsWithTag(CustomTagProperty.HomePosition).FirstOrDefault();
                    if (!target) 
                    {
                        walkHomeRequest.Decline(new Reason("I can't find my way home"));
                        return; 
                    }

                    if (Vector3.Distance(target.transform.position, Agent.RootTransform.position) < 0.2f)
                    {
                        walkHomeRequest.Decline(new Reason("I'm actually @home already"));
                        return;
                    }

                    if (MotionController is not IWalkCapability walkCapability)
                    {
                        walkHomeRequest.Decline(new Reason("IWalkingCapability not supported"));
                        return;
                    }

                    // create the action sequence
                    List<Action> actions = new() {
                        new DMSStateFence(this, HLState.WalkHome),
                        new ActionSequenceBeginFence(walkHomeRequest)
                    };

                    if (MotionController is ITurnCapability turnCapability)
                    {
                        actions.Add(new TurnToAction(turnCapability,  blocking:true, target));
                        actions.Add(new WalkToAction(walkCapability,  blocking:true, target));
                        actions.Add(new AlignToAction(turnCapability, blocking:true, target));
                    }
                    else
                    {
                        actions.Add(new WalkToAction(walkCapability, blocking:true, target));
                    }
                    
                    actions.Add(new DMSStateFence(this, HLState.Idle));
                    actions.Add(new ActionSequenceEndFence(walkHomeRequest));
                    
                    // add it to the actionList and accept the request.
                    actionList.Enqueue(actions);
                    walkHomeRequest.Accept();  
                }
                else if (actionRequest is Turn turnRequest)
                {
                    if (HLState != HLState.Idle && HLState != HLState.Posing)
                    {
                        turnRequest.Decline(new Reason("I'm doing something else now!"));
                        return;
                    }

                    if (MotionController is not ITurnCapability turnCapability)
                    {
                        turnRequest.Decline(new Reason("ITurnCapability not supported"));
                        return;
                    }

                    if (turnRequest.Type == Turn.TurnType.User && !model.User)
                    {
                        turnRequest.Decline(new Reason("Cannot find a user to turn to."));
                        return;
                    }

                    // create the action sequence
                    List<Action> actions = new() {
                        new DMSStateFence(this, HLState.Turning),
                        new ActionSequenceBeginFence(turnRequest)
                    };

                    if (HLState == HLState.Posing && MotionController is IPoseCapability poseCapability)
                    {
                        if (turnRequest.AutoAdjustPose)
                        {
                            // reset the feet
                            actions.Add(new LegsPoseAction(poseCapability, null, BodySide.Both, false));

                            // reset the bend state
                            actions.Add(new BodyPoseAction(poseCapability, null, 0.0f));

                            // wait
                            actions.Add(new FenceAction());
                        }
                        else if (!poseCapability.Pose.Body.Idle || !poseCapability.Pose.LeftLeg.Idle || !poseCapability.Pose.RightLeg.Idle)
                        {
                            // decline and return (this will dismiss the action sequence)
                            turnRequest.Decline(new Reason("I'm not in a good state to turn around... " +
                                "and AutoAdjustState is not allowed in this request"));

                            return;
                        }
                    }

                    if (turnRequest.Type == Turn.TurnType.Transform)
                    {
                        actions.Add(new TurnToAction(turnCapability, blocking: true, turnRequest.Target, turnRequest.Invert));
                    }
                    else if (turnRequest.Type == Turn.TurnType.User)
                    {
                        actions.Add(new TurnToAction(turnCapability, blocking: true, model.User, turnRequest.Invert));
                    }
                    else
                    {
                        actions.Add(new TurnAction(turnCapability, true, turnRequest.Angle));
                    }

                    actions.Add(new DMSStateFence(this, HLState.Idle));
                    actions.Add(new ActionSequenceEndFence(turnRequest));

                    // add it to the actionList and accept the request.
                    actionList.Enqueue(actions);
                    turnRequest.Accept();
                }
                else
                {
                    actionRequest.Decline(new Reason("Request not supported"));
                }
            }

            protected virtual bool HandleChangeClothesRequest(ChangeClothes request, IPerceptionModel model, ActionList actionList)
            {
                // can the agent use the changing room?
                if (MotionController is IUseChangingRoomCapability capability)
                {
                    // nothing to do, if we are in the appropriate state already.
                    if (request.Clothing == capability.Clothing)
                    {
                        request.Decline(new Reason($"... but I'm already {capability.Clothing}"));
                        return false;
                    }

                    // found a changing room?
                    GameObject changingRoomObject = (model.ObjectsWithTag(CustomTagProperty.ChangingRoom)).FirstOrDefault();
                    if (changingRoomObject == null)
                    {
                        request.Decline(new Reason("No changing room object found"));
                        return false;
                    }

                    // is the object properly configured?
                    ChangingRoomComponent changingRoom = changingRoomObject.GetComponentInChildren<ChangingRoomComponent>();
                    if (!changingRoom)
                    {
                        request.Decline(new Reason("No changing room component found"));
                        return false;
                    }

                    // target to go to after using the changing room
                    GameObject target = model.ObjectsWithTag(CustomTagProperty.HomePosition).FirstOrDefault();
                    target = target ? target : model.User;

                    // now we have everything we need and can create the entire action sequence
                    // ... but first check and reset the pose (if any)
                    if (MotionController is IPoseCapability poseCapability)
                    {
                        actionList.Enqueue(new FenceAction());
                        actionList.Enqueue(new MouthPoseAction(poseCapability, null, false));
                        actionList.Enqueue(new BodyPoseAction(poseCapability, null, 0.0f));
                        actionList.Enqueue(new HandsPoseAction(poseCapability, null, BodySide.Both, false));
                        actionList.Enqueue(new LegsPoseAction(poseCapability, null, BodySide.Both, false));
                        actionList.Enqueue(new DMSStateFence(this, HLState.Idle));
                        actionList.Enqueue(new FenceAction());
                    }

                    // now go on with the undress-action sequence
                    List<Action> actions = new()
                        {
                            new ActionSequenceBeginFence(request),

                                // walk to the changing room
                                new DMSStateFence(this, HLState.WalkToChangingRoom),
                                new TurnToAction(cap:capability, blocking:true, changingRoom.WalkTargetOut),
                                new WalkToAction(cap:capability, blocking:true, changingRoom.WalkTargetOut),
                                new AlignToAction(cap:capability, blocking:true, changingRoom.WalkTargetOut),

                                // open 
                                new GraspAction(cap:capability, blocking:true, changingRoom.GraspTargetOut),
                                new OpenDoorAction(door:changingRoom, blocking:true),
                                new ReleaseGraspAction(cap:capability, blocking:true),

                                // walk in and turn around
                                new WalkToAction(cap:capability, blocking:true, changingRoom.WalkTargetIn),
                                new AlignToAction(cap:capability, blocking:true, changingRoom.WalkTargetIn),

                                // close
                                new DMSStateFence(this, HLState.ChangingClothes),
                                new GraspAction(cap:capability, blocking:true, changingRoom.GraspTargetIn),
                                new CloseDoorAction(door:changingRoom, blocking:true),
                                new ReleaseGraspAction(cap:capability, blocking:true),

                                // change clothing state
                                new ChangeClothingAction(cap:capability, blocking:true, request.Clothing),

                                // open                           
                                new GraspAction(cap:capability, blocking:true, changingRoom.GraspTargetIn),
                                new OpenDoorAction(door:changingRoom, blocking:true),
                                new ReleaseGraspAction(cap:capability, blocking:true),

                                // walk out and turn around
                                new DMSStateFence(this, HLState.WalkHome),
                                new WalkToAction(cap:capability, blocking:true, changingRoom.WalkTargetOut),
                                new AlignToAction(cap:capability, blocking:true, changingRoom.WalkTargetOut),

                                // close
                                new GraspAction(cap:capability, blocking:true, changingRoom.GraspTargetOut),
                                new CloseDoorAction(door:changingRoom, blocking:true),
                                new ReleaseGraspAction(cap:capability, blocking:true),

                                // turn to user and walk to him/her  
                                new TurnToAction(cap:capability, blocking:true, target),
                                new WalkToAction(cap:capability, blocking:true, target),
                                new AlignToAction(cap:capability, blocking:true, target),
                                new DMSStateFence(this, HLState.Idle),

                            new ActionSequenceEndFence(request)
                        };

                    actionList.Enqueue(actions);
                    request.Accept();
                    return true;
                }
                
                // 
                request.Decline(new Reason("Capability not supported"));
                return false;
            }

            protected virtual bool HandlePoseRequest(PoseActionRequest request, IPerceptionModel model, ActionList actionList)
            {
                if (MotionController is IPoseCapability capability)
                {
                    switch (request)
                    {
                        case ResetPose poseRequest:
                            actionList.Enqueue(new ActionSequenceBeginFence(poseRequest));
                            actionList.Enqueue(new MoveButtocksAction(capability, null, false));
                            if (poseRequest.Body)
                                actionList.Enqueue(new BodyPoseAction(capability, null, 0.0f)); // for fully straight
                            if (poseRequest.Hands)
                                actionList.Enqueue(new HandsPoseAction(capability, null, BodySide.Both, false));
                            if (poseRequest.Legs)
                                actionList.Enqueue(new LegsPoseAction(capability, null, BodySide.Both, false));

                            actionList.Enqueue(new ActionSequenceEndFence(poseRequest));
                            poseRequest.Accept();
                            return true;

                        case ResetBodyPose poseRequest:
                            actionList.Enqueue(new MoveButtocksAction(capability, null, false));
                            actionList.Enqueue(new BodyPoseAction(capability, poseRequest, 0.0f)); // for fully straight
                            return true;

                        case ResetHandPose poseRequest:
                            actionList.Enqueue(new MoveButtocksAction(capability, null, false));
                            actionList.Enqueue(new HandsPoseAction(capability, poseRequest, poseRequest.Side, false));
                            return true;

                        case ResetLegPose poseRequest:
                            actionList.Enqueue(new LegsPoseAction(capability, poseRequest, poseRequest.Side, false));
                            return true;

                        case RaiseHand poseRequest:
                            if (poseRequest.HandPose != HandPose.Forward && poseRequest.PalmUp)
                            {
                                poseRequest.Decline(new Reason("I can only show palms up, if my hands are forward."));
                                return false;
                            }

                            actionList.Enqueue(new MoveButtocksAction(capability, null, false));
                            actionList.Enqueue(new HandsPoseAction(capability, poseRequest, poseRequest.Side,
                                    true, poseRequest.HandPose, poseRequest.PalmUp));
                            
                            return true;

                        case RaiseLeg poseRequest:
                            if (poseRequest.Side == BodySide.Both)
                            {
                                poseRequest.Decline(new Reason("Do you really want me to lift both legs?"));
                                return false;
                            }

                            if (poseRequest.AutoResetLegPose)
                            {
                                actionList.Enqueue(new List<Action>{
                                    new ActionSequenceBeginFence(poseRequest),
                                        new LegsPoseAction(capability, null, BodySide.Both, false),
                                        new FenceAction(),
                                        new LegsPoseAction(capability, null, poseRequest.Side, true),
                                    new ActionSequenceEndFence(poseRequest)
                                });
                                poseRequest.Accept();
                                return true;
                            }

                            actionList.Enqueue(new LegsPoseAction(capability, poseRequest, poseRequest.Side, true));
                            return true;

                        case OpenMouth poseRequest:
                            actionList.Enqueue(new MouthPoseAction(capability, poseRequest, true));
                            return true;

                        case CloseMouth poseRequest:
                            actionList.Enqueue(new MouthPoseAction(capability, poseRequest, false));
                            return true;

                        case BendForward poseRequest:
                            actionList.Enqueue(new MoveButtocksAction(capability, null, false));
                            actionList.Enqueue(new BodyPoseAction(capability, poseRequest, 1.0f)); // for bending fully forward
                            return true;

                        case MoveButtocks poseRequest:
                            if (poseRequest.Spread && poseRequest.AutoBend)
                            {
                                actionList.Enqueue(new List<Action>{
                                    new ActionSequenceBeginFence(poseRequest),
                                        new HandsPoseAction(capability, null, BodySide.Both, false),
                                        new BodyPoseAction(capability, null, 1.0f),
                                        new FenceAction(),
                                        new MoveButtocksAction(capability, null, poseRequest.Spread),
                                    new ActionSequenceEndFence(poseRequest)
                                });
                                poseRequest.Accept();
                            }
                            else
                            {
                                actionList.Enqueue(new MoveButtocksAction(capability, poseRequest, poseRequest.Spread));
                            }
                            return true;
                    }
                    
                    // if we are here, then the request was not recognized
                    request.Decline(new Reason("Unknown pose request."));
                    return false;
                }

                // capability not supported for this agent
                request.Decline(new Reason("Capability not supported!"));
                return false;
            }

            protected override void UpdateBehavior(IPerceptionModel model, ActionList actionList)
            {
                // check idle or posing and enable/disable the LookAt
                if (MotionController is IPoseCapability cap)
                {
                    if (HLState == HLState.Idle && !cap.Pose.Idle)
                    {
                        HLState = HLState.Posing;
                    }
                    else if (HLState == HLState.Posing && cap.Pose.Idle)
                    {
                        HLState = HLState.Idle;
                    }

                    LookAtController.Enabled = (HLState == HLState.Idle)
                        || (HLState == HLState.Posing && cap.Pose.Body.Idle);
                }
                else
                {
                    LookAtController.Enabled = (HLState == HLState.Idle);
                }
            }

            protected override void ProcessSpeechEvent(SpeechEvent speech, IPerceptionModel model, ActionList actionList)
            {
                switch (HLState)
                {
                    case HLState.Idle:   // fall-through
                    case HLState.Posing: // fall-through
                    case HLState.WalkHome:
                        speech.Accept();
                        break;
                    
                    case HLState.WalkToChangingRoom:
                        speech.Decline(new Reason("Don't want to talk right now"));
                        break;
                    
                    case HLState.ChangingClothes:
                        speech.Decline(new Reason("Well, I don't want to talk while changing clothes."));
                        break;
                }
            }

            // ...
            public HLState HLState = HLState.Idle;
            private readonly int _maxActionsInQueue;
        }
    }
}
