using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazePort.Models.FormModels
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
