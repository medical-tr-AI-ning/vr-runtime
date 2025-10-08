using medicaltraining.assetstore.ScenarioConfiguration;
using System.Collections.Generic;
using UnityEngine;


namespace MedicalTraining.Configuration
{
    public abstract class ScenarioConfigurator : MonoBehaviour
    {

        [SerializeField] public GameObject SceneRoot;
        [SerializeField] public GameObject PatientsRoot;
        [SerializeField] public GameObject ObjectsRoot;

        [SerializeField] private List<PrefabMapping> AvailablePrefabs;
        [SerializeField] private List<PrefabMapping> AvailableAgents;

        [Header("Debug")]
        [SerializeField, ReadOnly] protected ConfigurationContainer m_config;
        [SerializeField, ReadOnly] protected DialogueController m_dialogueController;

        [SerializeField, ReadOnly] protected GameObject m_agent;

        protected virtual void Awake()
        {
            this.m_config = ConfigurationContainer.Instance;
            Debug.Log("ScenarioConfigurator Awake");
            this.m_agent = GetAgent();
            this.m_config.SetAgent(this.m_agent);
        }

        protected virtual void Start()
        {
            this.m_dialogueController = DialogueController.Instance;
            if (this.m_dialogueController == null)
            {
                Debug.LogError("DialogueController not found!");
            }
            this.m_dialogueController.SetNewAgent(this.m_agent);
        }

        public void ConfigureScenario()
        {
            // Place objects in the scene
            this.PlaceObjects();

            // Do scenario specific configuration (e.g. apply textures, set up post processing)
            this.ConfigureScenarioSpecific();
        }

        public abstract void ConfigureScenarioSpecific();

        private GameObject FindPrefab(string prefabID)
        {
            foreach (var prefabMapping in this.AvailablePrefabs)
            {
                if (prefabMapping.prefabID == prefabID)
                {
                    return prefabMapping.prefab;
                }
            }
            return null;
        }

        protected void PlaceObjects()
        {
            foreach (var objectPlacement in this.m_config.Scenario.Environment.ObjectPlacements)
            {
                GameObject prefab = this.FindPrefab(objectPlacement.PrefabID);
                GameObject obj = Instantiate(prefab, this.ObjectsRoot.transform);
                objectPlacement.SerializableTransform.ApplyPropertiesToTransform(obj.transform);
                obj.name = prefab.name;
                Debug.Log("Placed object: " + obj.name);
                // TODO: Individual object setup required?
                // Add to Logger
                this.m_config.GetLogger().TrackedObjects.Add(obj.transform);
            }
        }

        private GameObject FindAgent(string agentID)
        {
            foreach (var prefabMapping in this.AvailableAgents)
            {
                if (prefabMapping.prefabID == agentID)
                {
                    return prefabMapping.prefab;
                }
            }
            return null;
        }

        private GameObject CreateAgent(string agentID)
        {
            GameObject prefab = FindAgent(agentID);
            if (prefab == null)
            {
                Debug.LogError("Agent not found: " + agentID);
                return null;
            }
            // TODO: Specify spawn position and rotation
            GameObject agent = Instantiate(prefab, this.PatientsRoot.transform);
            agent.name = prefab.name;
            Debug.Log("Created agent: " + agent.name);
            // Add to Logger
            this.m_config.GetLogger().TrackedObjects.Add(agent.transform);
            return agent;
        }

        public GameObject GetAgent(string AgentID = null)
        {
            if (AgentID == null)
            {
                AgentID = this.m_config.Scenario.Agent.AgentID;
                Debug.Log("AgentID: " + AgentID);
            }
            // FIXME: dv: If starting the scenario directly the agentDescription will
            // also end up being null. Quick fix, but someone should probably look at it.
            if (AgentID == null)
                return null;

            if (this.m_agent == null)
            {
                this.m_agent = CreateAgent(AgentID);
            }
            return this.m_agent;
        }
    }
}
