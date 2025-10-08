using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class ChangingRoomComponent : CustomPropertyList, IDoor
        {
            // Inspector
            [SerializeField] private Transform WalkToTarget;
            [SerializeField] private Transform WalkInTarget;

            [SerializeField] private Transform HandleOutside;
            [SerializeField] private Transform HandleInside;

            // Interface
            public virtual Transform GraspTargetOut => HandleOutside;
            public virtual Transform GraspTargetIn => HandleInside;
            public virtual Transform WalkTargetOut => WalkToTarget;
            public virtual Transform WalkTargetIn => WalkInTarget;
            public virtual float RelativePosition { get; set; } // TODO: dv: public set is a hack!

            public virtual void Open()
            {
                if (_animator)
                    _animator.SetBool(_openAnimationID, true);
            }
            
            public virtual void Close()
            {
                if (_animator) 
                    _animator.SetBool(_openAnimationID, false);
            }

            //public bool ClothesVisible { get; set; }
            //public virtual bool PatientInside { get; private set; }

            protected override void Start()
            {
                base.Start();
                EnsureTag(CustomTagProperty.ChangingRoom);

                RelativePosition = 0.0f;
                _openAnimationID = Animator.StringToHash("open");

                _animator = GetComponentInChildren<Animator>();
                if (_animator)
                    foreach (var behavior in _animator.GetBehaviours<CurtainPositionBehavior>())
                        behavior.Door = this;
            }

            private Animator _animator = null;
            private int _openAnimationID = 0;
        }
    }
}
