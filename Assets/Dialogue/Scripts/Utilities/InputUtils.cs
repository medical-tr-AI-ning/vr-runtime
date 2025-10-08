using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace MedicalTraining.Dialogue.Utilities
{
    /// <summary>
    /// Utilities for simulating inputs to the native Unity navigation system.
    /// Useful for mapping VR controller inputs to Unity navigation events.
    /// </summary>
    public static class InputUtils
    {
        public static void SimulateInputSystemNavigationEvent(MoveDirection direction)
        {
            //Use Unity Navigation System only for Up & Down Events
            Assert.IsTrue(direction == MoveDirection.Up || direction == MoveDirection.Down);

            AxisEventData data = new AxisEventData(EventSystem.current)
            {
                moveDir = direction,
                selectedObject = EventSystem.current.currentSelectedGameObject
            };
            ExecuteEvents.Execute(data.selectedObject, data, ExecuteEvents.moveHandler);
        }

        public static void SimulateInputSystemSelectEvent()
        {
            //Use Unity Navigation System for select events

            BaseEventData data = new BaseEventData(EventSystem.current)
            {
                selectedObject = EventSystem.current.currentSelectedGameObject
            };
            ExecuteEvents.Execute(data.selectedObject, data, ExecuteEvents.submitHandler);
        }
    }
}