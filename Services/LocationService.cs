using BlazePort.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazePort.Services
{
    public class LocationService
    {
        public IList<LocationDetails> GetLocations()
        {
            return new List<LocationDetails>()
            {
                new LocationDetails
                {
                    Id = 0,
                    Name = "Earth",
                    Description = "Earth is the third planet from the Sun and the only astronomical object known to harbor life. According to radiometric dating and other sources of evidence, Earth formed over 4.5 billion years ago. Earth's gravity interacts with other objects in space, especially the Sun and the Moon, Earth's only natural satellite."
                },
                new LocationDetails
                {
                    Id = 1,
                    Name = "Moon",
                    Description = "The Moon is an astronomical body that orbits planet Earth and is Earth's only permanent natural satellite. It is the fifth-largest natural satellite in the Solar System, and the largest among planetary satellites relative to the size of the planet that it orbits."
                },
                new LocationDetails
                {
                    Id = 2,
                    Name = "Mars",
                    Description = "Mars is the fourth planet from the Sun and the second-smallest planet in the Solar System after Mercury."
                }
            };
        }
    }
}
