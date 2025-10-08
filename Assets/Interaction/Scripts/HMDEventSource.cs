using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
    /// <summary>
    /// Event source for HMD related stuff
    /// </summary>
    // TODO: Needs to be replaced by a central VR hardware event system in the future
    // HACK
    public class HMDEventSource : MonoBehaviour
    {
        // Actions
        private SteamVR_Action_Boolean _headsetOnHeadAction;

        // States
        private bool _headsetCurrentlyOnHead = false;

        // Events
        [HideInInspector]
        public UnityEvent HeadsetOnHeadStart;
        [HideInInspector]
        public UnityEvent HeadsetOnHeadStay;
        [HideInInspector]
        public UnityEvent HeadsetOnHeadStop;

        private static HMDEventSource _instance;
        public static HMDEventSource instance {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<HMDEventSource>();
                    Debug.Assert(_instance, "Assertion Error: HMDEventSource instance is accessed, but no instance of the component was found in the scene");
                }

                return _instance;
            }
        }
        void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _headsetOnHeadAction = SteamVR_Input.GetBooleanAction("HeadsetOnHead");
            if (_headsetOnHeadAction == null)
            {
                Debug.LogAssertion("No SteamVR HeadsetOnHead Action found.", this);
            }
        }

        private void Update()
        {
            if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
                return;

            processHeadSetOnHead();
        }

        #region Event Invoker Functions
        private void processHeadSetOnHead()
        {
            if (_headsetOnHeadAction == null) return;

            bool proxSensorValue = _headsetOnHeadAction.GetState(SteamVR_Input_Sources.Head);

            if (proxSensorValue)
            {
                if (proxSensorValue != _headsetCurrentlyOnHead)
                {
                    // Headset newly placed on head
                    HeadsetOnHeadStart.Invoke();
                }
                else
                {
                    // Headset still on head
                    HeadsetOnHeadStay.Invoke();
                }
            }
            else
            {
                if (proxSensorValue != _headsetCurrentlyOnHead)
                {
                    // Headset removed from head
                    HeadsetOnHeadStop.Invoke();
                }
            }
            _headsetCurrentlyOnHead = proxSensorValue;
        }
        #endregion
    }
}
