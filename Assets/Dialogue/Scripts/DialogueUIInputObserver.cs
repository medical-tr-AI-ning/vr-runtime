using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace MedicalTraining.Dialogue
{
    /// <summary>
    /// Receives Inputs from SteamVR Controllers or Keyboard
    /// and triggers the appropriate actions in the DialogueUIController.
    /// Handles Attachment to the VR hands.
    /// </summary>
    [DefaultExecutionOrder(100)]
    public class DialogueUIInputObserver : MonoBehaviour
    {
        private static readonly float JOYSTICK_INPUT_THRESHOLD = 0.6f;
        private static readonly float SECONDS_BETWEEN_INPUTS_WHEN_HOLDING_JOYSTICK = 0.28f;  


        [Header("Index Input Actions")] public SteamVR_Action_Boolean toggleUI;
        public SteamVR_Action_Boolean selectUIOption;
        public SteamVR_Action_Vector2 navigateUI;

        public DialogueUIController DialogueUIController;


        private Valve.VR.InteractionSystem.Hand RightHand;
        private Valve.VR.InteractionSystem.Hand LeftHand;
        private Valve.VR.InteractionSystem.Hand hand;
        private Valve.VR.InteractionSystem.Hand otherHand;
        private Valve.VR.InteractionSystem.Interactable interactable;

        private bool m_joystickCentered = true;
        private float _timeDeltaSinceHoldInput = 0f;

        private void Awake()
        {
            toggleUI.onStateDown += OnToggleUIIndexInput;
            selectUIOption.onStateDown += OnSelectUIOptionIndexInput;
            navigateUI.onAxis += OnNavigateUIIndexInput;
        }

        private void Start()
        {
            RightHand = Player.instance.rightHand; //GameObject.Find("RightHand")?.GetComponent<Valve.VR.InteractionSystem.Hand>();
            LeftHand = Player.instance.leftHand; //GameObject.Find("LeftHand")?.GetComponent<Valve.VR.InteractionSystem.Hand>();
            interactable = gameObject.GetComponent<Valve.VR.InteractionSystem.Interactable>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                DialogueUIController.OnToggleVisibleEvent();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                DialogueUIController.OnNavigationEvent(MoveDirection.Left);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                DialogueUIController.OnNavigationEvent(MoveDirection.Right);
            }

            
            // Clicking in the Desktop application makes the DialogueUI lose its selection. This mitigates it.
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                DialogueUIController.RestoreSelection();
            }

            _timeDeltaSinceHoldInput += Time.deltaTime;

            //All other Keyboard Inputs are handled automatically by native Unity Interaction/Navigation
        }

        private void OnDestroy()
        {
            toggleUI.onStateDown -= OnToggleUIIndexInput;
            selectUIOption.onStateDown -= OnSelectUIOptionIndexInput;
            navigateUI.onAxis -= OnNavigateUIIndexInput;
        }

        private void OnToggleUIIndexInput(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            if (toggleUI.activeDevice == SteamVR_Input_Sources.LeftHand)
            {
                hand = LeftHand;
                otherHand = RightHand;
            }
            else if (toggleUI.activeDevice == SteamVR_Input_Sources.RightHand)
            {
                hand = RightHand;
                otherHand = LeftHand;
            }

            this.OnToggleUIInput();
        }

        private void OnSelectUIOptionIndexInput(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Valve.VR.InteractionSystem.Hand selectHand = null;
            if (selectUIOption.activeDevice == SteamVR_Input_Sources.LeftHand)
            {
                selectHand = LeftHand;
            }
            else if (selectUIOption.activeDevice == SteamVR_Input_Sources.RightHand)
            {
                selectHand = RightHand;
            }

            if (hand == selectHand)
            {
                this.OnSelectUIOptionInput();
            }
        }

        private void OnNavigateUIIndexInput(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource,
            Vector2 axis, Vector2 delta)
        {
            Valve.VR.InteractionSystem.Hand navigatedHand = null;
            if (navigateUI.activeDevice == SteamVR_Input_Sources.LeftHand)
            {
                navigatedHand = LeftHand;
            }
            else if (navigateUI.activeDevice == SteamVR_Input_Sources.RightHand)
            {
                navigatedHand = RightHand;
            }

            if (hand != navigatedHand)
            {
                return;
            }

            MoveDirection direction;
            float extent;

            if (Mathf.Abs(axis.x) > Mathf.Abs(axis.y))
            {
                direction = axis.x > 0 ? MoveDirection.Right : MoveDirection.Left;
                extent = Mathf.Abs(axis.x);
            }
            else
            {
                direction = axis.y > 0 ? MoveDirection.Up : MoveDirection.Down;
                extent = Mathf.Abs(axis.y);
            }

            bool joystickTitled = extent > DialogueUIInputObserver.JOYSTICK_INPUT_THRESHOLD;
            bool delayBetweenInputsReached = _timeDeltaSinceHoldInput > SECONDS_BETWEEN_INPUTS_WHEN_HOLDING_JOYSTICK;

            if (joystickTitled)
            {
                if (m_joystickCentered || delayBetweenInputsReached)
                {
                    m_joystickCentered = false;
                    DialogueUIController.OnNavigationEvent(direction);
                    _timeDeltaSinceHoldInput = 0f;
                }
            }
            else
            {
                m_joystickCentered = true;
            }
        }

        private void OnToggleUIInput()
        {
            if (hand.currentAttachedObjectInfo.HasValue &&
                hand.currentAttachedObjectInfo.Value.attachedObject == gameObject)
            {
                hand.DetachObject(gameObject);
                hand.HoverUnlock(interactable);
                DialogueUIController.OnToggleVisibleEvent();
                hand = null;
            }
            else
            {
                if (!otherHand.currentAttachedObjectInfo.HasValue ||
                    otherHand.currentAttachedObjectInfo.Value.attachedObject != gameObject)
                {
                    DialogueUIController.OnToggleVisibleEvent();
                }
                else
                {
                    otherHand.DetachObject(gameObject);
                    otherHand.HoverUnlock(interactable);
                }

                hand.AttachObject(gameObject, Valve.VR.InteractionSystem.GrabTypes.Scripted,
                    Valve.VR.InteractionSystem.Hand.AttachmentFlags.ParentToHand |
                    Valve.VR.InteractionSystem.Hand.AttachmentFlags.TurnOnKinematic |
                    Valve.VR.InteractionSystem.Hand.AttachmentFlags.SnapOnAttach |
                    Valve.VR.InteractionSystem.Hand.AttachmentFlags.DetachFromOtherHand);

                hand.HoverLock(interactable);
            }
        }

        private void OnSelectUIOptionInput()
        {
            DialogueUIController.OnSubmitEvent();
        }
    }
}