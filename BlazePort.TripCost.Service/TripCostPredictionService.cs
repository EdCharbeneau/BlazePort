using Microsoft.ML;
using System.IO;
using BlazePort.TripCost.Service.DataStructures;

namespace BlazePort.TripCost.Service
{
    public class TripCostPredictionService : ITripCostPredictionService
    {
        public string ModelPath { get; }
        public TripCostPredictionService(string modelPath) => ModelPath = modelPath;

        public TripCostPrediction PredictFare(Trip trip)
        {
            MLContext mlContext = new MLContext(seed: 0);

            ITransformer trainedModel;
            DataViewSchema modelSchema;

            using (var stream = new FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                trainedModel = mlContext.Model.Load(stream, out modelSchema);
            }

            // Create prediction engine related to the loaded trained model
            var predEngine = mlContext.Model.CreatePredictionEngine<Trip, TripCostPrediction>(trainedModel);

            //Score
            var prediction = predEngine.Predict(trip);

            prediction.FareAmount *= 1000;

            return prediction;
        }
    }
}
