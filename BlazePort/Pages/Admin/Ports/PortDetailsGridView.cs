using BlazePort.Data;
using System;
using System.Linq;

namespace BlazePort.Pages.Admin
{
    public class PortDetailsGridView
    {
        public PortDetailsGridView() { }

        public static PortDetailsGridView FromPort(PortDetails port) =>
            new PortDetailsGridView
            {
                Id = port.Id,
                Name = port.Name,
                Description = port.Description,
                Lat = port.Lat,
                Long = port.Long,
                Country = port.Country,
                ImageUrl = port.ImageUrl,
                LocationName = port.Location.Name,
                LocationId = port.Location.Id
            };

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription => String.Join("", Description.Take(160)) + "...";
        public string ImageUrl { get; set; }
        public string Country { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }

        public string LatLong => $"{Lat}, {Long}";

        public string LocationName { get; set; }
        public int LocationId { get; set; }
    }
}
