using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        /// <summary>
        /// Type of this speech message, as identified by the NLU system.
        /// </summary>
        public enum SpeechEventType
        {
            None,
            NotIdentifiable,

            Greeting,
            Farewell,

            Question,
            Information,

            Instruction,  // This is an additional hint. The actual instruction
                          // should be formulated as ActionRequest. 
        }

        /// <summary>
        /// 
        /// </summary>
        public enum SpeechSemanticToken
        {
            None
        }

        /// <summary>
        /// 
        /// </summary>
        public enum SpeechEmotionalToken
        {
            None
        }

        /// <summary>
        /// Convenience struct to encapsulate the most important data of 
        /// a SpeechEvent. Direct use of this struct should be avoided.
        /// </summary>
        public readonly struct SpeechEventInformation
        {
            /// <summary>
            /// Constructor of the SpeechEventInformation struct. 
            /// </summary>
            /// <param name="id">Identifier of the message, e.g., from the dialog system.</param>
            /// <param name="text">Raw text of the message.</param>
            public SpeechEventInformation(int id, String text = "")
            {
                ID = id;
                RawText = text;
                SemanticTokens = Enumerable.Empty<SpeechSemanticToken>();
                EmotionalTokens = Enumerable.Empty<SpeechEmotionalToken>();
            }

            /// <summary>
            /// Identifier of the message, e.g., from the dialog system.
            /// </summary>
            public readonly int ID;

            /// <summary>
            /// Raw text of the message.
            /// </summary>
            public readonly String RawText;

            /// <summary>
            /// Always empty.
            /// </summary>
            public readonly IEnumerable<SpeechSemanticToken> SemanticTokens;
            
            /// <summary>
            /// Always empty.
            /// </summary>
            public readonly IEnumerable<SpeechEmotionalToken> EmotionalTokens;
        }

        /// <summary>
        /// Speech message as identified by the NLU system.
        /// The SpeechEvent is neither processed nor forwarded by the Agent but may 
        /// be helpful to make it behave more naturally, e.g., greeting or waving back.
        /// </summary>
        public class SpeechEvent : PerceptionEvent
        {
            /// <summary>
            /// The constructor of the SpeechEvent
            /// </summary>
            /// <param name="observer">Observer for this SpeechEvent.</param>
            /// <param name="type">Type of the message.</param>
            /// <param name="id">Identifier of the message, e.g., from the dialog system.</param>
            /// <param name="text">Raw text of the message.</param>
            public SpeechEvent(IPerceptionEventStateObserver observer, SpeechEventType type, int id, String text = "")
                : base(observer)
            {
                Type = type;    
                Info = new SpeechEventInformation(id, text); 
            }

            /// <summary>
            /// The constructor of the SpeechEvent
            /// </summary>
            /// <param name="type">Type of the message.</param>
            /// <param name="id">Identifier of the message, e.g., from the dialog system.</param>
            /// <param name="text">Raw text of the message.</param>
            public SpeechEvent(SpeechEventType type, int id, String text = "")
                : this(null, type, id, text)
            {
                Type = type;
                Info = new SpeechEventInformation(id, text);
            }

            // ...
            public SpeechEventType Type { get; } 
            public SpeechEventInformation Info { get; }
        }
    }
}
