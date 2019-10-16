using BlazePort.Components;
using BlazePort.Pages.Index;
using BlazePort.TripCost.Service;
using BlazePort.TripCost.Service.DataStructures;
using BlazorSize;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazePort.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] ResizeListener ResizeListener { get; set; }
        [Inject] ITripCostPredictionService TripCostService { get; set; }

        protected string ConfigurationPanelWidth = "100%";

        protected float totalPrice;

        [Parameter] public FlyoutPanel ConfigurationPanel { get; set; }

        [Parameter] public TripConfigurationModel TripConfiguration { get; set; } = new TripConfigurationModel();

        public void InvalidSubmit()
        {
            totalPrice = 0;
        }

        public async Task ShowConfigurationPanel()
        {
            await ConfigurationPanel.ShowAsync();
        }

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await ResizeListener.WatchResize(
                (browserWindowSize) =>
                {
                    if (browserWindowSize.MediaQueryMatched)
                    {
                        //is tablet or larger
                        ConfigurationPanelWidth = "50%";
                        TripConfiguration.DropDownWidth = "calc(50% - 32px)";
                    } else
                    {
                        //is smaller than tablet
                        ConfigurationPanelWidth = "100%";
                        TripConfiguration.DropDownWidth = "calc(100% - 32px)";
                    }
                    StateHasChanged();
                }
                ,
                "(min-width: 768px)");
            }
        }

    }
}
