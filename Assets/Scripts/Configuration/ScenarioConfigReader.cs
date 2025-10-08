using System.Collections.Generic;

using UnityEngine;

using System.Linq;
using MedicalTraining.Utils;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using static medicaltraining.assetstore.ScenarioConfiguration.Serialization.VariableScenarioConfig;
using System.Windows.Forms.VisualStyles;


namespace MedicalTraining.Configuration
{
    public class ScenarioConfigReader : MonoBehaviour
    {
        private ConfigurationContainer m_config;

        private void Awake()
        {
            this.m_config = ConfigurationContainer.Instance;
        }

        public ScenarioVariant PrepareScenarioConfig(VariableScenarioConfig variableScenario, string scenarioConfigDir)
        {
            this.m_config.SetScenarioConfigDir(scenarioConfigDir);
            ScenarioVariant scenario = SelectVariantProbabilistic(variableScenario.Variants);
            this.m_config.SetVariableScenario(variableScenario);
            this.m_config.SetScenario(scenario);
            PathologyVariant pathology = ImportPathologyVariant(scenario);
            this.m_config.SetPathology(pathology);
            this.m_config.SetScenarioSpecificSettings(variableScenario.ScenarioSpecificSettings);
            return scenario;
        }

        private ScenarioVariant SelectVariantProbabilistic(List<ScenarioVariant> variants)
        {
            float totalProbability = variants.Sum(v => v.Probability);
            float randomValue = UnityEngine.Random.Range(0.0f, totalProbability);

            float currentProbability = 0.0f;
            foreach (ScenarioVariant variant in variants)
            {
                currentProbability += variant.Probability;
                if (randomValue <= currentProbability)
                {
                    return variant;
                }
            }

            return null;
        }

        private PathologyVariant ImportPathologyVariant(ScenarioVariant scenario)
        {
            return scenario.Pathology.Deserialize(this.m_config.PathologyPath);
        }
    }
}
