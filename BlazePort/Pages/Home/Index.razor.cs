using BlazePort.Components;
using BlazePort.TripCost.Service;
using BlazePort.TripCost.Service.DataStructures;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using BlazorSize;
using System;

namespace BlazePort.Pages.Home
{
    public partial class Index : IDisposable
    {
        [Inject] ResizeListener ResizeListener { get; set; }
        [Inject] ITripCostPredictionService TripCostService { get; set; }

        bool IsMediumUpMedia;

        protected string ConfigurationPanelWidth => IsMediumUpMedia ? "50%" : "100%";

        protected float totalPrice;

        [Parameter] public FlyoutPanel ConfigurationPanel { get; set; }

        [Parameter] public TripConfigurationModel TripConfiguration { get; set; } = new TripConfigurationModel();

        public void InvalidSubmit() => 
            totalPrice = 0;

        public async Task ShowConfigurationPanel() =>
            await ConfigurationPanel.ShowAsync();

        public async Task OnTripEstimateTripCost()
        {
            Trip trip = new Trip
            {
                PassengerCount = TripConfiguration.PassengerCount,
                PaymentType = TripConfiguration.paymentType,
                TripDistance = TripConfiguration.TripDistance,
                VendorId = TripConfiguration.vendor,
                RateCode = TripConfiguration.rateCode.ToString()
            };

            totalPrice = TripCostService.PredictFare(trip).FareAmount;
            await ConfigurationPanel.HideAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {

            if (firstRender)
            {
                ResizeListener.OnResized += WindowResized;
            }
        }

        void IDisposable.Dispose()
        {
            ResizeListener.OnResized -= WindowResized;
        }

        async void WindowResized(object _, BrowserWindowSize window)
        {
            IsMediumUpMedia = await ResizeListener.MatchMedia(Breakpoints.MediumUp);
            StateHasChanged();
        }
    }
}
