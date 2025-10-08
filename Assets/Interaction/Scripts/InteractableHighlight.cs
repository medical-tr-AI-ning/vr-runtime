using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Valve.VR.InteractionSystem
{
    public class InteractableHighlight : MonoBehaviour
    {
        [SerializeField] private SteamVR_Action_Boolean actionObject;

        private GameObject[] highlights;

        [SerializeField] private new string tag = "Highlight";

        [SerializeField] private GameObject target;

        [SerializeField] private bool limitRot = true;

        [SerializeField] private bool highlightTest = true;


        void Awake()
        {
            highlights = GameObject.FindGameObjectsWithTag(tag);
        }

        void Update()
        {
            if (actionObject.GetState(SteamVR_Input_Sources.RightHand) || actionObject.GetState(SteamVR_Input_Sources.LeftHand) || highlightTest)
            {
                On();
            }
            else
            {
                Off();
            }
        }


        private void On()
        {
            foreach (GameObject highlight in highlights)
            {
                highlight.SetActive(true);
                highlight.transform.LookAt(target.transform);
                if (limitRot)
                {
                    highlight.transform.transform.eulerAngles = new Vector3(0, highlight.transform.eulerAngles.y, 0);
                }
            }
        }

        private void Off()
        {
            foreach (GameObject highlight in highlights)
            {
                highlight.SetActive(false);
            }
        }
    }
}
