using BlazePort.Components;
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

        [Parameter] protected FlyoutPanel ConfigurationPanel { get; set; }

        string ArrivalLocation() => DestinationLocations.First(l => l.Id == int.Parse(form.SelectedDestinationId)).Name;
        string ArrivalPort() => DestinationPorts.First(l => l.Id == int.Parse(form.SelectedPortOfEntryId)).Name;
        protected string ArrivalTo => $"{ArrivalLocation()} - {ArrivalPort()}";
        string DepartureLocation() => DepartureLocations.First(l => l.Id == int.Parse(form.SelectedDepartureId)).Name;
        string DeparturePort() => DeparturePorts.First(l => l.Id == int.Parse(form.SelectedPortOfTravelId)).Name;
        protected string DepartureFrom => $"{DepartureLocation()} - {DeparturePort()}";
        protected bool IsValid { get; set; }
        protected override async Task OnInitAsync()
        {
            DepartureLocations = await dbContext.Locations.ToArrayAsync();
        }

        protected async Task OnLocationChanged(string selectedValue)
        {
            form.SelectedDepartureId = selectedValue;
            DeparturePorts = await GetPortsByLocation(selectedValue);
            form.SelectedPortOfTravelId = DeparturePorts[0].Id.ToString();
            await ClearDestinations(selectedValue);
        }

        protected void OnDeparturePortChanged(string selectedValue)
        {
            form.SelectedPortOfTravelId = selectedValue;
        }

        protected void OnPortOfEntryChanged(string selectedValue)
        {
            form.SelectedPortOfEntryId = selectedValue;
        }

        private async Task ClearDestinations(string selectedValue)
        {
            DestinationLocations = await dbContext.Locations.Where(loc => loc.Id != int.Parse(selectedValue)).ToArrayAsync();
            form.SelectedDestinationId = "";
            DestinationPorts = null;
        }

        protected async Task OnDestinationChanged(string selectedValue)
        {
            form.SelectedDestinationId = selectedValue;
            DestinationPorts = await GetPortsByLocation(selectedValue);
            form.SelectedPortOfEntryId = DestinationPorts[0].Id.ToString();
        }

        private async Task<PortDetails[]> GetPortsByLocation(string selectedValue) =>
            await dbContext.PortDetails
                .Where(p => p.LocationId == int.Parse(selectedValue))
                .ToArrayAsync();

        public async Task OnTripEstimateTripCost()
        {
            IsValid = true;
            Trip trip = new Trip();

            trip.PassengerCount = form.PassengerCount;
            trip.PaymentType = form.paymentType;
            trip.TripDistance = form.tripDistance;
            trip.VendorId = form.vendor;
            trip.RateCode = form.rateCode.ToString();

            totalPrice = tripCostService.PredictFare(trip).FareAmount;
            await ConfigurationPanel.HideAsync();
        }

        public void InvalidSubmit()
        {
            IsValid = false;
        }

        public async Task ShowConfigurationPanel()
        {
            await ConfigurationPanel.ShowAsync();
        }
    }
}
