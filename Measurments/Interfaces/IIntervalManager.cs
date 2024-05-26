using System;
using System.Collections.Generic;

namespace Measurements.Interfaces
{
    public interface IIntervalManager
    {
        bool IsInNextInterval(DateTime measurementTime, DateTime currentIntervalStart);

        void AddLastMeasurementIfExists(List<Measurement> sampledMeasurements, ref Measurement lastMeasurement);

        DateTime AdvanceToNextInterval(DateTime currentIntervalStart, DateTime measurementTime);
    }
}