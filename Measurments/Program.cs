using System;
using System.Collections.Generic;

namespace Measurements
{
    public class Program
    {
        private static void Main()
        {
            var measurements = CreateSampleMeasurements();
            var startOfSampling = new DateTime(2017, 1, 3, 10, 0, 0);

            var intervalManager = new IntervalManager();
            var sampler = new MeasurementSampler(intervalManager);
            var sampledMeasurements = sampler.Sample(startOfSampling, measurements);

            DisplaySampledMeasurements(sampledMeasurements);
        }

        private static List<Measurement> CreateSampleMeasurements() =>
            new()
            {
                new Measurement(new DateTime(2017, 1, 3, 10, 4, 45), 35.79, MeasurementType.TEMP),
                new Measurement(new DateTime(2017, 1, 3, 10, 1, 18), 98.78, MeasurementType.SPO2),
                new Measurement(new DateTime(2017, 1, 3, 10, 9, 7), 35.01, MeasurementType.TEMP),
                new Measurement(new DateTime(2017, 1, 3, 10, 3, 34), 96.49, MeasurementType.SPO2),
                new Measurement(new DateTime(2017, 1, 3, 10, 2, 1), 35.82, MeasurementType.TEMP),
                new Measurement(new DateTime(2017, 1, 3, 10, 5, 0), 97.17, MeasurementType.SPO2),
                new Measurement(new DateTime(2017, 1, 3, 10, 5, 1), 95.08, MeasurementType.SPO2)
            };

        private static void DisplaySampledMeasurements(Dictionary<MeasurementType, List<Measurement>> sampledMeasurements)
        {
            foreach (var entry in sampledMeasurements)
            {
                Console.WriteLine($"{entry.Key}:");
                foreach (var measurement in entry.Value)
                {
                    Console.WriteLine($"  {measurement.MeasurementTime:yyyy-MM-ddTHH:mm:ss}, {measurement.MeasurementValue}");
                }
            }
        }
    }
}