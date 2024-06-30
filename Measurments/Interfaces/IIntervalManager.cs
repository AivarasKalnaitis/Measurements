using System;
using System.Collections.Generic;

namespace Measurements.Interfaces
{
    public interface IIntervalManager
    {
        bool IsInPreviousInterval(DateTime measurementTime, DateTime currentIntervalStart);
        bool IsInNextInterval(DateTime measurementTime, DateTime currentIntervalStart);

        void AddLastMeasurementIfExists(List<Measurement> sampledMeasurements, ref Measurement lastMeasurement, DateTime currentIntervalStart);

        DateTime AdvanceToNextInterval(DateTime currentIntervalStart, DateTime measurementTime);
        public DateTime AdvanceToNextIntervalBoundary(DateTime currentIntervalStart, DateTime measurementTime);
    }
}