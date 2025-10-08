using System;
using MedicalTraining.Configuration;
using TMPro;
using UnityEngine;

namespace MedicalTraining.Dialogue
{
    public class PatientCard: MonoBehaviour
    {
        [SerializeField] private TMP_Text ageText;
        [SerializeField] private TMP_Text occupationText;
        [SerializeField] private TMP_Text weightText;
        [SerializeField] private TMP_Text heightText;
        
        public void UpdateText()
        {
            var agentInfo = ConfigurationContainer.Instance.Pathology.Agent;
            ageText.text = agentInfo.Age;
            occupationText.text = agentInfo.Occupation;
            weightText.text = agentInfo.Weight;
            heightText.text = agentInfo.Height;
        }

        public void Start()
        {
            UpdateText();
        }
    }
    
    
}