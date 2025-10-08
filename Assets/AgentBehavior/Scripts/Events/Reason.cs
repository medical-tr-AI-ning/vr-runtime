using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        /// <summary>
        /// Base class to encapsulate some additional information for 
        /// <code>IActionRequestStateObserver</code> and <code>IPerceptionEventStateObserver</code>.
        /// In particular, this should provide some information why an <code>ActionRequest</code> was declined
        /// or aborted or why a <code>PerceptionEvent</code> was ignored or declined. 
        /// </summary>
        /// <note>
        /// Currently, this is only a prototypic implementation that wraps some free-style text. 
        /// We still need to discuss how the class needs to look like to make sense for the other systems
        /// </note>
        
        // TODO: dv: only prototypic implementation.
        public class Reason
        {
            public Reason(string msg = "")
            {
                Message = msg;
            }
            public override string ToString() => Message;

            public string Message { get; set; }
        }
    }
}
