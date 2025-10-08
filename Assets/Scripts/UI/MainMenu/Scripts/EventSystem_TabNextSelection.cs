// TabNextSelection.cs
//  from forum posting by halley
//  https://forum.unity.com/threads/tab-between-input-fields.263779/page-2#post-9512734
// Adapted from a forum posting by SirRogers:
//  https://forum.unity.com/threads/
//  tab-between-input-fields.263779/#post-2404236
//
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Support for new Input System
#if ENABLE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
#endif

public class EventSystem_TabNextSelection : MonoBehaviour
{
    #region variables
    [Tooltip("Also support Shift+Tab to move backwards to prior selection")]
    [SerializeField] public bool _backTabSupport = true;

    private EventSystem _eventSystem;
    #endregion

    #region unity_functions
    private void OnEnable()
    {
        _eventSystem = EventSystem.current;
    }

    private void Update()
    {
        if (_eventSystem == null)
            return;

        GameObject currentSelectedGameObject = _eventSystem.currentSelectedGameObject;
        if (currentSelectedGameObject == null)
            return;

        if (!TabPressed())
            return;

        Selectable currentSelectedSelectable = currentSelectedGameObject.GetComponent<Selectable>();
        if (currentSelectedSelectable == null)
            return;

        bool shiftPressed = ShiftPressed();
        Selectable selectableToSwitchTo = shiftPressed && _backTabSupport ? GetPriorSelectable(currentSelectedSelectable) : GetNextSelectable(currentSelectedSelectable);

        // Wrap from end to beginning, or vice versa.
        if (selectableToSwitchTo == null)
        {
            selectableToSwitchTo = currentSelectedSelectable;
            Selectable tmpSelectable;
            if (shiftPressed && _backTabSupport)
                while ((tmpSelectable = GetNextSelectable(selectableToSwitchTo)) != null)
                    selectableToSwitchTo = tmpSelectable;
            else
                while ((tmpSelectable = GetPriorSelectable(selectableToSwitchTo)) != null)
                    selectableToSwitchTo = tmpSelectable;
        }

        if (selectableToSwitchTo == null)
            return;

        // Simulate mouse click for InputFields.
        InputField inputfield = selectableToSwitchTo.GetComponent<InputField>();
        if (inputfield != null)
            inputfield.OnPointerClick(new PointerEventData(_eventSystem));

        // Select the next item in the tab-order of our direction.
        _eventSystem.SetSelectedGameObject(selectableToSwitchTo.gameObject);
    }
    #endregion

    #region helper_functions
    #region input_handling
    private bool TabPressed()
    {
#if ENABLE_INPUT_SYSTEM
            bool tab =
                Keyboard.current.tabKey.wasPressedThisFrame;
#else
        bool tab = Input.GetKeyDown(KeyCode.Tab);
#endif
        return tab;
    }

    private bool ShiftPressed()
    {
#if ENABLE_INPUT_SYSTEM
            bool shift =
                Keyboard.current.leftShiftKey.isPressed ||
                Keyboard.current.rightShiftKey.isPressed;
#else
        bool shift =
            Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift);
#endif
        return shift;
    }
    #endregion

    private Selectable GetPriorSelectable(Selectable current)
    {
        Selectable prior = current.FindSelectableOnLeft();
        if (prior == null)
        {
            prior = current.FindSelectableOnUp();
        }
        return prior;
    }

    private Selectable GetNextSelectable(Selectable current)
    {
        Selectable next = current.FindSelectableOnRight();
        if (next == null)
        {
            next = current.FindSelectableOnDown();
        }
        return next;
    }
    #endregion
}
