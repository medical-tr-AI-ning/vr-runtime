using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{

    public class RotateAwayFromPlayer : MonoBehaviour
    {

        private Player player;
        // Start is called before the first frame update
        void Start()
        {
            player = Player.instance;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            transform.LookAt(new Vector3(player.leftHand.transform.position.x,transform.position.y,player.leftHand.transform.position.z));

            
            transform.Rotate(0,-90,0);
        }
    }
}
