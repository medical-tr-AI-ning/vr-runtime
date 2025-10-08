using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: dv: add preemption, pause, breaks, etc.
namespace MedicalTraining
{
    namespace AgentBehavior
    {
        public class ActionList
        {
            public void Enqueue(Action action)
                => actions.Add(action);          

            public void Enqueue(IEnumerable<Action> actionList)
                => actions.AddRange(actionList);

            public void Execute()
            {
                foreach (Action action in actions)
                {
                    action.Execute(this);

                    if (action.Blocking)
                        break;
                }
                actions.RemoveAll(action => action.Ready);
            }

            public void ActionFailed(Action action)
            {
                Debug.LogWarning($"Action failed: {action.GetType()}!");              
                action.MarkReady(); // forced ready

                // TODO: dv: undone: Loop-back to the DMS and let it decide what to do, 
                // or just check the reminder of the list for trash and handle it?
                // ...
            }

            public void ActionTimeout(Action action)
            {
                Debug.LogWarning($"Action timed out: {action.GetType()}!");
                action.MarkReady();    // forced ready
            }

            public Action First => actions.FirstOrDefault();
            
            public Action Last => actions.LastOrDefault();
            
            public bool Empty => NumPendingActions == 0; 
            
            public int NumPendingActions => actions.Count; 

            // ...
            protected List<Action> actions = new();
        }
    }
}
