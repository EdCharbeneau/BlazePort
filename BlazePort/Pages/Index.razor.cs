using BlazePort.Data;
using BlazePort.Models.FormModels;
using BlazePort.TripCost.Service;
using BlazePort.TripCost.Service.DataStructures;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BlazePort.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] BlazePortContext dbContext { get; set; }

        [Inject] ITripCostPredictionService tripCostService { get; set; }

        protected LocationDetails[] Locations;

        protected float totalPrice;

        protected TripConfiguration form = new TripConfiguration();

        protected override async Task OnInitAsync()
        {
            Locations = await dbContext.Locations.ToArrayAsync();
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
