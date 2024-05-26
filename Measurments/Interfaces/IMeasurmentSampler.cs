using System;
using System.Collections.Generic;

namespace Measurements.Interfaces
{
    public interface IMeasurementSampler
    {
        Dictionary<MeasurementType, List<Measurement>> Sample(DateTime startOfSampling, List<Measurement> unsampledMeasurements);
    }
}