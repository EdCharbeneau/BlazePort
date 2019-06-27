using BlazePort.TripCost.Service.DataStructures;

namespace BlazePort.TripCost.Service
{
    public interface ITripCostPredictionService
    {
        string ModelPath { get; }

        TripCostPrediction PredictFare(Trip taxiTrip);
    }
}