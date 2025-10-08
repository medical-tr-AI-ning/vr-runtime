using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        using Extensions;

        public class PatientPerceptionController : PerceptionController
        {
            public PatientPerceptionController(GameObject user)
            {
                _user = user;
            }

            public override void Update(IPerceptionModel model)
            {
                InitializeModel(model);

                // here we would search for new objects and agents
                // and would check if they are still visible, etc.

                //// but for now we simply check whether the interesting 
                //// objects are still interesting and update the tags
                //foreach (GameObject obj in model.Objects)
                //{
                //    CustomPropertyList props = obj.GetComponent<CustomPropertyList>();
                //    if (props)
                //    {
                //        if (props.LevelOfInterest > 0.0f)
                //            props.EnsureTag(CustomTagProperty.InterestingObject);
                //        else
                //            props.EnsureTagRemoved(CustomTagProperty.InterestingObject);
                //    }
                //}

                // now go through all events and process or forward them
                while (_events.Count > 0)
                {
                    InteractionEvent interactionEvent = _events.Dequeue();
                    switch (interactionEvent)
                    {
                        case InterestingObjectEvent evt: 
                            if (evt.Handle)
                            {
                                // check the props.
                                CustomPropertyList props = evt.Handle.GetComponent<CustomPropertyList>();
                                if (props == null)
                                {
                                    props = evt.Handle.AddComponent<CustomPropertyList>();
                                    props.EnsureTag(CustomTagProperty.InterestingObject);
                                    props.LevelOfInterest = 1.0f;    // TODO: dv: why?
                                    //props.InterestDecaySpeed = 0.5f;
                                }
                                model.Objects.Add(props.gameObject);
                                if (props.Tags.Contains(CustomTagProperty.Agent))
                                    model.Objects.Add(props.gameObject);

                                evt.Perceive();
                            }
                            else
                            {
                                evt.Ignore(new Reason("Well, 'null' is not that interesting, isn't it?"));
                            }
                            break;

                        case SpeechEvent evt:
                            // perceive the event and forward it to the DMS to decide what to do
                            evt.Perceive();
                            model.EnqueueEvent(evt);
                            
                            // but the user just became interesting (he/she is talking to me!)
                            model.UserLevelOfInterest = 1.0f;
                            break;

                        case PerceptionEvent:
                            Debug.LogWarning("Unknown type of PerceptionEvent");
                            break;

                        default:
                            // forward all the rest
                            model.EnqueueEvent(interactionEvent);
                            break;

                        case null:
                            Debug.LogError("WTF: NULL as event!");
                            //throw new ArgumentNullException(nameof(interactionEvent));
                            break;
                    }
                }
            }

            public override void EnqueueEvent(InteractionEvent interactionEvent)
                => _events.Enqueue(interactionEvent);

            // ...
            private GameObject _user = null;
            private readonly Queue<InteractionEvent> _events = new();
            private bool _initialized = false;

            private void InitializeModel(IPerceptionModel model)
            {
                if (_initialized)
                    return;

                // only objects with a custom property list are perceivable 
                // agents have a tag Agent and the property list is an AgentComponent
                CustomPropertyList[] propertyLists = GameObject.FindObjectsOfType<CustomPropertyList>();
                foreach (CustomPropertyList propertyList in propertyLists)
                {
                    model.Objects.Add(propertyList.gameObject);

                    if (propertyList is AgentComponent)
                        model.Agents.Add(propertyList.gameObject);
                }

                // use the value if explicitly set,
                // otherwise the consider the user to be the object the main camera is attached to
                model.User = _user ? _user: Camera.main.gameObject;
                model.UserLevelOfInterest = 1.0f;

                _initialized = true;
            }
        }
    }
}
