using BlazePort.Data;
using Microsoft.AspNetCore.Components;
using BlazePort.Services;
using BlazePort.TripCost.Service;
using BlazePort.TripCost.Service.DataStructures;
using System.Linq;
using BlazePort.Models.FormModels;

namespace BlazePort.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] LocationService locationService { get; set; }

        [Inject] ITripCostPredictionService tripCostService { get; set; }

        protected LocationDetails[] Locations;

        protected float totalPrice;

        protected TripConfiguration form = new TripConfiguration();

        protected override void OnInit()
        {
            Locations = locationService.GetLocations().ToArray();
        }

        public void OnTripEstimateTripCost()
        {
            Trip trip = new Trip();

            trip.PassengerCount = form.PassengerCount;
            trip.PaymentType = form.paymentType;
            trip.TripDistance = form.tripDistance;
            trip.VendorId = form.vendor;
            trip.RateCode = form.rateCode.ToString();

            totalPrice = tripCostService.PredictFare(trip).FareAmount;
        }
    }
}
