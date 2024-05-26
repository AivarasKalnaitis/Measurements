using Measurements.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Measurements
{
    public class MeasurementSampler : IMeasurementSampler
    {
        private readonly IIntervalManager _intervalManager;

        public MeasurementSampler(IIntervalManager intervalManager)
        {
            _intervalManager = intervalManager;
        }

        public Dictionary<MeasurementType, List<Measurement>> Sample(DateTime startOfSampling, List<Measurement> unsampledMeasurements)
        {
            var groupedByType = GroupByType(unsampledMeasurements);
            var result = new Dictionary<MeasurementType, List<Measurement>>();

            foreach (var group in groupedByType)
            {
                result[group.Key] = SampleMeasurements(startOfSampling, group.Value);
            }

            return result;
        }

        private Dictionary<MeasurementType, List<Measurement>> GroupByType(IEnumerable<Measurement> measurements)
        {
            return measurements.GroupBy(m => m.Type)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        private List<Measurement> SampleMeasurements(DateTime startOfSampling, IEnumerable<Measurement> measurements)
        {
            var sortedMeasurements = measurements.OrderBy(m => m.MeasurementTime).ToList();
            var sampledMeasurements = new List<Measurement>();

            var currentIntervalStart = startOfSampling;
            Measurement lastMeasurement = null;

            foreach (var measurement in sortedMeasurements)
            {
                if (_intervalManager.IsInNextInterval(measurement.MeasurementTime, currentIntervalStart))
                {
                    _intervalManager.AddLastMeasurementIfExists(sampledMeasurements, ref lastMeasurement);
                    currentIntervalStart = _intervalManager.AdvanceToNextInterval(currentIntervalStart, measurement.MeasurementTime);
                }

                lastMeasurement = measurement;
            }

            _intervalManager.AddLastMeasurementIfExists(sampledMeasurements, ref lastMeasurement);
            return sampledMeasurements;
        }
    }
}