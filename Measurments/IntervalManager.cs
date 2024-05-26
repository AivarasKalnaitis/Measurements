using Measurements.Interfaces;
using System;
using System.Collections.Generic;

namespace Measurements
{
    public class IntervalManager : IIntervalManager
    {
        private const int IntervalMinutes = 5;

        public bool IsInNextInterval(DateTime measurementTime, DateTime currentIntervalStart) => measurementTime >= currentIntervalStart.AddMinutes(IntervalMinutes);

        public void AddLastMeasurementIfExists(List<Measurement> sampledMeasurements, ref Measurement lastMeasurement)
        {
            if (lastMeasurement != null)
            {
                sampledMeasurements.Add(lastMeasurement);
                lastMeasurement = null;
            }
        }

        public DateTime AdvanceToNextInterval(DateTime currentIntervalStart, DateTime measurementTime)
        {
            while (measurementTime >= currentIntervalStart.AddMinutes(IntervalMinutes))
            {
                currentIntervalStart = currentIntervalStart.AddMinutes(IntervalMinutes);
            }

            return currentIntervalStart;
        }
    }
}