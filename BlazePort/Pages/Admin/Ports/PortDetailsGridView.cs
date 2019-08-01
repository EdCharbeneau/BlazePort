using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazePort.Pages.Admin
{
    public class PortDetailsGridView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescriptioin => String.Join("",Description.Take(160)) + "...";
        public string ImageUrl { get; set; }
        public string Country { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }

        public string LatLong => $"{Lat}, {Long}";

        public string LocationName { get; set; }
    }
}
