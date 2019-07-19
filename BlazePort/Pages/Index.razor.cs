using BlazePort.Data;
using BlazePort.Models.FormModels;
using BlazePort.TripCost.Service;
using BlazePort.TripCost.Service.DataStructures;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlazePort.Pages
{
    public class IndexBase : ComponentBase
    {
        [Inject] BlazePortContext dbContext { get; set; }

        [Inject] ITripCostPredictionService tripCostService { get; set; }

        protected LocationDetails[] DepartureLocations;

        protected LocationDetails[] DestinationLocations;

        protected PortDetails[] DeparturePorts;

        protected PortDetails[] DestinationPorts;

        protected float totalPrice;

        protected TripConfiguration form = new TripConfiguration();

        protected override async Task OnInitAsync()
        {
            DepartureLocations = await dbContext.Locations.ToArrayAsync();
        }

        protected async Task OnLocationChanged(string selectedValue)
        {
            form.SelectedDeparture = selectedValue;
            DeparturePorts = await GetPortsByLocation(selectedValue);
            form.SelectedPortOfTravel = DeparturePorts[0].Id.ToString();
            await ClearDestinations(selectedValue);
        }

        protected void OnDeparturePortChanged(string selectedValue)
        {
            form.SelectedPortOfTravel = selectedValue;
        }

        protected void OnPortOfEntryChanged(string selectedValue)
        {
            form.SelectedPortOfEntry = selectedValue;
        }

        private async Task ClearDestinations(string selectedValue)
        {
            DestinationLocations = await dbContext.Locations.Where(loc => loc.Id != int.Parse(selectedValue)).ToArrayAsync();
            form.SelectedDestination = "";
            DestinationPorts = null;
        }

        protected async Task OnDestinationChanged(string selectedValue)
        {
            form.SelectedDestination = selectedValue;
            DestinationPorts = await GetPortsByLocation(selectedValue);
            form.SelectedPortOfEntry = DestinationPorts[0].Id.ToString();
        }

        private async Task<PortDetails[]> GetPortsByLocation(string selectedValue) =>
            await dbContext.PortDetails
                .Where(p => p.LocationId == int.Parse(selectedValue))
                .ToArrayAsync();

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

        public void InvalidSubmit()
        {
            Console.WriteLine("invalid submit");
        }
    }
}
