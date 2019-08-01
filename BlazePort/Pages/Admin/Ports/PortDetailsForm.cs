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
    }
}
