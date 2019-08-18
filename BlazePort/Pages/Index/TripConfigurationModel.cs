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
                DepartureLocation = DepartureLocations.First(l => l.Id == int.Parse(value));
            }
        }

        [Required(AllowEmptyStrings = false)]
        public string SelectedDeparturePortId
        {
            get => selectedDeparturePortId;
            set
            {
                selectedDeparturePortId = value;
                DeparturePort = DeparturePorts.First(p => p.Id == int.Parse(value));
            }
        }

        [Required(AllowEmptyStrings = false)]
        public string SelectedArrivalLocationId
        {
            get => selectedArrivalLocationId;
            set
            {
                selectedArrivalLocationId = value;
                ArrivalLocation = ArrivalLocations.First(l => l.Id == int.Parse(value));
            }
        }

        [Required(AllowEmptyStrings = false)]
        public string SelectedArrivalPortId
        {
            get => selectedArrivalPortId;
            set
            {
                selectedArrivalPortId = value;
                ArrivalPort = ArrivalPorts.First(p => p.Id == int.Parse(value));
            }
        }
        public float TripDistance => ArrivalLocation == null || DepartureLocation == null ? 0 :
            Math.Abs(ArrivalLocation.Distance - DepartureLocation.Distance);

        public string paymentType = "CSH"; // Form ?

        public string vendor = "VTS"; // Form ?

        public int rateCode = 4; // Calc or Service

        public LocationDetails DepartureLocation { get; private set; }
        public PortDetails DeparturePort { get; private set; }

        public LocationDetails ArrivalLocation { get; private set; }
        public PortDetails ArrivalPort { get; private set; }

        public LocationDetails[] DepartureLocations { get; set; }

        public LocationDetails[] ArrivalLocations { get; set; }

        public PortDetails[] DeparturePorts { get; set; }

        public PortDetails[] ArrivalPorts { get; set; }

        public string ArrivalTo => $"{ArrivalLocation?.Name} - {ArrivalPort?.Name}";
        public string DepartureFrom => $"{DepartureLocation?.Name} - {DeparturePort?.Name}";

        public string FormattedTripTimeFromDistance
        {
            get
            {
                var time = TimeSpan.FromHours(TripDistance / .2F);
                return time.Days > 0 ?
                    $"{time.Days} Days {time.Hours} Hours" :
                    $"{time.Hours} Hours {time.Minutes} Minutes";
            }
        }

        public string FormattedMiles  =>
              TripDistance > 1 ? $"{TripDistance}mil. Miles" :
                    $"{TripDistance * 1000}k. Miles";

        private string selectedDepartureLocationId;
        private string selectedDeparturePortId;
        private string selectedArrivalLocationId;
        private string selectedArrivalPortId;
    }
}
