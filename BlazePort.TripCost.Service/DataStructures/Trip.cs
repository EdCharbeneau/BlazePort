using Microsoft.ML.Data;

namespace BlazePort.TripCost.Service.DataStructures
{
    public class Trip
    {
        [ColumnName("vendor_id"), LoadColumn(0)]
        public string VendorId;

        [ColumnName("rate_code"), LoadColumn(1)]
        public float RateCode;

        [ColumnName("passenger_count"), LoadColumn(2)]
        public float PassengerCount;

        [ColumnName("trip_time_in_secs"), LoadColumn(3)]
        public float Trip_time_in_secs { get; set; }

        [ColumnName("trip_distance"), LoadColumn(4)]
        public float TripDistance;

        [ColumnName("payment_type"), LoadColumn(5)]
        public string PaymentType;

        [ColumnName("fare_amount"), LoadColumn(6)]
        public float FareAmount;

    }

}