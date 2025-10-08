using MedicalTraining.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class LookAtController
        {
            public LookAtController(AgentComponent agent) 
            {
                _agent = agent;
                _valid = _agent && _agent.HeadTransform && _agent.TorsoTransform;

                if (_valid)
                {
                    float hfov2 = 0.5f * (_agent.LookAtSettings.FoV.y);
                    float vfov2 = 0.5f * (_agent.LookAtSettings.FoV.x);
                    DistanceRange = _agent.LookAtSettings.Distance;
                    HorizontalFoV = new ValueRange(-hfov2, hfov2, ValueRange.ValueType.Angle);
                    VerticalFoV = new ValueRange(-vfov2, vfov2, ValueRange.ValueType.Angle);
                }
            }

            //...
            public static implicit operator bool(LookAtController component)
                => component is not null && component._valid;

            public virtual bool Transient 
                => _weight < 1.0f; 

            public virtual bool Enabled 
            { 
                get => _enabled;
                set
                {
                    if (!_valid)
                    {
                        _enabled = false;
                        return;
                    }

                    if (value && !_enabled) // switch on
                    {
                        (_currentTarget, _previousTarget) = (_previousTarget, _currentTarget);
                        _currentTarget = _currentTarget && Visible(_currentTarget) ? _currentTarget : null;
                        _weight = 0.0f;
                    }

                    else if (!value && _enabled) // switch off
                        SetTarget(null);

                    _enabled = value;                   
                }
            }

            public virtual void Update(IPerceptionModel model)
            {
                if (!_valid) 
                    return;

                if (Enabled && CheckTargetSwitch()) 
                    SwitchTarget(model);

                UpdateHeadRotation();
            }

            // ...
            private void SetTarget(Transform target)
            {
                if (_currentTarget == target)
                    return;

                if (!Enabled)
                {
                    _previousTarget = target;
                }
                else
                {
                    _previousTarget = _currentTarget;
                    _currentTarget = target;
                    _weight = 0.0f;
                }
            }

            private bool CheckTargetSwitch()
            {
                if (!Enabled || Transient)
                    return false;

                float time = Time.time;
                bool timeToSwitch = time > _triggerTime;
                bool targetInvisible = _currentTarget && !Visible(_currentTarget); // not the right time to switch, but the
                                                                                   // target become invisible

                if (!targetInvisible && !timeToSwitch)
                    return false;

                // definitely switch -> set the new trigger time
                _triggerTime = time + MathUtilities.RandInRange(_agent.LookAtSettings.TimeInterval);
                return true;
            }

            private void SwitchTarget(IPerceptionModel model)
            {
                // if we want to keep the current target, we are ready here
                if (Random.value < _agent.LookAtSettings.ChanceRepeatTarget)
                    return;

                // look nowhere?
                if (_currentTarget && (Random.value < _agent.LookAtSettings.ChanceLookingNowhere))
                {
                    SetTarget(null);
                    return;
                }

                // get all potential targets
                List<KeyValuePair<float, GameObject>> targets = new();
                foreach (GameObject go in model.ObjectsWithTag(CustomTagProperty.InterestingObject))
                {
                    if (!go || go.transform == _currentTarget)
                        continue;

                    CustomPropertyList cpl = go.GetComponent<CustomPropertyList>();
                    if (!cpl)
                        continue;

                    if (!Visible(go.transform))
                        continue;

                    targets.Add(new KeyValuePair<float, GameObject>(cpl.LevelOfInterest, go));
                }

                // add the user
                GameObject user = model.User;
                bool hasUser = user != null;
                bool notLookingAtUser = user.transform != _currentTarget;
                bool userVisible = Visible(user.transform);

                //if (user != null && user.transform != _currentTarget && Visible(user.transform))
                //    targets.Add(new KeyValuePair<float, GameObject>(1.0f, user));
                if (hasUser && notLookingAtUser && userVisible)
                    targets.Add(new KeyValuePair<float, GameObject>(1.0f, user));

                // select & set the new target
                GameObject selected = MathUtilities.RandFromProbabilitySpace(targets);
                SetTarget(selected ? selected.transform : null);
            }

            private void UpdateHeadRotation()
            {
                Quaternion rotation;
                if (_weight > 1.0f)
                {
                    rotation = RotationToward(_currentTarget);
                }
                else
                {
                    Quaternion from = RotationToward(_previousTarget);
                    Quaternion to = RotationToward(_currentTarget);

                    if (MathUtilities.IsSimilar(from, to))
                    {
                        rotation = from;
                        _weight = 1.0f;
                    }
                    else
                    {
                        rotation = Quaternion.Slerp(from, to, Mathf.Clamp01(_weight));
                    }
                }
                _weight += (1.0f / TransitionTime) * Time.deltaTime;

                // apply the rotation here?
                _agent.HeadTransform.SetPositionAndRotation(_agent.HeadTransform.position, rotation);
            }

            // helper
            private HorizonCoordinates ToHorizonCoordinates(Transform target)
            {
                Vector3 opos = _agent.HeadTransform.position;
                Vector3 odir = _agent.TorsoTransform.forward;
                Vector3 oupd = _agent.TorsoTransform.up;
                return new HorizonCoordinates(target.position, opos, odir, oupd);
            }

            private Quaternion RotationToward(Transform target)
            {
                return target ?
                          ToHorizonCoordinates(target).ObserverOrientation()
                        : _agent.HeadTransform.rotation;
            }

            private bool Visible(HorizonCoordinates hc)
                => hc.CheckConstraints(DistanceRange, HorizontalFoV, VerticalFoV);

            private bool Visible(Transform target)
                => Visible(ToHorizonCoordinates(target));

            // ...
            private Transform _previousTarget;
            private Transform _currentTarget;
            private float _weight = 1.0f;

            // ...
            private float _triggerTime = 0.0f;

            // ...
            private bool _enabled = false;
            private readonly bool _valid = false;
            private readonly AgentComponent _agent;

            // ...
            private const float TransitionTime = 1.0f;
            private readonly ValueRange DistanceRange;
            private readonly ValueRange HorizontalFoV;
            private readonly ValueRange VerticalFoV;
        }
    }
}


