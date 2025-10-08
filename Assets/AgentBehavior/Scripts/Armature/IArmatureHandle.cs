using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {        
        /// <summary>
        /// Body part controlled by the armature handle
        /// </summary>
        public enum ArmatureHandleType
        {
            Invalid,

            LookAt,

            RightHand,
            RightTumb,
            RightIndex,
            RightMiddle,
            RightRing,
            RightPinky,

            LeftHand,
            LeftTumb,
            LeftIndex,
            LeftMiddle,
            LeftRing,
            LeftPinky,

            LeftForearm,
            RightForearm,

            RightFoot,
            RightBigToe,
            RightIndexToe,
            RightMiddleToe,
            RightRingToe,
            RightPinkyToe,

            LeftFoot,
            LeftBigToe,
            LeftIndexToe,
            LeftMiddleToe,
            LeftRingToe,
            LeftPinkyToe,

            Scrotum,
            Penis,

            LeftEar,
            RightEar
        }

        /// <summary>
        /// The base interface to allow the user to manipulate some body part of the agent.
        /// An armature handle is basically the target for the IK system when enabled, but 
        /// remains attached to the body part when disabled. 
        /// 
        /// Use <code>SetPosition</code> and <code>SetOrientation</code> to control the handle's 
        /// position directly or <code>SetTarget</code> to make it follow some other transform. 
        /// 
        /// The <code>time</code> parameter in the <code>Enable/Disable</code> methods allows
        /// or a smooth transition between the current and the new, procedurally calculated pose.
        /// </summary>
        /// <note>
        /// See IndirectAnchor and DirectAnchor components in the Example folder for example usage. 
        /// </note>
        public interface IArmatureHandle
        {
            /// <summary>
            /// Body part controlled by this handle.
            /// </summary>
            public abstract ArmatureHandleType HandleType { get; }

            /// <summary>
            /// True if the handle is enabled, false otherwise.
            /// </summary>
            public abstract bool Enabled { get; }

            /// <summary>
            /// True if the handle is in transition from Enabled to Disabled state or vice versa.
            /// </summary>
            public abstract bool Transient { get; }

            /// <summary>
            /// Enable the handle. 
            /// </summary>
            /// <param name="time">Time for the transition from the current pose to the new, 
            /// procedurally calculated pose. The property <code>Transient</code> will be true during the transition.</param>
            public abstract void Enable(float time);


            /// <summary>
            /// Disable the handle. This will also reset the target (set with <code>SetTarget</code>) in some cases. 
            /// </summary>
            /// <param name="time">Time for the transition from the current pose to the new, 
            /// procedurally calculated pose. The property <code>Transient</code> will be true during the transition.</param>
            public abstract void Disable(float time);

            /// <summary>
            /// Set the target transform to follow.
            /// </summary>
            /// 
            /// <example>
            /// The following code will make the IK target move from it's current position 
            /// to the position of the object <code>Anchor</code> in 1 second and will then follow the position 
            /// and the orientation of <code>Anchor</code>.
            /// <code>
            /// handle.SetTarget(Anchor.transform);
            /// handle.Enable(1.0f); 
            /// </code>
            /// </example>
            /// 
            /// <param name="target">Target transform to follow.</param>
            public abstract void SetTarget(Transform target);

            /// <summary>
            /// Set the position of the IK target directly.
            /// </summary>
            /// <param name="position">The new position of the IK target.</param>
            public abstract void SetPosition(Vector3 position);

            /// <summary>
            /// Set the orientation of the IK target directly. 
            /// </summary>
            /// <note>Some handles (e.g. the fingertips) may ignore the target's orientation.</note>
            /// <param name="rotation">The new orientation of the target.</param>
            public abstract void SetRotation(Quaternion rotation);
        }
    }
}
