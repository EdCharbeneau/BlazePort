using BlazePort.Data;
using System.ComponentModel.DataAnnotations;

namespace BlazePort.Pages.Admin
{
    public class PortDetailsForm
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Country { get; set; }
        public string ImageUrl { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string SelectedLocation { get; set; }

        public PortDetails ToPortDetails() => new PortDetails()
        {
            Name = Name,
            LocationId = int.Parse(SelectedLocation),
            Country = Country,
            Description = Description,
            ImageUrl = ImageUrl
        };
    }
}
