using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: dv: the agent state system needs refactoring
namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public enum PoseState
        {
            //...
            Invalid,

            // face 'pose'
            MouthClosed, // (idle)
            MouthOpen,

            // head pose
            // ...

            // body pose
            Idle, // (idle)
            Bent,
            BentMore,

            // buttocks
            Spread,

            // hands (s. GraspingState)
            Down, // (idle)
            Up,
            Forward,
            Side,

            // palms and legs
            // ->Down (idle)
            // ->Up
        }

        public enum BodyPart
        {
            Invalid,

            Head,
            Face,

            Body,
            //Torso,
            //Hip,
            Buttocks,

            //Hands,
            LeftHand,
            RightHand,

            //Palms,
            LeftPalm,
            RightPalm,

            //Legs,
            LeftLeg,
            RightLeg,

            //Feet,
            //--LeftFoot,
            //--RightFoot,
        }

        public class PoseStateDescriptor
        {
            public PoseStateDescriptor(PoseState idleState, IEnumerable<PoseState> possibleStates = null)
            {
                PossibleStates = possibleStates ?? Enumerable.Empty<PoseState>();

                if (!_possibleStates.Contains(idleState))
                    _possibleStates.Add(idleState);

                IdleState = idleState;
                State = IdleState;
                Dirty = false;  // TODO: declare dirty to sync the states?
            }

            // props
            //public bool Transient { get; set; } = false;
            //public bool Overwritten { get; set; } = false;
            public bool Dirty { get; set; } = false;

            public bool Idle => State == IdleState;
            
            public PoseState IdleState { get; }

            public IEnumerable<PoseState> PossibleStates
            {
                get => _possibleStates;
                private set => _possibleStates = value.ToList();
            }

            public PoseState State
            {
                get
                {
                    return _state;
                }
                set
                {
                    if (_state == value)
                        return;

                    if (PossibleStates.Contains(value))
                    {
                        _state = value;
                        Dirty = true;
                    }
                    else
                    {
                        // dv: assert
                    }
                }
            }

            // private
            private PoseState _state;
            private List<PoseState> _possibleStates;
        }

        // TODO: dv: (low priority) The pose description is fix in the constructor. It should
        //           be possible to construct/change it for different needs and capabilities.
        public class PoseDescription
        {
            // ctor
            public PoseDescription()
            {
                _states = new Dictionary<BodyPart, PoseStateDescriptor>
                {
                    {
                        BodyPart.Face, new PoseStateDescriptor(
                            PoseState.MouthClosed, new List<PoseState>()
                            {
                                PoseState.MouthOpen
                            })
                    },
                    {
                        BodyPart.Head, new PoseStateDescriptor(PoseState.Idle)
                    },
                    {
                        BodyPart.Body, new PoseStateDescriptor(
                            PoseState.Idle, new List<PoseState>()
                            {
                                PoseState.Bent,
                                PoseState.BentMore
                            })
                    },
                    {
                        BodyPart.Buttocks, new PoseStateDescriptor(
                            PoseState.Idle, new List<PoseState>()
                            {
                                PoseState.Spread
                            })
                    },
                    {
                        BodyPart.LeftHand, new PoseStateDescriptor(
                            PoseState.Down, new List<PoseState>()
                            {
                                PoseState.Up,
                                PoseState.Forward,
                                PoseState.Side
                            })
                    },
                    {
                        BodyPart.RightHand, new PoseStateDescriptor(
                            PoseState.Down, new List<PoseState>()
                            {
                                PoseState.Up,
                                PoseState.Forward,
                                PoseState.Side
                            })
                    },
                    {
                        BodyPart.LeftPalm, new PoseStateDescriptor(
                            PoseState.Down, new List<PoseState>()
                            {
                                PoseState.Up
                            })
                    },
                    {
                        BodyPart.RightPalm, new PoseStateDescriptor(
                            PoseState.Down, new List<PoseState>()
                            {
                                PoseState.Up
                            })
                    },
                    {
                        BodyPart.LeftLeg, new PoseStateDescriptor(
                            PoseState.Down, new List<PoseState>()
                            {
                                PoseState.Up
                            })
                    },
                    {
                        BodyPart.RightLeg, new PoseStateDescriptor(
                            PoseState.Down, new List<PoseState>()
                            {
                                PoseState.Up
                            })
                    },
                    //{
                    //    BodyPart.LeftFoot, new PoseStateDescriptor(PoseState.Down)
                    //},
                    //{
                    //    BodyPart.RightFoot, new PoseStateDescriptor(PoseState.Down)
                    //}
                };
            }

            // utilities
            public bool Dirty 
            { 
                get
                {
                    foreach (PoseStateDescriptor descriptor in _states.Values)
                        if (descriptor.Dirty)
                            return true;

                    return false;
                }
            }

            public bool Idle
            {
                get
                {
                    foreach (PoseStateDescriptor descriptor in _states.Values)
                        if (!descriptor.Idle)
                            return false;

                    return true;
                }
            }

            public PoseStateDescriptor State (BodyPart part)
            {
                if (_states.TryGetValue(part, out PoseStateDescriptor descriptor))
                    return descriptor;

                return null;
            } 

            public IEnumerable<KeyValuePair<BodyPart, PoseStateDescriptor>> States { get => _states; }

            public void ClearDirty()
            {
                foreach(PoseStateDescriptor descriptor in _states.Values)
                    descriptor.Dirty = false;
            }

            // convenience props
            public PoseStateDescriptor Head { get => _states[BodyPart.Head]; }
            public PoseStateDescriptor Face { get => _states[BodyPart.Face]; }  // TODO: dv: (long term) replace with dedicated sub-state
            public PoseStateDescriptor Body { get => _states[BodyPart.Body]; }
            public PoseStateDescriptor LeftHand { get => _states[BodyPart.LeftHand]; }
            public PoseStateDescriptor RightHand { get => _states[BodyPart.RightHand]; }
            public PoseStateDescriptor LeftPalm { get => _states[BodyPart.LeftPalm]; }
            public PoseStateDescriptor RightPalm { get => _states[BodyPart.RightPalm]; }
            public PoseStateDescriptor LeftLeg { get => _states[BodyPart.LeftLeg]; }
            public PoseStateDescriptor RightLeg { get => _states[BodyPart.RightLeg]; }

            // ...
            private readonly Dictionary<BodyPart, PoseStateDescriptor> _states;
        }
    }
}