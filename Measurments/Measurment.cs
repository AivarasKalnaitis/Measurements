using System;

namespace Measurements
{
    public enum MeasurementType
    {
        TEMP,
        SPO2,
        HEART_RATE
    }

    public class Measurement
    {
        public Measurement(DateTime measurementTime, double measurementValue, MeasurementType type)
        {
            MeasurementTime = measurementTime;
            MeasurementValue = measurementValue;
            Type = type;
        }

        public DateTime MeasurementTime { get; set; }

        public double MeasurementValue { get; set; }

        public MeasurementType Type { get; set; }
    }
}