using BlazePort.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BlazePort.Pages.Home
{
    public class TripConfigurationModel
    {
        public DateTime TripStart { get; set; } = DateTime.Now.AddDays(1);

        [Required]
        [Range(1, 4)]
        public int PassengerCount { get; set; } = 1;

        private Guid selectedDepartureLocationId;
        [Required(AllowEmptyStrings = false)]
        public Guid SelectedDepartureLocationId
        {
            get => selectedDepartureLocationId;
            set
            {
                selectedDepartureLocationId = value;
                if (value != Guid.Empty)
                {
                    DepartureLocation = DepartureLocations.First(l => l.Id == value);
                    SelectedDeparturePortId = DepartureLocation.Ports.First().Name;
                    ClearArrivals(value);
                }
            }
        }

        private void ClearArrivals(Guid selectedValue)
        {
            ArrivalLocations = DepartureLocations.Where(loc => loc.Id != selectedValue).ToArray();
            SelectedArrivalLocationId = ArrivalLocations.First().Id;
        }

        [Required(AllowEmptyStrings = false)]
        public string SelectedDeparturePortId
        {
            get => DeparturePort.Name;
            set => DeparturePort = DepartureLocation.Ports.First(p => p.Name == value);
        }

        private Guid selectedArrivalLocationId;
        [Required(AllowEmptyStrings = false)]
        public Guid SelectedArrivalLocationId
        {
            get => selectedArrivalLocationId;
            set
            {
                selectedArrivalLocationId = value;
                if (value != Guid.Empty)
                {
                    ArrivalLocation = ArrivalLocations.First(l => l.Id == value);
                    SelectedArrivalPortId = ArrivalLocation.Ports.First().Name;
                }
            }
        }

        [Required(AllowEmptyStrings = false)]
        public string SelectedArrivalPortId
        {
            get => ArrivalPort.Name;
            set => ArrivalPort = ArrivalLocation.Ports.First(p => p.Name == value);
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

        private LocationDetails[] departureLocations;
        public LocationDetails[] DepartureLocations
        {
            get => departureLocations;
            set
            {
                departureLocations = value;
                SelectedDepartureLocationId = departureLocations[0].Id;
            }
        }

        public LocationDetails[] ArrivalLocations { get; set; }

        public string FormattedMiles =>
              TripDistance > 1 ? $"{TripDistance}mil. Miles" :
                    $"{TripDistance * 1000}k. Miles";


    }
}
