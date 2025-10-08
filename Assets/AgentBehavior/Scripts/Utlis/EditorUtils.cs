using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace MedicalTraining
{
    namespace Utilities
    {
        public sealed class EditorUtil
        {
            public static void RangeSlider(string label, ref float min, ref float max, float minRange, float maxRange)
            {
                ValueRange value = new ValueRange(min, max);
                EditorUtil.RangeSlider(label, ref value, new ValueRange(minRange, maxRange));
                min = value.min; 
                max = value.max;
            }

            public static void RangeSlider(string label, ref ValueRange value, ValueRange range)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel(new GUIContent(label, $"Range [{range.min:0.##}; {range.max:0.##}]"));
                    GUILayout.Label(new GUIContent($"{value.min:0.#}°", $"min: {range.min:0.##}°"));
                    EditorGUILayout.MinMaxSlider(ref value.min, ref value.max, range.min, range.max);
                    GUILayout.Label(new GUIContent($"{value.max:0.#}°", $"max: {range.max:0.##}°"));
                }
            }
        }
    }
}
#endif
