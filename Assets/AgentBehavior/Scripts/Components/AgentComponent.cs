using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MedicalTraining.Utilities;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class AgentComponent : CustomPropertyList
        {
            //Settings
            [SerializeField] protected string AgentIdentifier;
            [SerializeField] protected Transform Head;
            [SerializeField] protected Transform Torso;

            // <debug
            [SerializeField] private bool ArmatureHandlesVisible = false;
            [ReadOnly, SerializeField] private List<ArmatureHandleType> AvailableArmatureHandles;
            // debug>
            // ...
            
            public LookAtSettings LookAtSettings;
            public BlinkSettings BlinkSettings;
            public LipSyncSettings LipSyncSettings;

            // ...
            public virtual string Identifier => AgentIdentifier;
            public virtual Transform HeadTransform => Head;
            public virtual Transform TorsoTransform => Torso;
            public virtual Transform RootTransform => transform;

            public virtual IArmatureHandle GetArmatureHandle(ArmatureHandleType type)
            {
                // in case someone wants a handle before Start was called
                InitializeArmatureHandles(); 

                // try get the handle, return null if none
                ArmatureHandles.TryGetValue(type, out IArmatureHandle handle);
                return handle;
            }

            protected override void Start()
            {
                // ensure proper tags
                base.Start();
                EnsureTag(CustomTagProperty.Agent);

                // initialize the armature handles
                InitializeArmatureHandles();

                // initialize the lip-sync components
                InitializeLipSync();

                // <debug
                AvailableArmatureHandles = ArmatureHandles.Keys.ToList();
                // debug>
            }

            protected override void Update()
            {
                base.Update();

                // <hack dv:
                // For some reason uLipSyncAudioSource blocks the output of our speech system if it's enabled on start.
                // It work with "normal" audio sources through. Weird :/
                // Disabled on start and re-enabling here does the trick (even more weird).
                if (LipSyncSettings)
                {
                    if (LipSyncSettings.AutoEnable && !LipSyncSettings.Enabled)
                        LipSyncSettings.Enabled = true;
                }
                // hack>
            }

            // ...
            protected Dictionary<ArmatureHandleType, IArmatureHandle> ArmatureHandles = new();

            // <hack
            private bool _armatureInitialized = false;
            private void InitializeArmatureHandles()
            {
                if (_armatureInitialized) 
                    return;

                // query all armature handles
                IArmatureHandle[] handles = GetComponentsInChildren<IArmatureHandle>();
                foreach (IArmatureHandle handle in handles)
                {
                    if (handle is ArmatureHandle ah)
                        ah.ArmatureHandlesVisible = ArmatureHandlesVisible;

                    if (handle != null && handle.HandleType == ArmatureHandleType.LookAt)
                        continue;

                    if (handle == null || !ArmatureHandles.TryAdd(handle.HandleType, handle))
                    {
                        Debug.LogWarning("[Behavior] Armature handles not properly configured for " +
                            $"agent {Identifier} in GameObject {gameObject.name}. " +
                            (handle == null ? "Handle is 'null'" : $"Handle {handle.HandleType} already exists."));
                    }
                }

                _armatureInitialized = true;
            }
            // hack>

            private void InitializeLipSync()
            {
                if (LipSyncSettings)
                    return;

                if (!LipSyncSettings.LipSync)
                    LipSyncSettings.LipSync = GetComponent<uLipSync.uLipSync>();

                if (!LipSyncSettings.LipSyncAudioSourceProxy)
                    LipSyncSettings.LipSyncAudioSourceProxy = GetComponentInChildren<uLipSync.uLipSyncAudioSource>();

                if (LipSyncSettings)
                    LipSyncSettings.LipSync.audioSourceProxy = LipSyncSettings.LipSyncAudioSourceProxy;
            }
        }

        // just some help to keep the inspector a bit cleaner
        [System.Serializable]
        public class LookAtSettings
        {
            [Tooltip("Time (seconds) to switch the look-at target.")]
            public ValueRange TimeInterval = new(2.0f, 5.0f);

            [Tooltip("Up/Down, Left/Right, Tilt; Max rotation is +/- FoV/2")]
            public Vector3 FoV = new (60.0f, 120.0f, 0.0f);

            [Tooltip("The look-at is off if the target is too close or too far away. Distance in meters")]
            public ValueRange Distance = new(0.45f, 3.0f);

            [Range(0f, 1f)]
            public float ChanceLookingNowhere = 0.2f;

            [Range(0f, 1f)]
            public float ChanceRepeatTarget = 0.1f;

            [Tooltip("How to look at the user :)")]
            public UserLookAtSettings User;
        }

        [System.Serializable]
        public class UserLookAtSettings
        {
            [Tooltip("How fast an object becomes uninteresting")]
            public ValueRange InterestDecayRate = new(6.0f, 12.0f);
        }

        // ...
        [System.Serializable]
        public class BlinkSettings
        {
            public BlendShapeAnimation BlinkAnimation;
            public ValueRange TimeInterval = new (60.0f / 20.0f, 60.0f / 14.0f); // 14 - 20 blinks per minute
            [Range(0.0f, 1.0f)] public float ChanceSkipBlink = 0.02f;
            [Range(0.0f, 1.0f)] public float ChanceEarlyBlink = 0.02f;
        }

        // ...
        [System.Serializable]
        public class LipSyncSettings
        {
            //public AudioSource AudioSource;
            public uLipSync.uLipSyncAudioSource LipSyncAudioSourceProxy;
            public uLipSync.uLipSync LipSync;
            public bool AutoEnable = true;

            public static implicit operator bool(LipSyncSettings me)
            {
                return (me is not null) && (me.LipSyncAudioSourceProxy) && (me.LipSync);
            }

            public bool Enabled
            {
                get => LipSyncAudioSourceProxy.enabled;
                set => LipSyncAudioSourceProxy.enabled = value;
            }
        }
    }
}
