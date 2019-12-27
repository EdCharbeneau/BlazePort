using BlazePort.Components;
using BlazePort.TripCost.Service;
using BlazePort.TripCost.Service.DataStructures;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazePort.Pages.Home
{
    public partial class Index
    {
        [Inject] ITripCostPredictionService TripCostService { get; set; }

        [Inject] TripConfigurationState State { get; set; }

        [Parameter] public FlyoutPanel ConfigurationPanel { get; set; }

        public void InvalidSubmit() => State.TotalPrice = 0;

        public async Task ShowConfigurationPanel() => await ConfigurationPanel.ShowAsync();

        public async Task OnTripEstimateTripCost()
        {
            Trip trip = new Trip
            {
                PassengerCount = State.TripConfiguration.PassengerCount,
                PaymentType = State.TripConfiguration.paymentType,
                TripDistance = State.TripConfiguration.TripDistance,
                VendorId = State.TripConfiguration.vendor,
                RateCode = State.TripConfiguration.rateCode.ToString()
            };

            State.TotalPrice = TripCostService.PredictFare(trip).FareAmount;
            await ConfigurationPanel.HideAsync();
        }

    }
}
