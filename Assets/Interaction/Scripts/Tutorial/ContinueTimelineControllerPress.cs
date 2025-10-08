using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Valve.VR.InteractionSystem
{
    public class ContinueTimelineControllerPress : MonoBehaviour
    {
        [SerializeField] private SteamVR_Action_Boolean actionObject;
        [SerializeField] private GameObject timeline;

        private PlayableDirector director;

        [SerializeField] private bool once = true;

        private bool played = false;

        void Start()
        {
            director = timeline.GetComponent<PlayableDirector>();
        }

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
                    director.Play();
                    played = true;
                    Debug.Log(played);
                }
            }

            else
            {
                director.Play();
                Debug.Log(played);
            }
            Debug.Log("Signal gesetzt" + gameObject.name);

        }
    }
}