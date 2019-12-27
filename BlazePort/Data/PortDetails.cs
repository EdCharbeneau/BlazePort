using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlazePort.Data
{
    public class PortDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Country { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }

        [Required]
        public int LocationId { get; set; }

        public LocationDetails Location { get; set; }

        public void Deconstruct(out int id, out string name)
        {
            id = Id;
            name = Name;
        }
        public void Deconstruct(out int id, out string name, out string description)
        {
            id = Id;
            name = Name;
            description = Description;
        }

    }
}
