using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.Configuration;

namespace MedicalTraining.Logger {
    public class TriggerLogger : MonoBehaviour
    {
        [SerializeField]
        private new string name;
        [SerializeField]
        private string[] tags;
        private SimulationLogger logger;
        private bool entered = false;
        void Start()
        {
            logger = ConfigurationContainer.Instance.GetLogger();
        }
    
        void OnTriggerEnter (Collider other)
        {
            if(entered == true){
                return;
            }

            foreach (string tag in tags)
            {
                if (other.gameObject.CompareTag(tag))
                {
                    logger.WriteEvent(name, "was triggered", "");
                    entered = true;
                    break;
                }
            }
        }

        void OnTriggerExit (Collider other)
        {
            foreach (string tag in tags)
            {
                if (other.gameObject.CompareTag(tag))
                {   
                    entered = false;
                    break;
                }
            }
        }
    }
}
