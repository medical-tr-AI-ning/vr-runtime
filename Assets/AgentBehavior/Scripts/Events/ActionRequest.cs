using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        /// <summary>
        /// Helper enum to define which body side we want to manipulate, 
        /// i.e., left hand, right leg, both hands. 
        /// </summary>
        /// <remarks>
        /// Used in ResetHandPose, ResetLegPose, RaiseHand, and RaiseLeg ActionRequests
        /// </remarks>
        public enum BodySide
        {
            //None, 
            Left, 
            Right, 
            Both
        }

        /// <summary>
        /// Helper enum to define how the agent should lift the hands 
        /// </summary>
        public enum HandPose
        {
            //Down,
            Up,
            Forward,
            Side
        }

        /// <summary>
        /// Common interface for all Action Requests.
        /// </summary>
        public abstract class ActionRequest : InteractionEvent
        {
            /// <summary>
            /// The constructor of the ActionRequest
            /// </summary>
            /// <param name="observer">Optional observer for this ActionRequest.</param>
            public ActionRequest(IActionRequestStateObserver observer = null) 
            {
                _observers = new List<IActionRequestStateObserver>();
                if (observer != null)
                    AddObserver(observer);
            }

            /// <summary>
            /// Adds an IActionRequestObserver to be informed about 
            /// the state of this request
            /// </summary>
            /// <param name="observer">The object to be informed about status changes</param>
            public virtual void AddObserver(IActionRequestStateObserver observer)
            {
                // dv: assert(observer)
                if (observer != null)
                    _observers.Add(observer);
            }

            /// <summary>
            /// Removes a particular observer from the list for this request
            /// </summary>
            /// <param name="observer">The object to be removed from the list</param>
            public virtual void RemoveObserver(IActionRequestStateObserver observer)
            {
                // dv: assert(observer)
                if (observer != null)
                    _observers.Remove(observer);
            }

            // ...
            // Internal use only!
            public IEnumerable<IActionRequestStateObserver> Observers { get => _observers; }
            private List<IActionRequestStateObserver> _observers;
        }

        /// <summary>
        /// Base class for all pose action requests. 
        /// </summary>
        public abstract class PoseActionRequest : ActionRequest
        {
            public PoseActionRequest(IActionRequestStateObserver observer)
                : base(observer) { }    
        }

        /// <summary>
        /// The ResetPose ActionRequest informs the agent that we want it to reset its pose. 
        /// Since the pose is layered, the different layers may be controlled independently.  
        /// The flags control whether the deeper layers need to be reset.
        /// </summary>
        public class ResetPose : PoseActionRequest
        {
            /// <summary>
            /// The constructor of the ResetPose ActionRequest.
            /// </summary>
            /// <param name="observer">The object to be informed about status changes. 
            /// Can be null if no observer is needed.</param>
            /// <param name="resetBodyPose">If set, the BodyPose (bent forward) will be reset.</param>
            /// <param name="resetHandPose">If set, the HandPose will be reset for both hands.</param>
            /// <param name="resetLegsPose">If set, the LegPose will be reset for both legs.</param>
            public ResetPose(IActionRequestStateObserver observer, bool resetBodyPose = true, bool resetHandPose = true, bool resetLegsPose = true)
                : base(observer)
            {
                Body  = resetBodyPose;
                Hands = resetHandPose;
                Legs  = resetLegsPose;
            }

            /// ...
            public bool Body { get; }
            public bool Hands { get; }
            public bool Legs { get; }
        }

        /// <summary>
        /// The ResetBodyPose ActionRequest informs the agent that we only want it to reset the BodyPose.
        /// Use this ActionRequest to "unbend" the Agent.
        /// </summary>
        public class ResetBodyPose : PoseActionRequest
        {
            /// <summary>
            /// The constructor of the ResetBodyPose ActionRequest.
            /// </summary>
            /// <param name="observer">The object to be informed about status changes.
            /// Can be null if no observer is needed.</param>
            public ResetBodyPose(IActionRequestStateObserver observer)
                : base(observer)
            {
            }
        }

        /// <summary>
        /// The ResetHandPose ActionRequest resets the only hands' pose. 
        /// Use this if you want to reset only (one of) the hands.
        /// </summary>
        public class ResetHandPose : PoseActionRequest
        {
            /// <summary>
            /// The constructor of the ResetHandPose ActionRequest
            /// </summary>
            /// <param name="observer">The object to be informed about status changes.
            /// Can be null if no observer is needed.</param>
            /// <param name="side">Which hand should be affected.</param>
            public ResetHandPose(IActionRequestStateObserver observer, BodySide side = BodySide.Both)
                : base(observer)
            {
                Side = side;
            }

            /// ...
            public BodySide Side { get; }
        }

        /// <summary>
        /// The ResetLegPose ActionRequest resets only the legs' pose. 
        /// Use this if you want to reset only (one of) the legs.
        /// </summary>
        public class ResetLegPose : PoseActionRequest
        {
            /// <summary>
            /// The constructor of the ResetLegPose ActionRequest
            /// </summary>
            /// <param name="observer">The object to be informed about status changes.
            /// Can be null if no observer is needed.</param>
            /// <param name="side">Which leg should be affected.</param>
            public ResetLegPose(IActionRequestStateObserver observer, BodySide side = BodySide.Both)
                : base(observer)
            {
                Side = side;
            }

            /// ...
            public BodySide Side { get; }
        }

        /// <summary>
        /// The RaiseHand ActionRequest informs the Agent that we want it to raise 
        /// its hand(s). The Agent can raise the hand forward or to the side (T-Pose).
        /// With the hand raised forward, the palms may be up or down.
        /// </summary>
        /// <remarks>
        /// - Palm Up is (currently) ignored if the hand is in T-Pose.  
        /// - If the hand is already up, it will instantly change from T-Pose to 
        ///   Forward and from Palms-Up to Palms-Down (and vice versa). 
        /// - If the requested pose cannot be achieved (e.g., already in this pose, 
        ///   Palm-Up while in T-Pose) the request will be declined.
        /// </remarks>
        public class RaiseHand : PoseActionRequest
        {
            /// <summary>
            /// The constructor of the RaiseHand ActionRequest.
            /// </summary>
            /// <param name="observer">The object to be informed about status changes.
            /// Can be null if no observer is needed.</param>
            /// <param name="side">Which hand(s) will be affected.</param>
            /// <param name="handPose">Raise hand(s) forward, up, or sidewards.</param>
            /// <param name="palmUp">Palms-Up or not.</param>
            public RaiseHand(IActionRequestStateObserver observer, BodySide side, HandPose handPose = HandPose.Forward, bool palmUp = false)
                : base(observer)
            {
                Side = side;
                HandPose = handPose;
                PalmUp = palmUp;
            }

            /// ...
            public BodySide Side { get; }
            public HandPose HandPose { get; }
            public bool PalmUp { get; }
        }

        /// <summary>
        /// The RaiseLeg ActionRequest informs the Agent that we want it to raise 
        /// one of its legs.
        /// </summary>
        /// <remarks> 
        /// If the requested pose cannot be achieved (e.g., already in this pose, 
        /// Agent is walking, requested to lift both legs) the request will be declined.
        /// </remarks>
        public class RaiseLeg : PoseActionRequest
        {
            /// <summary>
            /// The constructor of the RaiseLeg ActionRequest
            /// </summary>
            /// <param name="observer">The object to be informed about status changes.
            /// Can be null if no observer is needed.</param>
            /// <param name="side">Which leg should be affected.</param>
            /// <param name="reset">If true the agent will first reset the leg pose and then 
            /// raise the leg. This is meant to avoid the 'flying' phase where the agent start lifting
            /// one foot before the other reached the ground.</param>
            public RaiseLeg(IActionRequestStateObserver observer, BodySide side, bool reset = false)
                : base(observer)
            {
                Side = side;
                AutoResetLegPose = reset;
            }

            /// ...
            public BodySide Side { get; }
            public bool AutoResetLegPose { get; }
        }

        /// <summary>
        /// The OpenMouth ActionRequest informs the Agent that we want it to open the mouth.
        /// </summary>
        public class OpenMouth : PoseActionRequest
        {
            /// <summary>
            /// The constructor of the OpenMouth ActionRequest.
            /// </summary>
            /// <param name="observer">The object to be informed about status changes.
            /// Can be null if no observer is needed.</param>
            public OpenMouth(IActionRequestStateObserver observer)
                : base(observer)
            {
            }
        }

        /// <summary>
        /// The CloseMouth ActionRequest informs the Agent that we want it to close the mouth.
        /// </summary>
        public class CloseMouth : PoseActionRequest
        {
            /// <summary>
            /// The constructor of the CloseMouth ActionRequest.
            /// </summary>
            /// <param name="observer">The object to be informed about status changes.
            /// Can be null if no observer is needed.</param>
            public CloseMouth(IActionRequestStateObserver observer)
                : base(observer)
            {
            }
        }

        /// <summary>
        /// The BendForward ActionRequest instructs the Agent to bend forward. 
        /// Use ResetBodyPose to unbent.
        /// </summary>
        public class BendForward : PoseActionRequest
        {
            /// <summary>
            /// The constructor of the BendForward ActionRequest.
            /// </summary>
            /// <param name="observer">The object to be informed about status changes.
            /// Can be null if no observer is needed.</param>
            public BendForward(IActionRequestStateObserver observer)
                : base(observer)
            {
            }
        }

        /// <summary>
        /// The MoveButtocks ActionRequest instructs the Agent to grasp & spread or release its buttocks. 
        /// ResetBodyPose, RaiseHand and ResetHandPose releases the grasp.
        /// </summary>
        public class MoveButtocks : PoseActionRequest
        {
            /// <summary>
            /// The constructor of the MoveButtocks ActionRequest.
            /// </summary>
            /// <param name="spread">If requested to spread or release the buttocks.</param>
            /// <param name="autoBend"> If set the agent will ensure the correct bend and hand state.</param>
            /// <param name="observer">The object to be informed about status changes.
            /// Can be null if no observer is needed.</param>
            public MoveButtocks(IActionRequestStateObserver observer, bool spread, bool autoBend = true)
                : base(observer)
            {
                Spread = spread;
                AutoBend = autoBend;
            }

            // ...
            public bool Spread { get; }
            public bool AutoBend { get; }
        }

        /// <summary>
        /// The WalkHome ActionRequest instructs the Agent to go back to its Home position. 
        /// </summary>
        /// <remarks>
        /// The request will be declined if there is no (reachable) home position in the scene.
        /// The home position is an object in the scene with a component of the type CustomPropertyList 
        /// attached, where the Tags property contains CustomTagProperty.HomePosition.
        /// </remarks>
        public class WalkHome : ActionRequest
        {
            /// <summary>
            /// The constructor of the WalkHome ActionRequest
            /// </summary>
            /// <param name="observer">The object to be informed about status changes.
            /// Can be null if no observer is needed.</param>
            public WalkHome(IActionRequestStateObserver observer)
                : base(observer)
            {
            }
        }

        /// <summary>
        /// TODO: dv: document me 
        /// </summary>
        public class Turn : ActionRequest
        {
            public enum TurnType
            {
                User,
                Transform,
                ExplicitAngle
            }

            /// <summary>
            /// TODO: dv: document me 
            /// </summary>
            public Turn(IActionRequestStateObserver observer, bool invert = false, bool autoAdjustPose = true)
                : base(observer)
            {
                Type = TurnType.User;
                Invert = invert;
                AutoAdjustPose = autoAdjustPose;
            }

            /// <summary>
            /// TODO: dv: document me 
            /// </summary>
            public Turn(IActionRequestStateObserver observer, Transform target, bool invert = false, bool autoAdjustPose = true)
                : base(observer)
            {
                Type = TurnType.Transform;
                Target = target;
                Invert = invert;
                AutoAdjustPose = autoAdjustPose;
            }

            /// <summary>
            /// TODO: dv: document me 
            /// </summary>
            public Turn(IActionRequestStateObserver observer, float degrees, bool autoAdjustPose = true)
                : base(observer)
            {
                Type = TurnType.ExplicitAngle;
                Angle = degrees; 
                AutoAdjustPose = autoAdjustPose;
            }

            public Transform Target { get; }
            public bool Invert { get; protected set; }
            public float Angle { get; }
            public TurnType Type { get; protected set; }
            public bool AutoAdjustPose { get; protected set; }
        }

        /// <summary>
        /// The ChangeChothes ActionRequest instructs the Agent to dress/undress. 
        /// </summary>
        /// <remarks>
        /// The request will be declined if the Agent is already in the same ClothingState
        /// </remarks>
        public class ChangeClothes : ActionRequest
        {
            /// <summary>
            /// The constructor of the ChangeClothes ActionRequest
            /// </summary>
            /// <param name="observer">The object to be informed about status changes.
            /// Can be null if no observer is needed.</param>
            /// <param name="clothing">The requested clothing state.</param>
            public ChangeClothes(IActionRequestStateObserver observer, ClothingState clothing)
                : base(observer)
            {
                Clothing = clothing;
            }

            /// ...
            public ClothingState Clothing { get; }
        }

        // sit(where)
        // raise
        // lay(where)
    }
}
