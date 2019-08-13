using System;
using System.ComponentModel.DataAnnotations;

namespace BlazePort.Models.FormModels
{
    public class TripConfiguration
    {
        public DateTime TripStart { get; set; } = DateTime.Now;

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

        public string paymentType = "CSH"; // Form ?

        public string vendor = "VTS"; // Form ?

        public float tripDistance = 2.1F; // Calc or Service

        public int rateCode = 4; // Calc or Service
    }
}
