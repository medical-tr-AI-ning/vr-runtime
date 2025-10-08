using UnityEngine;
using Valve.VR.InteractionSystem;

namespace MedicalTraining.Logger {

    [RequireComponent( typeof( Valve.VR.InteractionSystem.Interactable ), typeof(LinearMapping), typeof(Rigidbody) )]
    public class InteractableLinearDriveLogger : InteractableLogger
    {
        #region variables
        // Options
        [SerializeField] private float _linearMappingChangeThreshold = 0.0001f;

        // References
        private LinearMapping _linearMapping;

        // Status Variables
        private bool _handRecentlyDetached = false;
        private float _lastLinearMappingvalue;
        #endregion

        #region unity_functions
        protected override void Start()
        {
            base.Start();
            _linearMapping = GetComponent<LinearMapping>();
            _lastLinearMappingvalue = _linearMapping.value;
        }

        private void Update()
        {
            if (_handRecentlyDetached && Mathf.Abs(_linearMapping.value - _lastLinearMappingvalue) < _linearMappingChangeThreshold)
            {
                _handRecentlyDetached = false;

                float value = _linearMapping.value;
                if (logger == null)
                {
                    Debug.LogWarning("Logger not set", this);
                    return;
                }
                else
                { 
                    logger.WriteEvent(name, "moved to", value.ToString());
                }
            }
            _lastLinearMappingvalue = _linearMapping.value;
        }
        #endregion

        override protected void OnDetachedFromHand(Hand hand)
        {
            // Call parent method
            base.OnDetachedFromHand(hand);
            _handRecentlyDetached = true;
        }
    }
}
