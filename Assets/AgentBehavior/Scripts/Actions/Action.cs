using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        using Extensions;

        public abstract class Action
        {
            public Action(bool bocking)
            {
                Blocking = bocking;
                //Debug.Log($"Action created: {this.GetType()}");
            }

            public abstract void Execute(ActionList list);

            public virtual bool Blocking { get; private set; }
            public virtual bool Ready { get; protected set; } = false;

            // utility
            public virtual void MarkReady() => Ready = true;
        }

        // ...
        public class FenceAction : Action
        {
            public FenceAction() : base(true) { }
            public virtual void OnExecution(ActionList list) { }
            public override void Execute(ActionList list)
            {
                if (list.First == this)
                {
                    MarkReady();
                    OnExecution(list);
                }
            }
        }

        // TODO: dv: too bound on the very particular DMS-Controller... ok?
        public class DMSStateFence : FenceAction
        {
            public DMSStateFence(PatientDecisionController controller, HLState state) 
                : base() 
            { 
                _controller = controller;
                _state = state;
            }

            public override void OnExecution(ActionList list)
            {
                if (_controller != null)
                    _controller.HLState = _state;
            }

            public override void Execute(ActionList list)
            {
                if (list.First == this)
                {
                    MarkReady();
                    OnExecution(list);
                }
            }

            private readonly PatientDecisionController _controller = null;
            private readonly HLState _state;    
        }

        // ...
        public class ActionSequenceBeginFence : FenceAction
        {
            public ActionSequenceBeginFence(ActionRequest request)
                : base() => _request = request;

            public override void OnExecution(ActionList list)
                => _request.Start();

            private readonly ActionRequest _request;
        }

        // ...
        public class ActionSequenceEndFence : FenceAction
        {
            public ActionSequenceEndFence(ActionRequest request)
                : base() => _request = request;

            public override void OnExecution(ActionList list)
                => _request.Finish();

            private readonly ActionRequest _request;
        }

        // ...
        public abstract class BaseAction : Action
        {
            // delay in seconds
            // request is set if this action corresponds to ActionRequest
            // it in null for internal Action-s or within a sequence
            public BaseAction(bool blocking, float delay, ActionRequest request)
                : base(blocking)
            {
                _delay = delay;
                Request = request;
                Request?.Accept();
            }

            // use MarkReady to declare the action finished execution
            public virtual void OnStart(ActionList list) { }

            // use MarkReady to declare the action finished execution
            public virtual void OnUpdate(ActionList list) { }

            // called after the execution was finished
            public virtual void OnReady(ActionList list) { }

            // called if the action timed out
            public virtual void OnTimeout(ActionList list) { }

            public override void Execute(ActionList list)
            {
                // handle the delay, if any
                if (_delay > 0.0f)
                {
                    _delay -= Time.deltaTime;
                    return;
                }

                // execute
                if (!_started)
                {
                    _started = true;
                    Request?.Start();
                    OnStart(list);
                }
                else
                {
                    OnUpdate(list);

                    // check for timeout
                    _watchdogTimer -= Time.deltaTime;
                    if (!Ready && (_watchdogTimer <= 0) && WatchdogEnabled)
                    {
                        OnTimeout(list);
                        list.ActionTimeout(this);
                        Request?.Abort(new Reason("Action timeout"));
                    }
                }

                if (Ready)
                {
                    OnReady(list);
                    Request?.Finish();
                }
            }

            // ...
            protected virtual void ResetWatchdogTimer()
                => _watchdogTimer = WatchdogTimeout;

            // ...
            public ActionRequest Request { protected get; set; } = null;

            // ...
            private bool  _started = false; 
            private float _delay = 0.0f;
            private float _watchdogTimer = WatchdogTimeout;

            private const float WatchdogTimeout = 10.0f; // 5s
            private const bool  WatchdogEnabled = true;
        }

        //...
        // Agent Actions
        // ...
        public class WalkToAction : BaseAction
        {
            public WalkToAction(IWalkCapability cap, bool blocking, Transform target)//, bool ignoreTargetRotation = false) 
                : base(blocking:blocking, delay:0.0f, request:null) 
            {
                _cap = cap;
                _target = target;
                //_ignoreTargetRotation = ignoreTargetRotation;
            }

            public WalkToAction(IWalkCapability cap, bool blocking, GameObject target)//, bool ignoreTargetRotation = false) 
                : this(cap, blocking, target.transform) { }//, ignoreTargetRotation) { }

            public override void OnStart(ActionList list)
                => _cap.WalkTo(_target);//, _ignoreTargetRotation);

            public override void OnUpdate(ActionList list) 
            {
                //if ((_readyCheckDelay -= Time.deltaTime) > 0)
                //    return;

                if (!_cap.Walking) 
                {
                    if (!_cap.WalkTargetReached)
                    {
                        Request?.Abort(new Reason("Stopped walking, but target not reached. Collision?"));
                        list.ActionFailed(this); // The agent stopped before reaching the target 
                    }

                    MarkReady();
                }
            }

            private readonly IWalkCapability _cap;
            private readonly Transform _target;
            //private readonly bool _ignoreTargetRotation;
            //private const float DelayTime = 0.1f; // 100 ms
            //private float _readyCheckDelay = DelayTime;
        }

        // ...
        public class TurnAction : BaseAction
        {
            public TurnAction(ITurnCapability cap, bool blocking, float degrees)
                : base(blocking: blocking, delay: 0.0f, request: null)
            {
                _cap = cap;
                _degrees = degrees;
            }

            public override void OnStart(ActionList list)
                => _cap.TurnBy(_degrees);

            public override void OnUpdate(ActionList list)
            {
                //Debug.Log($"Turn Update:{Ready}, state: {(_cap as MotionController).MotionState}");
                //if ((_readyCheckDelay -= Time.deltaTime) > 0)
                //    return;

                Ready = !_cap.Turning;
                //Debug.Log($"Turn Ready:{Ready}");
            }

            protected readonly ITurnCapability _cap;
            private readonly float _degrees;
            //private const float DelayTime = 0.1f; // 100 ms
            //private float _readyCheckDelay = DelayTime;
        }

        // ...
        public class AlignToAction : TurnAction
        {
            public AlignToAction(ITurnCapability cap, bool blocking, Transform target) 
                : base(cap, blocking, 0.0f)
            {
                _target = target;
            }
            public AlignToAction(ITurnCapability cap, bool blocking, GameObject target) 
                : this(cap, blocking, target.transform) 
            { 
            }

            public override void OnStart(ActionList list)
                => _cap.AlignTo(_target);//, _invert);

            private readonly Transform _target;
            //private readonly bool _invert;
        }

        // ...
        public class TurnToAction : TurnAction
        {
            public TurnToAction(ITurnCapability cap, bool blocking, Transform target, bool invert = false)
                : base(cap, blocking, 0.0f)
            {
                _target = target;
                _invert = invert;
            }
            public TurnToAction(ITurnCapability cap, bool blocking, GameObject target, bool invert = false)
                : this(cap, blocking, target.transform, invert)
            {
            }

            public override void OnStart(ActionList list)
                => _cap.TurnTo(_target, _invert);

            private readonly Transform _target;
            private readonly bool _invert;
        }

        // ...
        public class GraspAction : BaseAction
        {
            public GraspAction(IGraspCapability cap, bool blocking, Transform target) 
                : base(blocking: blocking, delay: 0.0f, request: null) 
            {
                _cap = cap;
                _target = target;
            }

            public override void OnStart(ActionList list)
            {
                if (!_cap.Grasp(_target))
                {
                    Request?.Abort(new Reason($"target:{_target}, state:{_cap.GraspingState}"));
                    list.ActionFailed(this);
                }
            }

            public override void OnUpdate(ActionList list)
            {
                //if (!_cap.GraspTargetReachable)
                //    ResetWatchdogTimer();

                Ready = _cap.Grasped;
            }

            private readonly IGraspCapability _cap;
            private readonly Transform _target;
        }

        // ...
        public class ReleaseGraspAction : BaseAction
        {
            public ReleaseGraspAction(IGraspCapability cap, bool blocking)
                : base(blocking: blocking, delay: 0.0f, request: null)
                => _cap = cap;

            public override void OnStart(ActionList list)
                => _cap.Release();

            public override void OnUpdate(ActionList list)
                => Ready = _cap.Released;

            private readonly IGraspCapability _cap;
        }

        //...
        public class ChangeClothingAction : BaseAction
        {
            public ChangeClothingAction(IChangeClothingCapability cap, bool blocking, ClothingState state)
                : base(blocking: blocking, delay: 0.0f, request: null)
            {
                _cap = cap;
                _clothingState = state;
            }

            public override void OnStart(ActionList list)
                => _cap.ChangeClothing(_clothingState);

            public override void OnUpdate(ActionList list)
                => Ready = _cap.ClothingChanged;

            private readonly IChangeClothingCapability _cap;
            private readonly ClothingState _clothingState;
        }

        // ...
        // object manipulating actions
        // ...
        public abstract class DoorAction : BaseAction
        {
            public DoorAction(IDoor door, bool blocking, bool open)
                : base(blocking: blocking, delay: 0.0f, request: null) 
            { 
                _door = door; 
                _open = open;
            }

            public override void OnStart(ActionList list)
            {
                if (_open) 
                    _door.Open();
                else 
                    _door.Close();
            }

            public override void OnUpdate(ActionList list)
                => Ready = _open ? _door.Opened : _door.Closed;

            private readonly IDoor _door;
            private readonly bool _open;
        }

        public class OpenDoorAction : DoorAction
        {
            public OpenDoorAction(IDoor door, bool blocking) 
                : base(door, blocking, true) { }
        }

        public class CloseDoorAction : DoorAction
        {
            public CloseDoorAction(IDoor door, bool blocking) 
                : base(door, blocking, false) { }
        }

        // ...
        // Pose Actions
        // ...
        public abstract class PoseAction : BaseAction
        {
            public PoseAction(IPoseCapability cap, ActionRequest request = null)
                : base(blocking: false, delay: 0.0f, request: request)
            {
                _cap = cap;
            }

            protected abstract bool UpdatePose(Reason failReason);

            public override void OnStart(ActionList list) 
            {
                // The ActionRequest has already been accepted and started but
                // in the (eventually updated) pose it does not work any more.
                // Thus, the only thing we can do here is to abort the request.
                Reason reason = new();
                if (!UpdatePose(reason))
                {
                    Request?.Abort(reason);
                    MarkReady();
                }
            }

            public override void OnUpdate(ActionList list)
            {
                if ((_readyCheckDelay -= Time.deltaTime) > 0)
                    return;

                Ready = _cap.PoseUpdated;
            }

            // ...
            private readonly IPoseCapability _cap;
            protected IPoseCapability Capability { get => _cap; }
            
            private const float DelayTime = 0.1f; // 100 ms
            float _readyCheckDelay = DelayTime;
        }

        // ...
        public class BodyPoseAction : PoseAction
        {
            public BodyPoseAction(IPoseCapability cap, ActionRequest request, float bending) 
                : base(cap, request) 
            {
                if (bending <= 0.2f)
                    _bodyState = PoseState.Idle;
                else if (bending <= 0.6f)
                    _bodyState = PoseState.Bent;
                else
                    _bodyState = PoseState.Bent; //PoseState.BentMore;
            }

            protected override bool UpdatePose(Reason failReason)
                => Capability.UpdatePose(BodyPart.Body, _bodyState, failReason);

            private readonly PoseState _bodyState;
        }

        // ...
        public class MoveButtocksAction : PoseAction
        {
            public MoveButtocksAction(IPoseCapability cap, ActionRequest request, bool spread) 
                : base(cap, request)
            {
                _spread = spread;
            }

            protected override bool UpdatePose(Reason failReason)
                => Capability.UpdatePose(BodyPart.Buttocks, _spread ? PoseState.Spread : PoseState.Idle, failReason);

            private readonly bool _spread;
        } 

        // ...
        public class HandsPoseAction : PoseAction
        {
            public HandsPoseAction(IPoseCapability cap, ActionRequest request, 
                BodySide side, bool up, HandPose handPose = HandPose.Forward, bool palmsUp = false)
                : base(cap, request) 
            {
                if (up && handPose != HandPose.Forward && palmsUp) // can only be with palms up in Forward-Poses
                    return;
                
                if (side == BodySide.Left || side == BodySide.Both)
                {
                    //_leftHandState = !up ? PoseState.Down : (sidewards ? PoseState.Side : PoseState.Forward);
                    //_leftPalmState = palmsUp ? PoseState.Up : PoseState.Down;
                    _leftHandState = handToPoseState(up, handPose);
                    _leftPalmState = palmToPoseState(up, palmsUp, handPose);
                    
                }
                if (side == BodySide.Right || side == BodySide.Both)
                {
                    //_rightHandState = !up ? PoseState.Down : (sidewards ? PoseState.Side : PoseState.Forward);
                    //_rightPalmState = palmsUp ? PoseState.Up : PoseState.Down;
                    _rightHandState = handToPoseState(up, handPose);
                    _rightPalmState = palmToPoseState(up, palmsUp, handPose);
                }
            }

            protected override bool UpdatePose(Reason failReason)
            {
                if (_leftHandState == PoseState.Invalid && _rightHandState == PoseState.Invalid)
                {
                    if (failReason != null)
                        failReason.Message = "UpdatePose failed: Requested hand pose not supported";
                    return false;
                }

                if (_leftHandState != PoseState.Invalid)
                {
                    if (!Capability.UpdatePose(BodyPart.LeftHand, _leftHandState, failReason) ||
                        !Capability.UpdatePose(BodyPart.LeftPalm, _leftPalmState, failReason))
                        return false;
                }
                if (_rightHandState != PoseState.Invalid)
                {
                    if (!Capability.UpdatePose(BodyPart.RightHand, _rightHandState, failReason) ||
                        !Capability.UpdatePose(BodyPart.RightPalm, _rightPalmState, failReason))
                        return false;
                }
                return true;
            }

            private readonly PoseState _leftHandState  = PoseState.Invalid;
            private readonly PoseState _rightHandState = PoseState.Invalid;
            private readonly PoseState _leftPalmState  = PoseState.Invalid;
            private readonly PoseState _rightPalmState = PoseState.Invalid;

            private PoseState handToPoseState(bool up, HandPose hp)
            {
                if (!up)
                    return PoseState.Down;

                switch (hp)
                {
                    case HandPose.Up: 
                        return PoseState.Up;
                    case HandPose.Forward: 
                        return PoseState.Forward;
                    case HandPose.Side: 
                        return PoseState.Side;
                }
                return PoseState.Invalid;
            }

            private PoseState palmToPoseState(bool up, bool palmsUp, HandPose hp)
            {
                if (!up || hp != HandPose.Forward || !palmsUp)
                    return PoseState.Down;

                return PoseState.Up;
            }
        }

        // ...
        public class LegsPoseAction : PoseAction
        {
            public LegsPoseAction(IPoseCapability cap, ActionRequest request, 
                BodySide side, bool up) 
                : base(cap, request) 
            {
                if (side == BodySide.Left || side == BodySide.Both)
                    _leftLegState = up ? PoseState.Up : PoseState.Down;

                if (side == BodySide.Right || side == BodySide.Both)
                    _rightLegState = up ? PoseState.Up : PoseState.Down;
            }

            protected override bool UpdatePose(Reason failReason)
            {
                if (_leftLegState != PoseState.Invalid)
                    if (!Capability.UpdatePose(BodyPart.LeftLeg, _leftLegState, failReason))
                        return false;

                if (_rightLegState != PoseState.Invalid)
                    if (!Capability.UpdatePose(BodyPart.RightLeg, _rightLegState, failReason))
                        return false;

                return true;
            }

            private readonly PoseState _leftLegState  = PoseState.Invalid;
            private readonly PoseState _rightLegState = PoseState.Invalid;
        }

        // ...
        public class MouthPoseAction : PoseAction
        {
            public MouthPoseAction(IPoseCapability cap, ActionRequest request, bool open) 
                : base(cap, request) => _mouthState = open ? PoseState.MouthOpen : PoseState.MouthClosed;

            protected override bool UpdatePose(Reason failReason)
                => Capability.UpdatePose(BodyPart.Face, _mouthState, failReason);
            
            private readonly PoseState _mouthState;
        }
    }
}
