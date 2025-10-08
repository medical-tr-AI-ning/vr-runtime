using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Valve.VR.InteractionSystem
{

    [RequireComponent( typeof( Interactable ) )]
    public class PlaySoundViaObjectSpeed : MonoBehaviour
    {
        private Vector3 _lastPosition;
        private AudioSource _audioSrc;
        [SerializeField] private float _dampeningFactor = 1;
        [SerializeField] private Transform _start;
        [SerializeField] private Transform _end;
        private AudioSource _audStart;
        private AudioSource _audEnd;
        private Vector3 _velocity;

        private bool _wasAtStart;
        private bool _wasAtEnd;
        protected Interactable interactable;
        private bool _isPlaying = true;

        void Start()
        {
            _lastPosition = transform.position;
            _audioSrc = GetComponent<AudioSource>();
            _audStart = _start.GetComponent<AudioSource>();
            _audEnd = _end.GetComponent<AudioSource>();
            interactable = GetComponent<Interactable>();

        }

        void Update()
        {
            if(_isPlaying)
            {
                _velocity = (transform.position - _lastPosition) / Time.deltaTime;
                CheckStartEnd();
                _lastPosition = transform.position;
                _audioSrc.volume = _velocity.magnitude;

                if(_velocity.magnitude == 0f)
                {
                    _isPlaying = false;
                }
                
            }
        }

        private void CheckStartEnd()
        {
            if(transform.position == _start.position)
            {
                if(!_wasAtStart)
                {
                    _audStart.volume = _velocity.magnitude;
                    _audStart.Play();
                }
                _wasAtStart = true;
            }
            else
            {
                _wasAtStart = false;
            }
    
            if(transform.position == _end.position)
            {
                if(!_wasAtEnd)
                {
                    _audEnd.volume = _velocity.magnitude; 
                    _audEnd.Play();
                }
                _wasAtEnd = true;
            }
            else
            {
                _wasAtEnd = false;
            }
        }

        protected virtual void HandAttachedUpdate(Hand hand)
        {
            _isPlaying = true;
        }
    }
}
