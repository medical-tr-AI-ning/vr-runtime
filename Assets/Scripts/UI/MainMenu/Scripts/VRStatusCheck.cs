using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;
using System;

[Serializable]
public struct VRDeviceSprites
{
    [Header("Left Controller")]
    [SerializeField] private Sprite _spriteControllerLeftConnected;
    [SerializeField] private Sprite _spriteControllerLeftDisconnected;

    [Header("Right Controller")]
    [SerializeField] private Sprite _spriteControllerRightConnected;
    [SerializeField] private Sprite _spriteControllerRightDisconnected;

    [Header("HMD")]
    [SerializeField] private Sprite _spriteHMDConnected;
    [SerializeField] private Sprite _spriteHMDDisconnected;

    public Sprite SpriteControllerLeftConnected => _spriteControllerLeftConnected;
    public Sprite SpriteControllerLeftDisconnected => _spriteControllerLeftDisconnected;
    public Sprite SpriteControllerRightConnetcted => _spriteControllerRightConnected;
    public Sprite SpriteControllerRightDisconnected => _spriteControllerRightDisconnected;
    public Sprite SpriteHMDConnected => _spriteHMDConnected;
    public Sprite SpriteHMDDisconnected => _spriteHMDDisconnected;
}

[Serializable]
public struct VRDeviceGameObjects
{
    [Header("Left Controller")]
    [SerializeField] private GameObject[] _gameObjectsControllerLeftConnected;
    [Header("Right Controller")]
    [SerializeField] private GameObject[] _gameObjectsControllerRightConnected;
    [Header("HMD")]
    [SerializeField] private GameObject[] _gameObjectsHMDConnected;

    public GameObject[] GameObjectsControllerLeftConnected => _gameObjectsControllerLeftConnected;
    public GameObject[] GameObjectsControllerRightConnected => _gameObjectsControllerRightConnected;
    public GameObject[] GameObjectsHMDConnected => _gameObjectsHMDConnected;
}

public class VRStatusCheck : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private VRDeviceSprites _vRDeviceSprites;
    [SerializeField] private VRDeviceGameObjects _vRDeviceGameObjectsToToggle;
    [SerializeField] private string VRGearStatusActive = "Bereit";
    [SerializeField] private string VRGearStatusInactive = "Nicht bereit";
    [SerializeField] private Color ColorActive = new Color(111, 186, 103);
    [SerializeField] private Color ColorInactive = new Color(196, 196, 196);

    [Space(10)]
    [Header("References")]
    [Header("Images")]
    [SerializeField] private Image ImageControllerLeft;
    [SerializeField] private Image ImageControllerRight;
    [SerializeField] private Image ImageHMD;

    [Space(5)]
    [Header("Labels")]
    [SerializeField] private TMP_Text LabelControllerLeft;
    [SerializeField] private TMP_Text LabelControllerRight;
    [SerializeField] private TMP_Text LabelHMD;
    [SerializeField] private TMP_Text LabelVRGearStatus;

    [Space(5)]
    [Header("Buttons")]
    [SerializeField] private Button ButtonScenarioStart;
    [SerializeField] private Button ButtonTutorialStart;
    [SerializeField] private Button ButtonExit;

    [Space(10)]
    [Header("Debug Status")]
    [SerializeField, ReadOnly]
    private bool ControllerLeftReady = false;
    [SerializeField, ReadOnly]
    private bool ControllerRightReady = false;
    [SerializeField, ReadOnly]
    private bool HMDReady = false;

    public Dictionary<int, string> controllers = new Dictionary<int, string>();

    public Dictionary<int, string> trackers = new Dictionary<int, string>();


    void Awake()
    {
        // Button Setup
        Assert.IsNotNull(ButtonScenarioStart);
        Assert.IsNotNull(ButtonTutorialStart);

        if (ButtonExit != null)
        {
            ButtonExit.interactable = true;
            ButtonExit.onClick.AddListener(delegate ()
            {
                Application.Quit();
            });
        }

        // Initial Sprite/Icon Setup
        ImageControllerLeft.sprite = _vRDeviceSprites.SpriteControllerLeftDisconnected;
        ImageControllerRight.sprite = _vRDeviceSprites.SpriteControllerRightDisconnected;
        ImageHMD.sprite = _vRDeviceSprites.SpriteHMDDisconnected;

        // Initial Animation Setup
        setGameObjects(_vRDeviceGameObjectsToToggle.GameObjectsControllerLeftConnected, false);
        setGameObjects(_vRDeviceGameObjectsToToggle.GameObjectsControllerRightConnected, false);
        setGameObjects(_vRDeviceGameObjectsToToggle.GameObjectsHMDConnected, false);

        // Initial Label Setup
        setTextColor(LabelControllerLeft, ColorInactive);
        setTextColor(LabelControllerRight, ColorInactive);
        setTextColor(LabelHMD, ColorInactive);

#if UNITY_EDITOR
        ButtonScenarioStart.interactable = true;
        ButtonTutorialStart.interactable = true;
#endif
    }

    private void Start()
    {

        OnHMDConnectionChanged(VRDeviceConnectionTracker.Instance.ConnectionHMD);
        OnControllerLeftConnectionChanged(VRDeviceConnectionTracker.Instance.ConnectionControllerLeft);
        OnControllerRightConnectionChanged(VRDeviceConnectionTracker.Instance.ConnectionControllerRight);

        VRDeviceConnectionTracker.Instance.HMDConnectionChanged.AddListener(OnHMDConnectionChanged);
        VRDeviceConnectionTracker.Instance.ControllerLeftConnectionChanged.AddListener(OnControllerLeftConnectionChanged);
        VRDeviceConnectionTracker.Instance.ControllerRightConnectionChanged.AddListener(OnControllerRightConnectionChanged);
    }

#if !UNITY_EDITOR
    private void Update()
    {
        if (ControllerLeftReady && ControllerRightReady && HMDReady)
        {
            ButtonScenarioStart.interactable = true;
            ButtonTutorialStart.interactable = true;
            setTextLabel(LabelVRGearStatus, VRGearStatusActive);
            setTextColor(LabelVRGearStatus, ColorActive);
        }
        else
        {
            ButtonScenarioStart.interactable = false;
            ButtonTutorialStart.interactable = false;
            setTextLabel(LabelVRGearStatus, VRGearStatusInactive);
            setTextColor(LabelVRGearStatus, ColorInactive);
        }
    }
#endif
    private void OnHMDConnectionChanged(bool pConnected)
    {
        if (pConnected)
        {
            setImageSprite(ImageHMD, _vRDeviceSprites.SpriteHMDConnected);
            setTextColor(LabelHMD, ColorActive);
            setGameObjects(_vRDeviceGameObjectsToToggle.GameObjectsHMDConnected, true);
            HMDReady = true;
        }
        else
        {
            setImageSprite(ImageHMD, _vRDeviceSprites.SpriteHMDDisconnected);
            setTextColor(LabelHMD, ColorInactive);
            setGameObjects(_vRDeviceGameObjectsToToggle.GameObjectsHMDConnected, false);
            HMDReady = false;
        }
    }

    private void OnControllerLeftConnectionChanged(bool pConnected)
    {
        if (pConnected)
        {
            setImageSprite(ImageControllerLeft, _vRDeviceSprites.SpriteControllerLeftConnected);
            setTextColor(LabelControllerLeft, ColorActive);
            setGameObjects(_vRDeviceGameObjectsToToggle.GameObjectsControllerLeftConnected, true);
            ControllerLeftReady = true;
        }
        else
        {
            setImageSprite(ImageControllerLeft, _vRDeviceSprites.SpriteControllerLeftDisconnected);
            setTextColor(LabelControllerLeft, ColorInactive);
            setGameObjects(_vRDeviceGameObjectsToToggle.GameObjectsControllerLeftConnected, false);
            ControllerLeftReady = false;
        }
    }

    private void OnControllerRightConnectionChanged(bool pConnected)
    {
        if (pConnected)
        {
            setImageSprite(ImageControllerRight, _vRDeviceSprites.SpriteControllerRightConnetcted);
            setTextColor(LabelControllerRight, ColorActive);
            setGameObjects(_vRDeviceGameObjectsToToggle.GameObjectsControllerRightConnected, true);
            ControllerRightReady = true;
        }
        else
        {
            setImageSprite(ImageControllerRight, _vRDeviceSprites.SpriteControllerRightDisconnected);
            setTextColor(LabelControllerRight, ColorInactive);
            setGameObjects(_vRDeviceGameObjectsToToggle.GameObjectsControllerRightConnected, false);
            ControllerRightReady = false;
        }
    }

#region helper_functions
    private void setImageSprite(Image pImage, Sprite pSprite)
    {
        if (pImage)
        {
            pImage.sprite = pSprite;
        }
    }

    private void setTextColor(TMP_Text pText, Color pColor)
    {
        if (pText)
        {
            pText.color = pColor;
        }
    }

    private void setTextLabel(TMP_Text pText, string pTextLabel)
    {
        if (pText)
        {
            pText.text = pTextLabel;
        }
    }

    private void setGameObjects(GameObject[] pGameObjects, bool pBool)
    {
        if (pGameObjects != null)
        {
            foreach (GameObject gameObject in pGameObjects)
            {
                if (gameObject)
                {
                    gameObject.SetActive(pBool);
                }
            }
        }
    }
#endregion
}
