using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        // The motions are grouped by capabilities, and the motion controller 
        // implements these capabilities as mix-ins. This should allow more flexible
        // definition of motion controllers for the different agent roles.
        public interface ILookAtCapability
        {
            // target == null resets the look-at
            public abstract void SetLookAtTarget(Transform target);
            public abstract void SetLookAtTarget(Transform target, float blendSpeed);

            public virtual void SetLookAtTarget(GameObject target)
                => SetLookAtTarget(!target ? (Transform)null : target.transform);

            public virtual void SetLookAtTarget(GameObject target, float blendSpeed)
                => SetLookAtTarget(!target ? (Transform)null : target.transform, blendSpeed);
        }

        public interface IWalkCapability
        {
            public abstract void WalkTo(Transform target);//, bool ignoreTargetOrientation);
            //??? public abstract void WalkBy(Vector3 distance); 
            //??? public virtual bool WalkPath(IEnumerable<Vector3> path) { return false; } 
            public abstract bool Walking { get; }
            public abstract bool WalkTargetReached { get; }
        }

        public interface ITurnCapability
        {
            public abstract void AlignTo(Transform target);//, bool invert = false); 
            public abstract void TurnTo(Transform target, bool invert);
            public abstract void TurnBy(float degrees);
            public abstract bool Turning { get; }
            //??? public abstract bool OrientationTargetReached { get; }
        }

        public interface IGraspCapability
        {
            public abstract bool Grasp(Transform target);
            public abstract void Release();

            public abstract bool Grasped { get; }
            public abstract bool Released { get; }
            public abstract bool GraspTargetReachable { get; }

            // grasping state is part of the capability
            public abstract GraspingState GraspingState { get; set; }
            //public abstract Transform GraspTarget { get; }
        }

        public interface IChangeClothingCapability
        {
            public abstract void ChangeClothing(ClothingState clothingState);
            public abstract bool ClothingChanged { get; }

            // clothing state is part of the capability
            public abstract ClothingState Clothing { get; }
        }

        // currently we need to walk to the changing room, grasp and open  
        // the door and turn around in order to change clothes...
        // 
        // NOTE: dv: We can later implement different variants of this.
        //           How do we control the action sequences? 
        public interface IUseChangingRoomCapability : IChangeClothingCapability,
            IWalkCapability, ITurnCapability, IGraspCapability
        {
        }

        public interface IPoseCapability
        {
            // returns false if the pose cannot be updated
            // (the pose is not supported, the pose does not make sense, etc.)
            public abstract bool UpdatePose(BodyPart part, PoseState state, Reason failReason = null);
            public abstract bool PoseUpdated { get; }
            public abstract bool IsInTransientPose(int layer); 
            public abstract void SetInTransientPose(int layer, bool transient);

            // Agent's pose state is part of the capability
            public abstract PoseDescription Pose { get; }
        }
    }
}
