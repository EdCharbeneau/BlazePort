using Microsoft.ML.Data;

namespace BlazePort.TripCost.Service.DataStructures
{
    public class TripCostPrediction
    {
        [ColumnName("Score")]
        public float FareAmount;
    }

    public class TestDataPoint
    {
        [ColumnName("fare_amount"), LoadColumn(6)]
        public float Actual { get; set; }
        [ColumnName("Score")]
        public float Predicted { get; set; }
    }
}