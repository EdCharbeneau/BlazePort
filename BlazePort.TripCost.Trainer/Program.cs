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

            // Make a single test prediction loding the model from .ZIP file
            TestSinglePrediction();

            // Paint regression distribution chart for a number of elements read from a Test DataSet file
            PlotRegressionChart(mlContext, TestDataPath, 100, args);

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

        private static void Evaluate(MLContext mlContext, ITransformer trainedModel, IDataView testDataView)
        {
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====");

            IDataView predictions = trainedModel.Transform(testDataView);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            ConsoleHelper.PrintRegressionMetrics("FastTree Regression Trainer", metrics);
        }
        private static void BuildTrainEvaluateAndSaveModel(MLContext mlContext)
        {
            // Common data loading configuration
            IDataView baseTrainingDataView = mlContext.Data.LoadFromTextFile<Trip>(TrainDataPath, hasHeader: true, separatorChar: ',');
            IDataView testDataView = mlContext.Data.LoadFromTextFile<Trip>(TestDataPath, hasHeader: true, separatorChar: ',');
            IDataView trainingDataView = mlContext.Data.FilterRowsByColumn(baseTrainingDataView, nameof(Trip.FareAmount), lowerBound: 1, upperBound: 150);

            ITransformer trainedModel = Train(mlContext, trainingDataView, testDataView);

            // Evaluate the model and show accuracy stats
            Evaluate(mlContext, trainedModel, testDataView);

            // Save/persist the trained model to a .ZIP file
            SaveModelToDisk(mlContext, trainedModel, trainingDataView);

        }

        private static void SaveModelToDisk(MLContext mlContext, ITransformer trainedModel, IDataView trainingDataView)
        {
            mlContext.Model.Save(trainedModel, trainingDataView.Schema, ModelPath);

            Console.WriteLine("The model is saved to {0}", ModelPath);
        }

        private static void TestSinglePrediction()
        {
            //Sample: 
            //vendor_id,rate_code,passenger_count,trip_time_in_secs,trip_distance,payment_type,fare_amount
            //VTS,1,1,1140,3.75,CRD,15.5

            var taxiTripSample = SingleTripSample.TestTrip;

            var service = new TripCostPredictionService(ModelPath);


            Console.WriteLine($"**********************************************************************");
            Console.WriteLine($"Predicted fare: {service.PredictFare(taxiTripSample).FareAmount:0.####}, actual fare: 15.5");
            Console.WriteLine($"**********************************************************************");
        }

        private static void PlotRegressionChart(MLContext mlContext,
                                                string testDataSetPath,
                                                int numberOfRecordsToRead,
                                                string[] args)
        {
            ITransformer trainedModel;
            DataViewSchema modelSchema;
            using (var stream = new FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                trainedModel = mlContext.Model.Load(stream, out modelSchema);
            }

            // Create prediction engine related to the loaded trained model
            var predFunction = mlContext.Model.CreatePredictionEngine<Trip, TripCostPrediction>(trainedModel);

            string chartFileName = "";
            using (var pl = new PLStream())
            {
                // use SVG backend and write to SineWaves.svg in current directory
                if (args.Length == 1 && args[0] == "svg")
                {
                    pl.sdev("svg");
                    chartFileName = "TaxiRegressionDistribution.svg";
                    pl.sfnam(chartFileName);
                }
                else
                {
                    pl.sdev("pngcairo");
                    chartFileName = "TaxiRegressionDistribution.png";
                    pl.sfnam(chartFileName);
                }

                // use white background with black foreground
                pl.spal0("cmap0_alternate.pal");

                // Initialize plplot
                pl.init();

                // set axis limits
                const int xMinLimit = 0;
                const int xMaxLimit = 35; //Rides larger than $35 are not shown in the chart
                const int yMinLimit = 0;
                const int yMaxLimit = 35;  //Rides larger than $35 are not shown in the chart
                pl.env(xMinLimit, xMaxLimit, yMinLimit, yMaxLimit, AxesScale.Independent, AxisBox.BoxTicksLabelsAxes);

                // Set scaling for mail title text 125% size of default
                pl.schr(0, 1.25);

                // The main title
                pl.lab("Measured", "Predicted", "Distribution of Taxi Fare Prediction");

                // plot using different colors
                // see http://plplot.sourceforge.net/examples.php?demo=02 for palette indices
                pl.col0(1);

                int totalNumber = numberOfRecordsToRead;
                var testData = new TripCsvReader().GetDataFromCsv(testDataSetPath, totalNumber).ToList();

                //This code is the symbol to paint
                char code = (char)9;

                // plot using other color
                //pl.col0(9); //Light Green
                //pl.col0(4); //Red
                pl.col0(2); //Blue

                double yTotal = 0;
                double xTotal = 0;
                double xyMultiTotal = 0;
                double xSquareTotal = 0;

                for (int i = 0; i < testData.Count; i++)
                {
                    var x = new double[1];
                    var y = new double[1];

                    //Make Prediction
                    var FarePrediction = predFunction.Predict(testData[i]);

                    x[0] = testData[i].FareAmount;
                    y[0] = FarePrediction.FareAmount;

                    //Paint a dot
                    pl.poin(x, y, code);

                    xTotal += x[0];
                    yTotal += y[0];

                    double multi = x[0] * y[0];
                    xyMultiTotal += multi;

                    double xSquare = x[0] * x[0];
                    xSquareTotal += xSquare;

                    double ySquare = y[0] * y[0];

                    Console.WriteLine($"-------------------------------------------------");
                    Console.WriteLine($"Predicted : {FarePrediction.FareAmount}");
                    Console.WriteLine($"Actual:    {testData[i].FareAmount}");
                    Console.WriteLine($"- Distance:    {testData[i].TripDistance}");
                    Console.WriteLine($"- Payment:    {testData[i].PaymentType}");
                    Console.WriteLine($"- Passengers:    {testData[i].PassengerCount}");
                    Console.WriteLine($"- Rate Code:    {testData[i].RateCode}");
                    Console.WriteLine($"-------------------------------------------------");
                }

                // Regression Line calculation explanation:
                // https://www.khanacademy.org/math/statistics-probability/describing-relationships-quantitative-data/more-on-regression/v/regression-line-example

                double minY = yTotal / totalNumber;
                double minX = xTotal / totalNumber;
                double minXY = xyMultiTotal / totalNumber;
                double minXsquare = xSquareTotal / totalNumber;

                double m = ((minX * minY) - minXY) / ((minX * minX) - minXsquare);

                double b = minY - (m * minX);

                //Generic function for Y for the regression line
                // y = (m * x) + b;

                double x1 = 1;
                //Function for Y1 in the line
                double y1 = (m * x1) + b;

                double x2 = 39;
                //Function for Y2 in the line
                double y2 = (m * x2) + b;

                var xArray = new double[2];
                var yArray = new double[2];
                xArray[0] = x1;
                yArray[0] = y1;
                xArray[1] = x2;
                yArray[1] = y2;

                pl.col0(4);
                pl.line(xArray, yArray);

                // end page (writes output to disk)
                pl.eop();

                // output version of PLplot
                pl.gver(out var verText);
                Console.WriteLine("PLplot version " + verText);

            } // the pl object is disposed here

            // Open Chart File In Microsoft Photos App (Or default app, like browser for .svg)

            Console.WriteLine("Showing chart...");
            var p = new Process();
            string chartFileNamePath = @".\" + chartFileName;
            p.StartInfo = new ProcessStartInfo(chartFileNamePath)
            {
                UseShellExecute = true
            };
            p.Start();
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }
    }

    public class TripCsvReader
    {
        public IEnumerable<Trip> GetDataFromCsv(string dataLocation, int numMaxRecords)
        {
            IEnumerable<Trip> records =
                File.ReadAllLines(dataLocation)
                .Skip(1)
                .Select(x => x.Split(','))
                .Select(x => new Trip()
                {
                    VendorId = x[0],
                    RateCode = x[1],
                    PassengerCount = float.Parse(x[2]),
                    //TripTime = float.Parse(x[3]),
                    TripDistance = float.Parse(x[4]),
                    PaymentType = x[5],
                    FareAmount = float.Parse(x[6])
                })
                .Take<Trip>(numMaxRecords);

            return records;
        }
    }

}