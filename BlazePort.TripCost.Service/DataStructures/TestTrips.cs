namespace BlazePort.TripCost.Service.DataStructures
{
    public class SingleTripSample
    {
        public static readonly Trip TestTrip = new Trip
        {
            VendorId = "VTS",
            RateCode = 1,
            PassengerCount = 1,
            //TripTime = 1140,
            TripDistance = 2.21f,
            PaymentType = "CRD",
            FareAmount = 0 // To predict. Actual/Observed = 9.97
        };
        //{
        //    VendorId = "VTS",
        //    RateCode = "1",
        //    PassengerCount = 1,
        //    //TripTime = 1140,
        //    TripDistance = 3.75f,
        //    PaymentType = "CRD",
        //    FareAmount = 0 // To predict. Actual/Observed = 15.5
        //};
    }
}