using BlazePort.TripCost.Service;
using BlazePort.TripCost.Service.DataStructures;
using Microsoft.AspNetCore.Components;

namespace BlazePort.Pages.Home
{
    public partial class Index
    {
        [Inject] ITripCostPredictionService TripCostService { get; set; }

        [Inject] TripConfigurationState State { get; set; }

        bool configurationPanelVisible;

        public void InvalidSubmit() => State.TotalPrice = 0;

        public void ShowConfigurationPanel() => configurationPanelVisible = true;

        public void OnTripEstimateTripCost()
        {
            Trip trip = new Trip
            {
                PassengerCount = State.TripConfiguration.PassengerCount,
                PaymentType = State.TripConfiguration.paymentType,
                TripDistance = State.TripConfiguration.TripDistance,
                VendorId = State.TripConfiguration.vendor,
                RateCode = State.TripConfiguration.rateCode
            };

            State.TotalPrice = TripCostService.PredictFare(trip).FareAmount;
            configurationPanelVisible = false;
        }

    }
}
