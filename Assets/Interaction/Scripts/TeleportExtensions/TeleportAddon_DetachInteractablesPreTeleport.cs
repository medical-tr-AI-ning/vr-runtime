using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Valve.VR.InteractionSystem
{
    /// <summary>
    /// Teleport Addon which uses MonoBehaviours attached to GameObjects to identify the objects which need to get detached prior to teleporting.
    /// </summary>
    public class TeleportAddon_DetachInteractablesPreTeleport : MonoBehaviour
    {
        [SerializeField, ReadOnly] 
        private List<string> _types = new List<string>();

#if UNITY_EDITOR
        [Header("Add Scripts by dragging them here:")]
        // List of MonoScripts used to identify GameObjects which should be detached before teleporting player
        [SerializeField]
        private List<MonoScript> _monoScripts = new List<MonoScript>();

        // Create list of types from list of MonoScripts to allow this script to work at runtime
        private void OnValidate()
        {
            _monoScripts = _monoScripts.Distinct().ToList();
            _types.Clear();
            foreach(MonoScript script in _monoScripts)
            {
                if (!script) continue;
                _types.Add(script.GetClass().ToString());
            }
        }
#endif

        void Start()
        {
            // Add Listener to PlayerPre Event, which is thrown before the Player gets teleported
            Teleport.PlayerPre.AddListener(OnPlayerPreTeleport);
        }

        private void OnPlayerPreTeleport(TeleportMarkerBase markerBase)
        {
            Hand[] playerHands = Player.instance.hands;
            foreach(Hand hand in playerHands)
            {
                // Create a copy of all currently attached Objects for each hand and check if they need to get detached
                Hand.AttachedObject[] tmpAttachedObjects = new Hand.AttachedObject[hand.AttachedObjects.Count];
                hand.AttachedObjects.CopyTo(tmpAttachedObjects, 0);
                foreach (Hand.AttachedObject attachedObject in tmpAttachedObjects)
                {
                    foreach (string type in _types)
                    {
                        if (attachedObject.attachedObject.GetComponent(System.Type.GetType(type)))
                        {
                            hand.DetachObject(attachedObject.attachedObject);
                        }
                    }
                }
            }
        }
    }
}
