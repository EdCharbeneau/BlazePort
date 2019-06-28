using BlazePort.Data;
using Microsoft.AspNetCore.Components;
using BlazePort.Services;
using BlazePort.TripCost.Service;
using BlazePort.TripCost.Service.DataStructures;
using System.Linq;

namespace BlazePort.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] LocationService locationService { get; set; }

        [Inject] ITripCostPredictionService tripCostService { get; set; }

        protected LocationDetails[] Locations;

        protected float totalPrice;

        protected MyForm form = new MyForm();

        public class MyForm
        {
            public string SelectedLocation { get; set; }
            public string SelectedDestination { get; set; }
        }

        protected override void OnInit()
        {
            Locations = locationService.GetLocations().ToArray();
            Trip trip = new Trip()
            {
                PassengerCount = 1, //Form
                RateCode = "4", //service
                PaymentType = "CSH", //?
                VendorId = "VTS", //Form
                TripDistance = 2.1F //caclculated 
            };

            totalPrice = tripCostService.PredictFare(trip).FareAmount;

        }
    }
}
