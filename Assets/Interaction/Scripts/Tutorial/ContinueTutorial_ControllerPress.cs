using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
    public class ContinueTutorial_ControllerPress : MonoBehaviour
    {
        [SerializeField] private SteamVR_Action_Boolean actionObject;
        [SerializeField] private bool once = true;

        private bool played = false;
        public UnityEvent continueTutorial;

        void Update()
        {
            if (actionObject.GetState(SteamVR_Input_Sources.RightHand) || actionObject.GetState(SteamVR_Input_Sources.LeftHand))
            {
                ContinueTheTimelineOnButtonpress();
            }
        }


        public void ContinueTheTimelineOnButtonpress()
        {
            if (once)
            {
                if (!played)
                {
                    continueTutorial.Invoke();
                    played = true;
                }
            }

            else
            {
                continueTutorial.Invoke();
                Debug.Log(played);
            }

        }
    }
}