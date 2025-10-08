using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining
{
    namespace Utilities
    {

        // ...
        [System.Serializable]
        public struct ValueRange
        {
            public enum ValueType
            {
                Angle,
                Real
            }

            public ValueRange(float min, float max, ValueType valueType = ValueType.Real)
            {
                float mn = NormalizeValue(min, valueType);
                float mx = NormalizeValue(max, valueType);

                this.min = Mathf.Min(mn, mx);
                this.max = Mathf.Max(mn, mx);
                Type = valueType;
            }

            public float min;
            public float max;
            public ValueType Type { get; private set; }

            public readonly bool Contains(float value)
            {
                float v = NormalizeValue(value, Type);
                return (v >= this.min) && (v <= this.max);
            }

            public readonly float Clamp(float value)
                => Mathf.Clamp(NormalizeValue(value, Type), min, max);

            public readonly float Remap(float value)
                => (Mathf.Clamp01(value)) * (max - min) + min;

            // ...
            private static float NormalizeValue(float value, ValueType type)
                => type == ValueType.Angle ? MathUtilities.NormalizeAngle(value) : value;
        }

        // ...
        public struct HorizonCoordinates
        {
            public struct Base
            {
                public Base(Vector3 position, Vector3 direction, Vector3 up)
                {
                    Position = position;

                    Direction = direction.normalized;
                    Up = up.normalized;

                    Right = Vector3.Cross(Up, Direction).normalized;
                    Up = Vector3.Cross(Direction, Right);
                }

                public Vector3 Position { get; private set; }
                public Vector3 Direction { get; private set; }
                public Vector3 Up { get; private set; }
                public Vector3 Right { get; private set; }
            }

            public HorizonCoordinates(Vector3 point, Transform observer)
                : this(point, observer.position, observer.forward, observer.up)
            {
            }

            public HorizonCoordinates(Vector3 point, Vector3 observerPosition, Vector3 observerDirection, Vector3 observerUp)
            {
                Observer = new(observerPosition, observerDirection, observerUp);

                Distance = (point - Observer.Position).magnitude;
                Vector3 direction = (point - Observer.Position).normalized;

                // project the direction in observer space
                float d = Vector3.Dot(direction, Observer.Direction);
                float u = Vector3.Dot(direction, Observer.Up);
                float r = Vector3.Dot(direction, Observer.Right);

                // get the angles
                Azimuth = MathUtilities.NormalizeAngle(Mathf.Rad2Deg * Mathf.Atan2(r, d));
                Ascent  = MathUtilities.NormalizeAngle(Mathf.Rad2Deg * Mathf.Atan2(u, Mathf.Sqrt(r*r + d*d)));
            }

            public readonly Quaternion ObserverOrientation(ValueRange? hFoV = null, ValueRange? vFoV = null)
            {
                float az = hFoV.HasValue ? hFoV.Value.Clamp(Azimuth) : Azimuth;
                float ac = vFoV.HasValue ? vFoV.Value.Clamp(Ascent)  : Ascent;

                Quaternion q1 = Quaternion.AngleAxis(az, Observer.Up);
                Quaternion q2 = Quaternion.AngleAxis(-ac, Observer.Right);
                Quaternion q3 = Quaternion.LookRotation(Observer.Direction, Observer.Up);
                return (q1 * q2) * q3;
            }

            public readonly Vector3 Position(ValueRange? dist = null, ValueRange? hFoV = null, ValueRange? vFoV = null)
            {
                float vd = dist.HasValue ? dist.Value.Clamp(Distance) : Distance;
                return Observer.Position + ObserverOrientation(hFoV, vFoV) * (new Vector3(0, 0, vd));
            }

            public readonly bool CheckDistance(ValueRange range)
                => range.Contains(Distance);

            public readonly bool CheckHorizontalFoV(ValueRange range)
                => range.Contains(Azimuth);

            public readonly bool CheckVerticalFoV(ValueRange range)
                => range.Contains(Ascent);

            public readonly bool CheckConstraints(ValueRange? distance, ValueRange? horizontalFoV, ValueRange? verticalFoV)
            {
                bool inDistRange = distance.HasValue      ? CheckDistance(distance.Value) : true;
                bool inHFovRange = horizontalFoV.HasValue ? CheckHorizontalFoV(horizontalFoV.Value) : true;
                bool inVFovRange = verticalFoV.HasValue   ? CheckVerticalFoV(verticalFoV.Value) : true;

                return inDistRange && inHFovRange && inVFovRange;
            }

            // ...
            public float Azimuth { get; private set; }
            public float Ascent { get; private set; }
            public float Distance { get; private set; }
            public Base Observer { get; private set; }
        }


        // ...
        public sealed class MathUtilities
        {
            public static float RandInRange(ValueRange range)
                => range.Remap(Random.value);

            public static T RandFromProbabilitySpace<T>(List<KeyValuePair<float, T>> list)
            {
                if (list == null || list.Count == 0)
                    return default(T);

                List<KeyValuePair<float, T>> l = new(list);
                l.Sort((x, y) => x.Key.CompareTo(y.Key));

                float[] cumulative = new float[l.Count];
                cumulative[0] = l[0].Key;

                for (int i = 1; i < l.Count; ++i)
                    cumulative[i] = cumulative[i - 1] + l[i].Key;

                float max = cumulative[^1];
                float sel = RandInRange(new ValueRange(0, max));

                for (int i = 0; i < l.Count; ++i)
                {
                    if (sel <= cumulative[i])
                        return l[i].Value;
                }
                // we should never be here
                return default(T);
            }

            public static float NormalizeAngle(float angle)
            {
                while (angle > 180)
                    angle -= 360.0f;

                while (angle < -180)
                    angle += 360.0f;

                return angle;
            }

            public static float DiffAngle(float d1, float d2)
                => NormalizeAngle(d1) - NormalizeAngle(d2);

            public static bool IsEqualAngle(float d1, float d2, float eps = 1e-4f)
                => Mathf.Abs(DiffAngle(d1, d2)) <= eps;

            public static float ClampAngle(float degree, float min, float max)
                => Mathf.Clamp(NormalizeAngle(degree), min, max);

            public static float ClampAngle(float degree, float min, float max, ref bool clamped)
                => ClampAngle(degree, new ValueRange(min, max), ref clamped);

            public static float ClampAngle(float degree, ValueRange range)
                => ClampAngle(degree, range.min, range.max);

            public static float ClampAngle(float degree, ValueRange range, ref bool clamped)
            {
                float result = ClampAngle(degree, range.min, range.max);
                clamped = !IsEqualAngle(degree, result);
                return result;
            }

            public static bool IsSimilar(Quaternion q1, Quaternion q2, float acceptableRange = 1e-4f)
            {
                return 1.0f - Mathf.Abs(Quaternion.Dot(q1, q2)) < acceptableRange;
            }
        }
    }
}
