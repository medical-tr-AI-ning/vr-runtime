using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MedicalTraining.AgentBehavior;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class IndirectAnchor : MonoBehaviour
{
    // ...
    [Header("Settings")]
    [SerializeField] private ArmatureHandleType HandleType = ArmatureHandleType.Invalid;
    [SerializeField, Range(0, 10)] private float EnableTime = 0.0f;
    [SerializeField, Range(0, 10)] private float DisableTime = 1.0f;

    // ...
    [Header("Handle State")]
    [SerializeField, ReadOnly] private bool Enabled;
    [SerializeField, ReadOnly] private bool Transient;

    // ...
    [Header("Anchor State")]
    [SerializeField, ReadOnly] private bool ArmatureHandleFound;

    // ..
    private IArmatureHandle _handle = null;
    private GameObject _anchor = null;

    // ...
    public void Activate()
    {
        // do we have a handle (thus a target)?
        if (_handle == null)
            return;

        // set it to track out anchor point
        _handle.SetTarget(_anchor.transform);
        _handle.Enable(EnableTime);
    }

    public void Deactivate()
    {
        if (_handle == null)
            return;

        // disable the handle
        _handle.Disable(DisableTime);
        _handle = null;

        // ... reset the search 
        HandleType = ArmatureHandleType.Invalid;

        // ... and the anchor point
        _anchor.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public void OnTriggerStay(Collider other)
    {
        // don't care, if we already have an active handle
        if (_handle != null && _handle.Enabled)
            return;

        // check if this is the handle we are looking for
        // note: dv: you can fill a list with all available handles and fine-select the "most appropriate"
        IArmatureHandle handle = other.gameObject.GetComponent<IArmatureHandle>();
        if (handle == null || handle.HandleType != HandleType)
            return;

        // found it -> align the anchor point with it (track while inactive)
        _handle = handle;
        _anchor.transform.SetPositionAndRotation(other.transform.position, other.transform.rotation);
    }

    // ...
    void Start()
    {
        // create an indirect anchor to use for controlling the armature handles 
        _anchor = new GameObject("Anchor Point");
        _anchor.transform.parent = transform;
        _anchor.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    // ...
    void Update()
    {
        // just update the state variables
        ArmatureHandleFound = _handle != null;
        if (_handle != null)
        {
            Enabled = _handle.Enabled;
            Transient = _handle.Transient;
        }
    }
}

// ...
#region EditorScript
#if UNITY_EDITOR

[CustomEditor(typeof(IndirectAnchor))]
public class IndirectAnchorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();

        if (GUILayout.Button("Activate"))
            ((IndirectAnchor)target).Activate();

        if (GUILayout.Button("Deactivate"))
            ((IndirectAnchor)target).Deactivate();
    }
}
#endif
#endregion
