using BlazePort.TripCost.Service.DataStructures;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlazePort.TripCost.Service
{
    public static class PredictionAnalysis
    {
        public static IEnumerable<Trip> GetTestDataFromCsv(string testDataPath, int numMaxRecords)
        {
            IEnumerable<Trip> records =
                File.ReadAllLines(testDataPath)
                .Skip(1)
                .Select(x => x.Split(','))
                .Select(x => new Trip()
                {
                    VendorId = x[0],
                    RateCode = float.Parse(x[1]),
                    PassengerCount = float.Parse(x[2]),
                    //TripTime = float.Parse(x[3]),
                    TripDistance = float.Parse(x[4]),
                    PaymentType = x[5],
                    FareAmount = float.Parse(x[6])
                })
                .Take(numMaxRecords);

            return records;
        }

        public static TestDataResults GetAnalysis(IEnumerable<Trip> testData, PredictionEngine<Trip, TripCostPrediction> predictionEngine)
        {

            var result = testData.Select(t => new TestDataPoint()
            {
                Actual = t.FareAmount,
                Predicted = predictionEngine.Predict(t).FareAmount
            });
            return new TestDataResults(result);
        }
    }

    public class DataPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class TestDataResults
    {
        public TestDataResults() { }
        public TestDataResults(IEnumerable<TestDataPoint> resultSet) => ResultSet = resultSet;
        public IEnumerable<TestDataPoint> ResultSet { get; set; }
        public IEnumerable<DataPoint> MinimizedSquareError => GetMinimizedSquareError();
        public double RSquared { get; set; }
        public double RootMeansSquaredError { get; set; }
        public double MeanSquaredError { get; set; }
        public double MeanAbsoluteError { get; set; }

        private IEnumerable<DataPoint> GetMinimizedSquareError()
        {
            var funcY = GetRegressionFunction();
            var min = ResultSet.Min(x => x.Actual);
            var max = ResultSet.Max(x => x.Actual);
            var a = new DataPoint { X = min, Y = funcY(min) };
            var b = new DataPoint { X = max, Y = funcY(max) };
            return new[] { a, b };
        }

        private Func<double, double> GetRegressionFunction()
        {
            // Regression Line calculation explanation:
            // https://www.khanacademy.org/math/statistics-probability/describing-relationships-quantitative-data/more-on-regression/v/regression-line-example

            double xyMultiTotal = ResultSet.Sum(r => r.Actual * r.Predicted);
            double xSquareTotal = ResultSet.Sum(r => r.Actual * r.Actual);

            double meanX = ResultSet.Average(r => r.Actual);
            double meanY = ResultSet.Average(r => r.Predicted);

            double meanXY = ResultSet.Average(r => r.Actual * r.Predicted);
            double meanXsquare = ResultSet.Average(r => r.Actual * r.Actual);

            //double mslope = (meanXY - meanX * meanY) / meanXsquare - (meanX * meanX);

            double mslope = ((meanX * meanY) - meanXY) / ((meanX * meanX) - meanXsquare);

            double bintercept = meanY - (mslope * meanX);

            //Generic function for Y for the regression line
            // y = (m * x) + b;

            ////Function for Y1 in the line
            return (double x) => (mslope * x) + bintercept;
        }
    }
}
