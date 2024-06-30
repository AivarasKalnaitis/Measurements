using Measurements.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Measurements
{
    public class IntervalManager : IIntervalManager
    {
        public const int IntervalMinutes = 5;

        public bool IsInPreviousInterval(DateTime measurementTime, DateTime currentIntervalStart) => measurementTime == currentIntervalStart;

        public bool IsInNextInterval(DateTime measurementTime, DateTime currentIntervalStart) => measurementTime >= currentIntervalStart.AddMinutes(IntervalMinutes);

        public void AddLastMeasurementIfExists(List<Measurement> sampledMeasurements, ref Measurement lastMeasurement, DateTime currentIntervalStart)
        {
            if (lastMeasurement != null)
            {
                var measurmentTimeToSet = DateTime.MinValue;

                if (currentIntervalStart.AddMinutes(IntervalMinutes) < lastMeasurement.MeasurementTime)
                {
                    while (currentIntervalStart < lastMeasurement.MeasurementTime)
                    {
                        measurmentTimeToSet = currentIntervalStart.AddMinutes(IntervalMinutes);
                        currentIntervalStart = measurmentTimeToSet;
                    }
                }


                var lastMeasurementTime = lastMeasurement.MeasurementTime;
                var duplicateTimedMeasurement = sampledMeasurements.FirstOrDefault(x => x.MeasurementTime == lastMeasurementTime);
                lastMeasurement.MeasurementTime = measurmentTimeToSet == DateTime.MinValue ? currentIntervalStart.AddMinutes(IntervalMinutes) : measurmentTimeToSet;

                if (duplicateTimedMeasurement != null)
                {
                    var duplicateMeasurementIndex = sampledMeasurements.IndexOf(duplicateTimedMeasurement);
                    sampledMeasurements[duplicateMeasurementIndex] = lastMeasurement;
                }
                else
                {
                    sampledMeasurements.Add(lastMeasurement);
                }

                lastMeasurement = null;
            }
        }

        public DateTime AdvanceToNextInterval(DateTime currentIntervalStart, DateTime measurementTime)
        {
            while (measurementTime > currentIntervalStart.AddMinutes(IntervalMinutes))
            {
                currentIntervalStart = currentIntervalStart.AddMinutes(IntervalMinutes);
            }

            return currentIntervalStart;
        }

        public DateTime AdvanceToNextIntervalBoundary(DateTime currentIntervalStart, DateTime measurementTime)
        {
            if (measurementTime >= currentIntervalStart.AddMinutes(IntervalMinutes))
            {
                currentIntervalStart = currentIntervalStart.AddMinutes(IntervalMinutes);
            }

            return currentIntervalStart;
        }
    }
}