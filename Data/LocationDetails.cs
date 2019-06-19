using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazePort.Data
{
    public class LocationDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

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
