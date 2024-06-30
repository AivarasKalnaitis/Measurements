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
            var groupedByType = unsampledMeasurements.GroupBy(m => m.Type).ToDictionary(g => g.Key, g => g.ToList());
            var result = new Dictionary<MeasurementType, List<Measurement>>();

            foreach (var group in groupedByType)
            {
                var sampledMeasurements = SampleMeasurements(startOfSampling, group.Value);
                result[group.Key] = sampledMeasurements;
            }

            return result;
        }

        private List<Measurement> SampleMeasurements(DateTime startOfSampling, IEnumerable<Measurement> measurements)
        {
            var sortedMeasurements = measurements.OrderBy(m => m.MeasurementTime).ToList();
            var sampledMeasurements = new List<Measurement>();

            var currentIntervalStart = startOfSampling;
            Measurement lastMeasurement = null;

            foreach (var measurement in sortedMeasurements)
            {
                if (_intervalManager.IsInPreviousInterval(measurement.MeasurementTime, currentIntervalStart))
                {
                    currentIntervalStart = currentIntervalStart.AddMinutes(-IntervalManager.IntervalMinutes);
                    lastMeasurement = measurement;
                    _intervalManager.AddLastMeasurementIfExists(sampledMeasurements, ref lastMeasurement, currentIntervalStart);
                    currentIntervalStart = _intervalManager.AdvanceToNextIntervalBoundary(currentIntervalStart, measurement.MeasurementTime);
                    continue;
                }

                if (_intervalManager.IsInNextInterval(measurement.MeasurementTime, currentIntervalStart))
                {
                    var timeDiff = measurement.MeasurementTime - currentIntervalStart;
                    if (timeDiff.TotalMinutes == IntervalManager.IntervalMinutes)
                    {
                        lastMeasurement = measurement;
                        _intervalManager.AddLastMeasurementIfExists(sampledMeasurements, ref lastMeasurement, currentIntervalStart);
                        currentIntervalStart = _intervalManager.AdvanceToNextIntervalBoundary(currentIntervalStart, measurement.MeasurementTime);
                        continue;
                    }

                    _intervalManager.AddLastMeasurementIfExists(sampledMeasurements, ref lastMeasurement, currentIntervalStart);
                    currentIntervalStart = _intervalManager.AdvanceToNextIntervalBoundary(currentIntervalStart, measurement.MeasurementTime);
                }

                lastMeasurement = measurement;
            }

            if (lastMeasurement != null)
            {
                _intervalManager.AddLastMeasurementIfExists(sampledMeasurements, ref lastMeasurement, currentIntervalStart);
            }

            return sampledMeasurements;
        }
    }
}