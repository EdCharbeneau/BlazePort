using BlazePort.Data;
using BlazePort.TripCost.Service;
using BlazePort.TripCost.Service.DataStructures;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlazePort.Pages.Home
{
    public partial class Index
    {
        [Inject] ITripCostPredictionService TripCostService { get; set; }
        [Inject] BlazePortContext dbContext { get; set; }
        [Inject] TripConfigurationState State { get; set; }

        TripConfigurationModel Model => State.TripConfiguration;

        bool configurationPanelVisible;

        public void InvalidSubmit() => State.TotalPrice = 0;

        public void ShowConfigurationPanel() => configurationPanelVisible = true;

        void OnTripEstimateTripCost()
        {
            Trip trip = new Trip
            {
                PassengerCount = Model.PassengerCount,
                PaymentType = Model.paymentType,
                TripDistance = Model.TripDistance,
                VendorId = Model.vendor,
                RateCode = Model.rateCode
            };

            State.TotalPrice = TripCostService.PredictFare(trip).FareAmount;
            configurationPanelVisible = false;
        }

        protected override async Task OnInitializedAsync()
        {
            Model.DepartureLocations = await dbContext.AllLocationDetails().ToArrayAsync();
        }

    }
}
