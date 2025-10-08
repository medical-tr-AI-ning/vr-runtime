using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace AgentBehavior
    {
        namespace Extensions 
        {
            internal static class ActionRequestExtensions
            {
                public static void Accept(this ActionRequest request)
                {
                    foreach (var observer in request.Observers)
                        observer.OnAccepted(request);
                }
                public static void Decline(this ActionRequest request, Reason reason)
                {
                    foreach (var observer in request.Observers)
                        observer.OnDeclined(request, reason);
                }
                public static void Start(this ActionRequest request)
                {
                    foreach (var observer in request.Observers)
                        observer.OnStarted(request);
                }
                public static void Abort(this ActionRequest request, Reason reason)
                {
                    foreach (var observer in request.Observers)
                        observer.OnAborted(request, reason);
                }
                public static void Finish(this ActionRequest request)
                {
                    foreach (var observer in request.Observers)
                        observer.OnFinished(request);
                }
            }

            internal static class PerceptionEventExtensions
            {
                public static void Perceive(this PerceptionEvent request)
                {
                    foreach (var observer in request.Observers)
                        observer.OnPerceived(request);
                }
                public static void Ignore(this PerceptionEvent request, Reason reason)
                {
                    foreach (var observer in request.Observers)
                        observer.OnIgnored(request, reason);
                }
                public static void Accept(this PerceptionEvent request)
                {
                    foreach (var observer in request.Observers)
                        observer.OnAccepted(request);
                }
                public static void Decline(this PerceptionEvent request, Reason reason)
                {
                    foreach (var observer in request.Observers)
                        observer.OnDeclined(request, reason);
                }
            }
        }
    }
}

