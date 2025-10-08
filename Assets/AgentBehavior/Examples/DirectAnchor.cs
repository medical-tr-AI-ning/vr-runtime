using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DirectAnchor : MonoBehaviour
{
    // ...
    [Header("Settings")]
    [SerializeField] private ArmatureHandleType HandleType = ArmatureHandleType.Invalid;
    [SerializeField, Range(0, 10)] private float EnableTime  = 0.0f;
    [SerializeField, Range(0, 10)] private float DisableTime = 1.0f;

    // ...
    [Header("Handle State")]
    [SerializeField, ReadOnly] private bool Enabled;
    [SerializeField, ReadOnly] private bool Transient;

    // ...
    [Header("Anchor State")]
    [SerializeField, ReadOnly] private bool PatientControllerFound;
    [SerializeField, ReadOnly] private bool ArmatureHandleFound;

    // ..
    private AgentController _patient = null;
    private IArmatureHandle _handle  = null;
    private ArmatureHandleType _previousHandleType = ArmatureHandleType.Invalid;

    // ...
    public void Activate()
    {
        if (_handle == null)
            return;

        _handle.SetTarget(transform);
        _handle.Enable(EnableTime);
    }

    public void Deactivate()
    {
        if (_handle != null)
            _handle.Disable(DisableTime);
    }

    // ...
    void Start()
    {
        GameObject patientNode = GameObject.Find("Agents/Patient");
        if (patientNode)
            _patient = patientNode.GetComponent<AgentController>();

        PatientControllerFound = _patient != null;
    }

    // ...
    void Update()
    {
        if (!_patient)
            return;

        if (_handle == null || _previousHandleType != HandleType)
        {
            _handle = _patient.GetArmatureHandle(HandleType);
            _previousHandleType = HandleType;
        }

        if (_handle != null)
        {
            Enabled = _handle.Enabled;
            Transient = _handle.Transient;
        }
        ArmatureHandleFound = _handle != null;
    }
}

// ...
#region EditorScript
#if UNITY_EDITOR

[CustomEditor(typeof(DirectAnchor))]
public class DirectAnchorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();

        if (GUILayout.Button("Activate"))
            ((DirectAnchor)target).Activate();

        if (GUILayout.Button("Deactivate"))
            ((DirectAnchor)target).Deactivate();
    }
}
#endif
#endregion
