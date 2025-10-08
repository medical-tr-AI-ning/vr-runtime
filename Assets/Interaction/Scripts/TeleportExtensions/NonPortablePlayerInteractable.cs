using UnityEngine;

/// <summary>
/// This script can be attached to non portable objects which are player interactable.
/// These objects get automatically detached from the player's hand before teleporting.
/// 
/// Note:
/// Place on the same GameObject as "Interactable" script.
/// </summary>
namespace Valve.VR.InteractionSystem {
    public class NonPortablePlayerInteractable : MonoBehaviour
    {
        private void OnValidate()
        {
            Debug.Assert(GetComponent<Interactable>(), "Assertion failed: This script can only be placed on GameObjects which also contain a SteamVR Interactable component.", this);
        }
    }
}