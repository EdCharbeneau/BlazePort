using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazePort.Data
{
    public partial class PortDetails
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Country { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public Guid LocationId { get; set; }

        public LocationDetails Location { get; set; }

        [NotMapped] // Form Field Only
        [Required(AllowEmptyStrings = false)]
        public string SelectedLocation {
            get => LocationId == null ? string.Empty : LocationId.ToString();
            set => LocationId = string.IsNullOrWhiteSpace(value) ? Guid.Empty : Guid.Parse(value);
        }

    }
}
