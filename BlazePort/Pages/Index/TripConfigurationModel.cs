using BlazePort.Data;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace BlazePort.Pages.Index
{
    public class TripConfigurationModel
    {
        public DateTime TripStart { get; set; } = DateTime.Now.AddDays(1);

        [Required]
        [Range(1, 4)]
        public int PassengerCount { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SelectedDepartureLocationId
        {
            get => selectedDepartureLocationId;
            set
            {
                selectedDepartureLocationId = value;
                if (!string.IsNullOrEmpty(value))
                {
                    DepartureLocation = DepartureLocations.First(l => l.Id == int.Parse(value));
                    SelectedDeparturePortId = DepartureLocation.Ports.First().Id.ToString();
                    ClearDestinations(value);
                }
            }
        }

        private void ClearDestinations(string selectedValue)
        {
            ArrivalLocations = DepartureLocations.Where(loc => loc.Id != int.Parse(selectedValue)).ToArray();
            SelectedArrivalLocationId = ArrivalLocations.First().Id.ToString();
        }

        [Required(AllowEmptyStrings = false)]
        public string SelectedDeparturePortId
        {
            get => selectedDeparturePortId;
            set
            {
                selectedDeparturePortId = value;
                if (!string.IsNullOrEmpty(value))
                    DeparturePort = DepartureLocation.Ports.First(p => p.Id == int.Parse(value));
            }
        }

        [Required(AllowEmptyStrings = false)]
        public string SelectedArrivalLocationId
        {
            get => selectedArrivalLocationId;
            set
            {
                selectedArrivalLocationId = value;
                if (!string.IsNullOrEmpty(value))
                {
                    ArrivalLocation = ArrivalLocations.First(l => l.Id == int.Parse(value));
                    SelectedArrivalPortId = ArrivalLocation.Ports.First().Id.ToString();
                }
            }
        }

        [Required(AllowEmptyStrings = false)]
        public string SelectedArrivalPortId
        {
            get => selectedArrivalPortId;
            set
            {
                selectedArrivalPortId = value;
                if (!string.IsNullOrEmpty(value))
                {
                    ArrivalPort = ArrivalLocation.Ports.First(p => p.Id == int.Parse(value));
                }
            }
        }
        public float TripDistance => ArrivalLocation == null || DepartureLocation == null ? 0 :
            Math.Abs(ArrivalLocation.Distance - DepartureLocation.Distance);

        public string paymentType = "CSH"; // Form ?

        public string vendor = "VTS"; // Form ?

        public int rateCode = 4; // Calc or Service

        public LocationDetails DepartureLocation { get; set; } = new LocationDetails();
        public PortDetails DeparturePort { get; set; } = new PortDetails();

        public LocationDetails ArrivalLocation { get; set; } = new LocationDetails();
        public PortDetails ArrivalPort { get; set; } = new PortDetails();

        public LocationDetails[] DepartureLocations { get; set; }

        public LocationDetails[] ArrivalLocations { get; set; }

        public string FormattedMiles =>
              TripDistance > 1 ? $"{TripDistance}mil. Miles" :
                    $"{TripDistance * 1000}k. Miles";

        private string selectedDepartureLocationId;
        private string selectedDeparturePortId;
        private string selectedArrivalLocationId;
        private string selectedArrivalPortId;
    }
}
