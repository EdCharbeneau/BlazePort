using Microsoft.ML.Data;

namespace BlazePort.TripCost.Service.DataStructures
{
    public class TripCostPrediction
    {
        [ColumnName("Score")]
        public float FareAmount;
    }
}