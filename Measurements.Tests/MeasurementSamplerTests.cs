using System;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace Measurements.Tests
{
    public class MeasurementSamplerTests
    {
        [Fact]
        public void Sample_ShouldReturnEmpty_WhenNoMeasurementsProvided()
        {
            var intervalManagerMock = new Mock<IIntervalManager>();
            var sampler = new MeasurementSampler(intervalManagerMock.Object);
            var startOfSampling = new DateTime(2023, 5, 23, 10, 0, 0);
            var measurements = new List<Measurement>();

            var result = sampler.Sample(startOfSampling, measurements);

            Assert.Empty(result);
        }

        [Fact]
        public void Sample_ShouldGroupByTypeAndSampleCorrectly()
        {
            var intervalManager = new IntervalManager();
            var sampler = new MeasurementSampler(intervalManager);
            var startOfSampling = new DateTime(2023, 5, 23, 10, 0, 0);
            var measurements = new List<Measurement>
            {
                new Measurement(new DateTime(2023, 5, 23, 10, 4, 45), 35.79, MeasurementType.TEMP),
                new Measurement(new DateTime(2023, 5, 23, 10, 1, 18), 98.78, MeasurementType.SPO2),
                new Measurement(new DateTime(2023, 5, 23, 10, 9, 7), 35.01, MeasurementType.TEMP),
                new Measurement(new DateTime(2023, 5, 23, 10, 3, 34), 96.49, MeasurementType.SPO2),
                new Measurement(new DateTime(2023, 5, 23, 10, 2, 1), 35.82, MeasurementType.TEMP),
                new Measurement(new DateTime(2023, 5, 23, 10, 5, 0), 97.17, MeasurementType.SPO2),
                new Measurement(new DateTime(2023, 5, 23, 10, 5, 1), 95.08, MeasurementType.SPO2)
            };

            var result = sampler.Sample(startOfSampling, measurements);

            Assert.Equal(2, result.Count);

            var tempMeasurements = result[MeasurementType.TEMP];
            Assert.Equal(2, tempMeasurements.Count);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 5, 0), tempMeasurements[0].MeasurementTime);
            Assert.Equal(35.79, tempMeasurements[0].MeasurementValue);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 10, 0), tempMeasurements[1].MeasurementTime);
            Assert.Equal(35.01, tempMeasurements[1].MeasurementValue);

            var spo2Measurements = result[MeasurementType.SPO2];
            Assert.Equal(2, spo2Measurements.Count);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 5, 0), spo2Measurements[0].MeasurementTime);
            Assert.Equal(97.17, spo2Measurements[0].MeasurementValue);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 10, 0), spo2Measurements[1].MeasurementTime);
            Assert.Equal(95.08, spo2Measurements[1].MeasurementValue);
        }

        [Fact]
        public void Sample_ShouldHandleMeasurementsOnIntervalBorder()
        {
            var intervalManager = new IntervalManager();
            var sampler = new MeasurementSampler(intervalManager);
            var startOfSampling = new DateTime(2023, 5, 23, 10, 0, 0);
            var measurements = new List<Measurement>
            {
                new Measurement(new DateTime(2023, 5, 23, 10, 5, 0), 97.17, MeasurementType.SPO2),
                new Measurement(new DateTime(2023, 5, 23, 10, 10, 0), 95.08, MeasurementType.SPO2)
            };

            var result = sampler.Sample(startOfSampling, measurements);

            var spo2Measurements = result[MeasurementType.SPO2];
            Assert.Equal(2, spo2Measurements.Count);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 5, 0), spo2Measurements[0].MeasurementTime);
            Assert.Equal(97.17, spo2Measurements[0].MeasurementValue);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 10, 0), spo2Measurements[1].MeasurementTime);
            Assert.Equal(95.08, spo2Measurements[1].MeasurementValue);
        }

        [Fact]
        public void Sample_ShouldSkipEmptyIntervals()
        {
            var intervalManager = new IntervalManager();
            var sampler = new MeasurementSampler(intervalManager);
            var startOfSampling = new DateTime(2023, 5, 23, 10, 0, 0);
            var measurements = new List<Measurement>
            {
                new Measurement(new DateTime(2023, 5, 23, 10, 1, 0), 35.7, MeasurementType.TEMP),
                new Measurement(new DateTime(2023, 5, 23, 10, 12, 0), 35.5, MeasurementType.TEMP)
            };

            var result = sampler.Sample(startOfSampling, measurements);

            var tempMeasurements = result[MeasurementType.TEMP];
            Assert.Equal(2, tempMeasurements.Count);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 5, 0), tempMeasurements[0].MeasurementTime);
            Assert.Equal(35.7, tempMeasurements[0].MeasurementValue);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 15, 0), tempMeasurements[1].MeasurementTime);
            Assert.Equal(35.5, tempMeasurements[1].MeasurementValue);
        }

        [Fact]
        public void Sample_ShouldHandleMeasurementsWithSameTimestamp()
        {
            var intervalManager = new IntervalManager();
            var sampler = new MeasurementSampler(intervalManager);
            var startOfSampling = new DateTime(2023, 5, 23, 10, 0, 0);
            var measurements = new List<Measurement>
            {
                new Measurement(new DateTime(2023, 5, 23, 10, 5, 0), 97.17, MeasurementType.SPO2),
                new Measurement(new DateTime(2023, 5, 23, 10, 5, 0), 95.08, MeasurementType.SPO2)
            };

            var result = sampler.Sample(startOfSampling, measurements);

            var spo2Measurements = result[MeasurementType.SPO2];
            Assert.Single(spo2Measurements);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 5, 0), spo2Measurements[0].MeasurementTime);
            Assert.Equal(95.08, spo2Measurements[0].MeasurementValue); // Last value with same timestamp should be taken
        }

        [Fact]
        public void Sample_ShouldHandleMeasurementsExactlyAtIntervalStart()
        {
            var intervalManager = new IntervalManager();
            var sampler = new MeasurementSampler(intervalManager);
            var startOfSampling = new DateTime(2023, 5, 23, 10, 0, 0);
            var measurements = new List<Measurement>
            {
                new Measurement(new DateTime(2023, 5, 23, 10, 0, 0), 97.17, MeasurementType.SPO2),
                new Measurement(new DateTime(2023, 5, 23, 10, 5, 0), 95.08, MeasurementType.SPO2)
            };

            var result = sampler.Sample(startOfSampling, measurements);

            var spo2Measurements = result[MeasurementType.SPO2];
            Assert.Equal(2, spo2Measurements.Count);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 0, 0), spo2Measurements[0].MeasurementTime);
            Assert.Equal(97.17, spo2Measurements[0].MeasurementValue);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 5, 0), spo2Measurements[1].MeasurementTime);
            Assert.Equal(95.08, spo2Measurements[1].MeasurementValue);
        }

        [Fact]
        public void Sample_ShouldHandleNonSequentialMeasurements()
        {
            var intervalManager = new IntervalManager();
            var sampler = new MeasurementSampler(intervalManager);
            var startOfSampling = new DateTime(2023, 5, 23, 10, 0, 0);
            var measurements = new List<Measurement>
            {
                new Measurement(new DateTime(2023, 5, 23, 10, 9, 0), 35.5, MeasurementType.TEMP),
                new Measurement(new DateTime(2023, 5, 23, 10, 2, 0), 35.8, MeasurementType.TEMP),
                new Measurement(new DateTime(2023, 5, 23, 10, 1, 0), 36.0, MeasurementType.TEMP)
            };

            var result = sampler.Sample(startOfSampling, measurements);

            var tempMeasurements = result[MeasurementType.TEMP];
            Assert.Equal(2, tempMeasurements.Count);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 5, 0), tempMeasurements[0].MeasurementTime);
            Assert.Equal(36.0, tempMeasurements[0].MeasurementValue);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 10, 0), tempMeasurements[1].MeasurementTime);
            Assert.Equal(35.5, tempMeasurements[1].MeasurementValue);
        }

        [Fact]
        public void Sample_ShouldHandleMultipleTypesWithDifferentIntervals()
        {
            var intervalManager = new IntervalManager();
            var sampler = new MeasurementSampler(intervalManager);
            var startOfSampling = new DateTime(2023, 5, 23, 10, 0, 0);
            var measurements = new List<Measurement>
            {
                new Measurement(new DateTime(2023, 5, 23, 10, 1, 0), 36.0, MeasurementType.TEMP),
                new Measurement(new DateTime(2023, 5, 23, 10, 1, 0), 98.0, MeasurementType.SPO2),
                new Measurement(new DateTime(2023, 5, 23, 10, 6, 0), 35.8, MeasurementType.TEMP),
                new Measurement(new DateTime(2023, 5, 23, 10, 6, 0), 97.5, MeasurementType.SPO2)
            };

            var result = sampler.Sample(startOfSampling, measurements);

            var tempMeasurements = result[MeasurementType.TEMP];
            var spo2Measurements = result[MeasurementType.SPO2];

            Assert.Equal(2, tempMeasurements.Count);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 5, 0), tempMeasurements[0].MeasurementTime);
            Assert.Equal(36.0, tempMeasurements[0].MeasurementValue);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 10, 0), tempMeasurements[1].MeasurementTime);
            Assert.Equal(35.8, tempMeasurements[1].MeasurementValue);

            Assert.Equal(2, spo2Measurements.Count);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 5, 0), spo2Measurements[0].MeasurementTime);
            Assert.Equal(98.0, spo2Measurements[0].MeasurementValue);
            Assert.Equal(new DateTime(2023, 5, 23, 10, 10, 0), spo2Measurements[1].MeasurementTime);
            Assert.Equal(97.5, spo2Measurements[1].MeasurementValue);
        }
    }
}
