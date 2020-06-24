using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using PLplot;
using System.Diagnostics;
using Microsoft.ML;
//using Microsoft.ML.Data;
//using Microsoft.Data.DataView;
//using static Microsoft.ML.Transforms.NormalizingEstimator;
using Common;
using BlazePort.TripCost.Service.DataStructures;
using BlazePort.TripCost.Service;
using Microsoft.ML.Data;

namespace BlazePort.TripCost.Trainer
{
    internal class Program
    {
        //private static string AppPath => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);

        private static readonly string RootPath = Path.GetFullPath(@"../../../../BlazePort.TripCost.Trainer");
        private static readonly string TrainDataPath = Path.Combine(RootPath, "Data", "taxi-fare-train.csv");
        private static readonly string TestDataPath = Path.Combine(RootPath, "Data", "taxi-fare-test.csv");
        private static readonly string ModelPath = Path.Combine(RootPath, "MLModels", "TripCostModel.zip");

        static void Main(string[] args) //If args[0] == "svg" a vector-based chart will be created instead a .png chart
        {

            //Create ML Context with seed for repeteable/deterministic results
            MLContext mlContext = new MLContext(seed: 0);

            // Create, Train, Evaluate and Save a model
            BuildTrainEvaluateAndSaveModel(mlContext);

            // Paint regression distribution chart for a number of elements read from a Test DataSet file
            // TODO: Remove PLplot and replace with something more stable https://github.com/surban/PLplotNet/issues/2
            // PlotRegressionChart(mlContext, TestDataPath, 100, args);

            Console.WriteLine("Press any key to exit..");
            Console.ReadLine();
        }

        private static ITransformer Train(MLContext mlContext, IDataView trainingDataView, IDataView testDataView)
        {

            // STEP 2: Common data process configuration with pipeline data transformations
            var dataProcessPipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(Trip.FareAmount))
                            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "VendorIdEncoded", inputColumnName: nameof(Trip.VendorId)))
                            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "RateCodeEncoded", inputColumnName: nameof(Trip.RateCode)))
                            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "PaymentTypeEncoded", inputColumnName: nameof(Trip.PaymentType)))
                            .Append(mlContext.Transforms.NormalizeMeanVariance(nameof(Trip.PassengerCount)))
                            .Append(mlContext.Transforms.NormalizeMeanVariance(outputColumnName: nameof(Trip.TripDistance)))
                            .Append(mlContext.Transforms.Concatenate("Features", "VendorIdEncoded", "RateCodeEncoded", "PaymentTypeEncoded", nameof(Trip.PassengerCount)
                            , nameof(Trip.TripDistance)));

            // (OPTIONAL) Peek data (such as 5 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
            ConsoleHelper.PeekDataViewInConsole(mlContext, trainingDataView, dataProcessPipeline, 5);
            ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "Features", trainingDataView, dataProcessPipeline, 5);

            // STEP 3: Set the training algorithm, then create and config the modelBuilder - Selected Trainer (SDCA Regression algorithm)                            
            //var trainer = mlContext.Regression.Trainers.StochasticDualCoordinateAscent(labelColumnName: "Label", featureColumnName: "Features");
            var trainer = mlContext.Regression.Trainers.FastTree();
            var trainingPipeline = dataProcessPipeline.Append(trainer);

            // STEP 4: Train the model fitting to the DataSet
            //The pipeline is trained on the dataset that has been loaded and transformed.
            Console.WriteLine("=============== Training the model ===============");
            return trainingPipeline.Fit(trainingDataView);
        }

        private static RegressionMetrics Evaluate(MLContext mlContext, ITransformer trainedModel, IDataView testDataView)
        {
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====");

            IDataView predictions = trainedModel.Transform(testDataView);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            ConsoleHelper.PrintRegressionMetrics("FastTree Regression Trainer", metrics);
            return metrics;
        }
        private static void BuildTrainEvaluateAndSaveModel(MLContext mlContext)
        {
            // Common data loading configuration
            IDataView baseTrainingDataView = mlContext.Data.LoadFromTextFile<Trip>(TrainDataPath, hasHeader: true, separatorChar: ',');
            IDataView testDataView = mlContext.Data.LoadFromTextFile<Trip>(TestDataPath, hasHeader: true, separatorChar: ',');
            IDataView trainingDataView = mlContext.Data.FilterRowsByColumn(baseTrainingDataView, nameof(Trip.FareAmount), lowerBound: 1, upperBound: 150);

            ITransformer trainedModel = Train(mlContext, trainingDataView, testDataView);
            // Evaluate the model and show accuracy stats
            var metrics = Evaluate(mlContext, trainedModel, testDataView);

            // Save/persist the trained model to a .ZIP file
            SaveModelToDisk(mlContext, trainedModel, trainingDataView);

            void SaveRegressionMetrics()
            {
                var predFunction = mlContext.Model.CreatePredictionEngine<Trip, TripCostPrediction>(trainedModel);
                var data = PredictionAnalysis.GetTestDataFromCsv(TestDataPath, 1000);
                var result = PredictionAnalysis.GetAnalysis(data, predFunction);
                // Save to JSON
                result.RSquared = metrics.RSquared;
                result.RootMeansSquaredError = metrics.RootMeanSquaredError;
                result.MeanSquaredError = metrics.MeanSquaredError;
                result.MeanAbsoluteError = metrics.MeanAbsoluteError;
                var json = System.Text.Json.JsonSerializer.Serialize(result);
                File.WriteAllText(Path.Combine(RootPath, "MLModels", "analysis.json"), json);
            }

            SaveRegressionMetrics();
        }

        private static void SaveModelToDisk(MLContext mlContext, ITransformer trainedModel, IDataView trainingDataView)
        {
            mlContext.Model.Save(trainedModel, trainingDataView.Schema, ModelPath);

            Console.WriteLine("The model is saved to {0}", ModelPath);
        }

        //private static void TestSinglePrediction()
        //{
        //    //Sample: 
        //    //vendor_id,rate_code,passenger_count,trip_time_in_secs,trip_distance,payment_type,fare_amount
        //    //VTS,1,1,1140,3.75,CRD,15.5

        //    var taxiTripSample = SingleTripSample.TestTrip;

        //    var service = new TripCostPredictionService(ModelPath);


        //    Console.WriteLine($"**********************************************************************");
        //    Console.WriteLine($"Predicted fare: {service.PredictFare(taxiTripSample).FareAmount:0.####}, actual fare: 15.5");
        //    Console.WriteLine($"**********************************************************************");
        //}

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }
    }

}