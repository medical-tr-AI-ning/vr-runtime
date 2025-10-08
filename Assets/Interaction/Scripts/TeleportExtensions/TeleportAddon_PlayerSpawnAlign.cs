using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    /// <summary>
    /// This addon waits until the player has placed the headset on their head and aligns his position and rotation with a given SpawnPoint in the scene
    /// </summary>
    public class TeleportAddon_PlayerSpawnAlign : MonoBehaviour
    {
        public Transform PlayerSpawnPoint;

        private bool _calibrated = false;
        
        void Start()
        {
            if (!PlayerSpawnPoint)
            {
                Debug.Assert(PlayerSpawnPoint, "Assertion Error: No Player Spawn Point specified", this);
            }
            HMDEventSource.instance?.HeadsetOnHeadStay.AddListener(OnHeadsetOnHead);
        }

        private void OnDestroy()
        {
            HMDEventSource.instance?.HeadsetOnHeadStay.RemoveListener(OnHeadsetOnHead);
        }

        private void OnHeadsetOnHead()
        {
            calibratePlayerPosRot();            
        }

        private void calibratePlayerPosRot()
        {
            if (_calibrated) return;
            if (!PlayerSpawnPoint) return;

            // Align player view direction with PlayerSpawn forward direction
            Quaternion playerSpawnLookRotation = Quaternion.LookRotation(PlayerSpawnPoint.forward, Vector3.up);
            Quaternion playerLookRotation = Quaternion.LookRotation(Player.instance.bodyDirectionGuess, Vector3.up);
            Quaternion rotationDifference = playerSpawnLookRotation * Quaternion.Inverse(playerLookRotation);
            Player.instance.trackingOriginTransform.rotation = rotationDifference * Player.instance.trackingOriginTransform.rotation;

            // Move player position to PlayerSpawn position
            Vector3 playerSpawnPosition = PlayerSpawnPoint.position;
            Vector3 playerHMDPosition = Player.instance.hmdTransform.position;
            Player.instance.trackingOriginTransform.position += new Vector3(playerHMDPosition.x - playerSpawnPosition.x, 0f, playerHMDPosition.z - playerSpawnPosition.z) * -1;

            _calibrated = true;
        }
    }
}