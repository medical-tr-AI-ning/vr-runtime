using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class UnityBoolEvent : UnityEvent<bool> { }

public class VRDeviceConnectionTracker : MonoBehaviour
{
    private static VRDeviceConnectionTracker _instance;
    public static VRDeviceConnectionTracker Instance { get { return _instance; } }

    [Header("Status")]
    // HMD
    [SerializeField, ReadOnly]
    private bool _connectionHMD = false;
    public bool ConnectionHMD
    {
        get { return _connectionHMD; }
    }
    public UnityBoolEvent HMDConnectionChanged = new UnityBoolEvent();

    // Left Controller
    [SerializeField, ReadOnly]
    private bool _connectionControllerLeft = false;
    public bool ConnectionControllerLeft
    {
        get { return _connectionControllerLeft; }
    }
    public UnityBoolEvent ControllerLeftConnectionChanged = new UnityBoolEvent();

    // Right Controller
    [SerializeField, ReadOnly]
    private bool _connectionControllerRight = false;
    public bool ConnectionControllerRight
    {
        get { return _connectionControllerRight; }
    }
    public UnityBoolEvent ControllerRightConnectionChanged = new UnityBoolEvent();

    public Dictionary<int, string> ConnectedControllers { get; } = new Dictionary<int, string>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        SteamVR_Events.DeviceConnected.AddListener(OnDeviceConnected);
    }

    private void OnDeviceConnected(int deviceID, bool conn)
    {
        ETrackedDeviceClass deviceClass = OpenVR.System.GetTrackedDeviceClass((uint)deviceID);
        if (deviceClass == ETrackedDeviceClass.HMD)
        {
            _connectionHMD = conn;
            HMDConnectionChanged.Invoke(conn);
        }
        else if (deviceClass == ETrackedDeviceClass.Controller)
        {
            ETrackedPropertyError error = new ETrackedPropertyError();
            int controllerRole = OpenVR.System.GetInt32TrackedDeviceProperty((uint)deviceID, ETrackedDeviceProperty.Prop_ControllerRoleHint_Int32, ref error);

            if (conn)
            {
                StringBuilder controllerSerialBuilder = new StringBuilder(50);
                OpenVR.System.GetStringTrackedDeviceProperty((uint)deviceID, ETrackedDeviceProperty.Prop_SerialNumber_String, controllerSerialBuilder, 50, ref error);

                if (!ConnectedControllers.ContainsKey((int)deviceID))
                {
                    ConnectedControllers.Add(deviceID, controllerSerialBuilder.ToString());
                    Debug.Log("Controller connected: " + deviceID.ToString() + ": " + ConnectedControllers[deviceID]);

                    if (controllerRole == (int)ETrackedControllerRole.RightHand)
                    {
                        _connectionControllerRight = true;
                        ControllerRightConnectionChanged.Invoke(true);
                    }
                    else if (controllerRole == (int)ETrackedControllerRole.LeftHand)
                    {
                        _connectionControllerLeft = true;
                        ControllerLeftConnectionChanged.Invoke(true);
                    }
                }
            }
            else
            {
                if (controllerRole == (int)ETrackedControllerRole.RightHand)
                {
                    _connectionControllerRight = false;
                    ControllerRightConnectionChanged.Invoke(false);
                }
                else if (controllerRole == (int)ETrackedControllerRole.LeftHand)
                {
                    _connectionControllerLeft = false;
                    ControllerLeftConnectionChanged.Invoke(false);
                }

                Debug.Log("Controller disconnected: " + deviceID.ToString() + ": " + ConnectedControllers[deviceID]);
                ConnectedControllers.Remove((int)deviceID);

            }
        }
    }
}
