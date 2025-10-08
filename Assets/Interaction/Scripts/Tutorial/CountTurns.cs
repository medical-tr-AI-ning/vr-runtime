using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class CountTurns : MonoBehaviour
    {
        [SerializeField] private GameObject obj;

        private int count = 0;
        [SerializeField] private int numberOfActions = 2;

        public void TurnRight()
        {
            count++;
            Debug.Log(count);
        }

        public void TurnLeft()
        {
            count--;
            Debug.Log(count);
        }

        void Update()
        {
            if (count >= numberOfActions || count * -1 >= numberOfActions)
            {
                obj.SetActive(true);
            }
        }
    }
}

