using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class FingerDisplacement : MonoBehaviour 
{
		public Vector3 displacement;
		public LinearMapping linearMapping;

		private Vector3 initialPosition;

		//-------------------------------------------------
		void Start()
		{
			initialPosition = transform.localPosition;

			if ( linearMapping == null )
			{
				linearMapping = GetComponent<LinearMapping>();
			}
		}

    void Update()
        {
            if ( linearMapping )
            {
                float v = linearMapping.value;
                v = Mathf.Clamp(Mathf.Abs((v - 0.5f) * 2), 0f, 1f);
                transform.localPosition = initialPosition + v * displacement;
            }
        }
}
