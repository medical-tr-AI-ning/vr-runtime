// Ignore Spelling: Collider Overwear
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class PatientAgentComponent : AgentComponent
        {
            // Inspector Settings
            public GraspColliders GraspColliders;
            public MovementSettings MovementSettings;

            // clothing and hair (default implementation)
            [SerializeField] protected ModelSettings ModelSettings;

            // Interface 
            public void ChangeHairVisibility(bool hairVisible)
            {
                foreach (var hairNode in ModelSettings.HairNodes)
                {
                    hairNode.SetActive(hairVisible);
                }
            }

            public void ChangeClothing(ClothingState state)
            {
                foreach (var genitalsNode in ModelSettings.GenitalsNodes) { 
                    genitalsNode.SetActive( state == ClothingState.Naked);
                }
                foreach (var underwearNode in ModelSettings.UnderwearNodes) { 
                    underwearNode.SetActive(state == ClothingState.Underwear);
                }
                foreach (var overwearNode in ModelSettings.OverwearNodes) { 
                    overwearNode.SetActive( state == ClothingState.Dressed);
                }
            }

            protected override void Start()
            {
                // ensure proper tagging
                base.Start();
                EnsureTag(CustomTagProperty.InteractiveAgent);
                EnsureTag(CustomTagProperty.Patient);

                // TODO: dv: (low priority) auto-detect clothing and genitals nodes?
                // TODO: dv: (low priority) auto-detect hair nodes?
            }

            // <hack 01.1
            // TODO: dv: the mix of (Patient)AgentComponent and (Patient)MotionController
            // gets progressively messy and weird... need to really think about refactoring.
            [HideInInspector] public UnityEvent<ControllerColliderHit> ControllerColliderHit;
            protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
                => ControllerColliderHit?.Invoke(hit);

            [HideInInspector] public UnityEvent<Collider> TriggerEnter;
            protected virtual void OnTriggerEnter(Collider collider)
                => TriggerEnter?.Invoke(collider);

            [HideInInspector] public UnityEvent<Collider> TriggerExit;
            protected virtual void OnTriggerExit(Collider collider)
                => TriggerExit?.Invoke(collider);
            // hack>
        }

        // just some help to keep the inspector a bit cleaner
        [System.Serializable]
        public class GraspColliders
        {
            public Collider Left;
            public Collider Right;
        }

        // ...
        [System.Serializable]
        public class MovementSettings
        {
            public float BaseSpeed = 0.92f;
            public float HeightOffset = 0.025f;
            [ReadOnly] public float TargetReachedDelta = 0.1f;
            [ReadOnly] public readonly float Gravity = 1.0f;
        }

        // ...
        [System.Serializable]
        public class ModelSettings
        {
            public GameObject[] GenitalsNodes;
            public GameObject[] UnderwearNodes;
            public GameObject[] OverwearNodes;
            public GameObject[] HairNodes;
        }
    }
}
