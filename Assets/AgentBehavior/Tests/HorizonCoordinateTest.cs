using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MedicalTraining.Utilities.Test
{
    public class HorizonCoordinateTest : MonoBehaviour
    {
        [SerializeField] private Transform Origin;
        [SerializeField] private Transform TrackedObject;
        [SerializeField] private List<Transform> RemappedObjects;
        [SerializeField] private List<Transform> ReorientedObjects;

        [SerializeField] private ValueRange DistanceRange = new ValueRange(0.5f, 5.0f);
        [SerializeField] private bool DistanceRangeEnabled = false;

        [SerializeField] private ValueRange HFOVRange = new ValueRange(-60.0f, 60.0f, ValueRange.ValueType.Angle);
        [SerializeField] private bool HFOVRangeEnabled = false;

        [SerializeField] private ValueRange VFOVRange = new ValueRange(-30.0f, 30.0f, ValueRange.ValueType.Angle);
        [SerializeField] private bool VFOVRangeEnabled = false;

        [ReadOnly, SerializeField] float Azimuth;
        [ReadOnly, SerializeField] float Ascent;
        [ReadOnly, SerializeField] float Distance;
        [ReadOnly, SerializeField] bool Valid;
        [ReadOnly, SerializeField] bool Constrained;

        void Update()
        {
            Valid = !(!Origin) && !(!TrackedObject);
            if (!Valid)
                return;

            // calculate the coordinates
            HorizonCoordinates hc = new HorizonCoordinates(TrackedObject.position, Origin);
            Azimuth = hc.Azimuth;
            Ascent = hc.Ascent;
            Distance = hc.Distance;

            // check the constraints
            ValueRange? dc = DistanceRangeEnabled ? DistanceRange : null;
            ValueRange? hf = HFOVRangeEnabled     ? HFOVRange     : null;
            ValueRange? vf = VFOVRangeEnabled     ? VFOVRange     : null;
            Constrained = !hc.CheckConstraints(dc, hf, vf);

            // apply
            foreach (var RemappedObject in RemappedObjects)
            {
                RemappedObject.position = hc.Position();
            }

            foreach (var ReorientedObject in ReorientedObjects)
            {
                ReorientedObject.rotation = hc.ObserverOrientation();
            }
        }
    }
}
