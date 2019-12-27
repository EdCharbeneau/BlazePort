using BlazePort.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BlazePort.Pages.Admin
{
    public class PortDetailsForm : IFormBehaviors
    {
        public PortDetailsForm(FormMode formMode) => FormMode = formMode;
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Country { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string SelectedLocation { get; set; }

        public FormMode FormMode { get; set; }

        public PortDetails ToPortDetails() => new PortDetails()
        {
            Id = Id.GetValueOrDefault(0),
            Name = Name,
            LocationId = int.Parse(SelectedLocation),
            Country = Country,
            Description = Description,
            ImageUrl = ImageUrl,
            Lat = Lat,
            Long = Long
        };

        public PortDetails ApplyFormToEntity(PortDetails editItem)
        {
            editItem.Name = Name;
            editItem.LocationId = int.Parse(SelectedLocation);
            editItem.Country = Country;
            editItem.Description = Description;
            editItem.ImageUrl = ImageUrl;
            editItem.Lat = Lat;
            editItem.Long = Long;
            return editItem;
        }
        public static PortDetailsForm FromPortDetails(PortDetails port, FormMode formMode) => new PortDetailsForm(formMode)
        {
            Id = port.Id,
            Name = port.Name,
            SelectedLocation = port.LocationId.ToString(),
            Country = port.Country,
            Description = port.Description,
            ImageUrl = port.ImageUrl,
            Lat = port.Lat,
            Long = port.Long
        };
    }
}
