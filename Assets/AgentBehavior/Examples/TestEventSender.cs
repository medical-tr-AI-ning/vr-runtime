using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MedicalTraining.AgentBehavior;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestEventSender : MonoBehaviour, IActionRequestStateObserver, IPerceptionEventStateObserver
{
    // ... 
    public enum TurnToWhere
    {
        User,
        AwayFromUser,
        Back,
        ExplicitAngle
    }

    // parameters
    [Header("Pose Parameters")]
    public BodySide BodySide = BodySide.Both;
    public HandPose HandPose = HandPose.Forward;
    public bool PalmsUp = false;

    [Header("Motion Parameters")]
    public TurnToWhere TurnTo = TurnToWhere.Back;
    [Range(-180.0f, 180.0f)] public float TurnAngle = 0.0f;

    // messages
    //public void SendNotification()
    //    => _controller?.notify(new Notification());

    public void SendSpeechEvent(SpeechEventType type, string message, int id)
        => notify(new SpeechEvent(this, type, id, message));

    public void SendWalkHome()
        => notify(new WalkHome(this));

    public void SendTurnRequest()
    {
        switch (TurnTo)
        {
            case TurnToWhere.User:
                notify(new Turn(this));
                break;

            case TurnToWhere.AwayFromUser:
                notify(new Turn(this, true));
                break;

            case TurnToWhere.Back: 
                notify(new Turn(this, 180.0f)); 
                break;

            case TurnToWhere.ExplicitAngle: 
                notify(new Turn(this, TurnAngle)); 
                break;
        }
    }

    public void SendRaiseHand()
        => notify(new RaiseHand(this, BodySide, HandPose, PalmsUp));

    public void SendResetHandPose()
        => notify(new ResetHandPose(this, BodySide));

    public void SendRaiseLeg()
        => notify(new RaiseLeg(this, BodySide));
    
    public void SendResetLegPose()
        => notify(new ResetLegPose(this, BodySide));

    public void SendOpenMouth()
        => notify(new OpenMouth(this));
    
    public void SendCloseMouth()
        => notify(new CloseMouth(this));

    public void SendBendForward()
        => notify(new BendForward(this));
    
    public void SendResetBodyPose()
        => notify(new ResetBodyPose(this));

    public void SendSpreadButtocks()
        => notify(new MoveButtocks(this, true, true));

    public void ReleaseButtocks()
        => notify(new MoveButtocks(this, false));

    public void SendChangeClothes(ClothingState clothing)
        => notify(new ChangeClothes(this, clothing));

    protected virtual void Start() 
    {
        // TODO: dv: HACK! (but good enough for testing)
        AgentController[] agents = FindObjectsOfType<AgentController>();
        _controllers = agents.ToList();

        Debug.Log("AgentController found");
    }

    // ...
    private void notify(InteractionEvent evt)
    {
        foreach (AgentController controller in _controllers)
            controller.notify(evt);
    }

    // ...
    private List<AgentController> _controllers = new();

    // ...
    // action event observer interface
    public virtual void OnAccepted(ActionRequest request) 
    {
        Debug.Log($"ActionRequest::Accepted => {request.GetType()}");
    }

    public virtual void OnDeclined(ActionRequest request, Reason reason)
    {
        Debug.Log($"ActionRequest::Declined => {request.GetType()}: {reason}");
    }

    public virtual void OnStarted(ActionRequest request)
    {
        Debug.Log($"ActionRequest::Started => {request.GetType()}");
    }

    public virtual void OnAborted(ActionRequest request, Reason reason)
    {
        Debug.Log($"ActionRequest::Aborted => {request.GetType()}: {reason}");
    }

    public virtual void OnFinished(ActionRequest request)
    {
        Debug.Log($"ActionRequest::Finished => {request.GetType()}");
    }

    // ...
    // perception event observer interface
    public virtual void OnAccepted(PerceptionEvent perceptionEvent)
    {
        Debug.Log($"PerceptionEvent::Accepted => {perceptionEvent.GetType()}");
        if ( perceptionEvent is SpeechEvent speechEvent )
        {
            DialogueController.Instance.ExecuteDialogueOption(speechEvent.Info.RawText);
        }
    }

    public virtual void OnDeclined(PerceptionEvent perceptionEvent, Reason reason)
    {
        Debug.Log($"PerceptionEvent::Declined => {perceptionEvent.GetType()}: {reason}");
    }

    public virtual void OnPerceived(PerceptionEvent perceptionEvent)
    {
        Debug.Log($"PerceptionEvent::Perceived => {perceptionEvent.GetType()}");
    }

    public virtual void OnIgnored(PerceptionEvent perceptionEvent, Reason reason)
    {
        Debug.Log($"PerceptionEvent::Ignored => {perceptionEvent.GetType()}: {reason}");
    }
}


// ...
#region Editor
#if UNITY_EDITOR

[CustomEditor(typeof(TestEventSender))]
public class TestEventSenderEditor : Editor
{
    [SerializeField, HideInInspector] private bool _showSpeechButtons = true;
    [SerializeField, HideInInspector] private bool _showClothingButtons = true;
    [SerializeField, HideInInspector] private bool _showMotionButtons = true;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();

        TestEventSender sender = (TestEventSender)target;

        //if (GUILayout.Button("Send Notification"))
        //    sender.SendNotification();

        _showSpeechButtons = EditorGUILayout.Foldout(_showSpeechButtons, "Speech Events");
        if (_showSpeechButtons)
        {
            if (GUILayout.Button("Say \"Hello!\""))
                sender.SendSpeechEvent(SpeechEventType.Greeting, "talk-hello", 1);

            if (GUILayout.Button("Say what's wrong!"))
                sender.SendSpeechEvent(SpeechEventType.Question, "talk-reason", 3);
        }


        _showClothingButtons = EditorGUILayout.Foldout(_showClothingButtons, "Change Clothes Events");
        if (_showClothingButtons)
        {
            if (GUILayout.Button("Send ChangeClothes(Underwear)"))
                sender.SendChangeClothes(ClothingState.Underwear);

            if (GUILayout.Button("Send ChangeClothes(Naked)"))
                sender.SendChangeClothes(ClothingState.Naked);

            if (GUILayout.Button("Send ChangeClothes(Dressed)"))
                sender.SendChangeClothes(ClothingState.Dressed);
        }

        _showMotionButtons = EditorGUILayout.Foldout(_showMotionButtons, "Movement Events");
        if (_showMotionButtons)
        {
            if (GUILayout.Button("Walk Home"))
                sender.SendWalkHome();

            if (GUILayout.Button("Turn"))
                sender.SendTurnRequest();

            if (GUILayout.Button("Reset Pose"))
                sender.SendResetBodyPose();

            if (GUILayout.Button("Raise Arm"))
                sender.SendRaiseHand();

            if (GUILayout.Button("Lower Arm"))
                sender.SendResetHandPose();

            if (GUILayout.Button("Raise Leg"))
                sender.SendRaiseLeg();

            if (GUILayout.Button("Lower Leg"))
                sender.SendResetLegPose();

            if (GUILayout.Button("Open Mouth"))
                sender.SendOpenMouth();

            if (GUILayout.Button("Close Mouth"))
                sender.SendCloseMouth();

            if (GUILayout.Button("Bend Forward"))
                sender.SendBendForward();

            if (GUILayout.Button("Spread Buttocks"))
                sender.SendSpreadButtocks();

            if (GUILayout.Button("Release Buttocks"))
                sender.ReleaseButtocks();
        }
    }
}
#endif
#endregion
