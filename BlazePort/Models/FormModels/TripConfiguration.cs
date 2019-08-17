using BlazePort.Data;
using System;
using System.ComponentModel.DataAnnotations;

namespace BlazePort.Models.FormModels
{
    public class TripConfiguration
    {
        public DateTime TripStart { get; set; } = DateTime.Now.AddDays(1);

        [Required]
        [Range(1,4)]
        public int PassengerCount { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SelectedDepartureId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SelectedPortOfTravelId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SelectedDestinationId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SelectedPortOfEntryId { get; set; }
        public float TripDistance => ArrivalLocation == null || DepartureLocation == null ? 0 : Math.Abs(ArrivalLocation.Distance - DepartureLocation.Distance);

        public string paymentType = "CSH"; // Form ?

        public string vendor = "VTS"; // Form ?
        
        public int rateCode = 4; // Calc or Service

        public LocationDetails DepartureLocation { get; set; }

        public LocationDetails ArrivalLocation { get; set; }
    }
}
