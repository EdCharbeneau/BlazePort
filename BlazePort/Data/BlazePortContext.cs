using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazePort.Data
{
    public class BlazePortContext : DbContext
    {
        public BlazePortContext(DbContextOptions<BlazePortContext> options)
                : base(options)
        { }

        public DbSet<LocationDetails> Locations { get; set; }

        public DbSet<PortDetails> PortDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<LocationDetails>().HasData(GenerateLocations());
            //modelBuilder.Entity<PortDetails>().HasData(GeneratePorts());
            modelBuilder.Entity<LocationDetails>()
                        .OwnsMany<PortDetails>(p => p.Ports);
            base.OnModelCreating(modelBuilder);

        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public async Task InitializeContainerAsync()
        {
            await Database.EnsureDeletedAsync();
            await Database.EnsureCreatedAsync();
            Locations.AddRange(GenerateLocations());
            //PortDetails.AddRange(GeneratePorts());
            var changed = await SaveChangesAsync();
            Console.WriteLine($"created {changed} records");
        }

        private List<LocationDetails> GenerateLocations()
        {
            var locations = new List<LocationDetails>();
            locations.Add(new LocationDetails
            {
                Id = Guid.NewGuid(),
                Name = "Earth",
                Description = "Earth is the third planet from the Sun, and the only astronomical object known to harbor life. According to radiometric dating and other sources of evidence, Earth formed over 4.5 billion years ago. Earth's gravity interacts with other objects in space, especially the Sun and the Moon, Earth's only natural satellite. Earth orbits around the Sun in 365.26 days, a period known as an Earth year. During this time, Earth rotates about its axis about 366.26 times.",
                Distance = 0,
                ImageUrl = "earth-600.jpg",
                Ports = GenerateEarthPorts()
            });

            locations.Add(new LocationDetails
            {
                Id = Guid.NewGuid(),
                Name = "Earth's Moon",
                Description = "The Moon is an astronomical body that orbits planet Earth and is Earth's only permanent natural satellite. It is the fifth-largest natural satellite in the Solar System, and the largest among planetary satellites relative to the size of the planet that it orbits (its primary). The Moon is after Jupiter's satellite Io the second-densest satellite in the Solar System among those whose densities are known.",
                Distance = 0.239F,
                ImageUrl = "moon-513.jpg",
                Ports = GenerateMoonPorts()
            });

            locations.Add(new LocationDetails
            {
                Id = Guid.NewGuid(),
                Name = "Mars",
                Description = "Mars is the fourth planet from the Sun and the second-smallest planet in the Solar System after Mercury. In English, Mars carries a name of the Roman god of war, and is often referred to as the \"Red Planet\" because the iron oxide prevalent on its surface gives it a reddish appearance that is distinctive among the astronomical bodies visible to the naked eye.[17] Mars is a terrestrial planet with a thin atmosphere, having surface features reminiscent both of the impact craters of the Moon and the valleys, deserts, and polar ice caps of Earth.",
                Distance = 140F,
                ImageUrl = "mars-540.jpg",
                Ports = GenerateMarsPorts()
            });

            return locations;
        }
        private static List<PortDetails> GenerateEarthPorts()
        {
            var ports = new List<PortDetails>();

            ports.Add(new PortDetails
            {
                Id = 4,
                Name = "Cape Canaveral Air Force Station",
                Description = "Located in Brevard County, Florida, the station is a primary site for America’s space program and missile testing. In July 1950, the first missile – a German V-2 rocket – was launched from here. It was also the launch site for the country’s first spaceflight, Freedom 7, that took place on May 5, 1961, and was piloted by astronaut Alan Shepard.",
                Country = "United States",
                Lat = 28.3922,
                Long = 80.6077,
                ImageUrl = ""

            });

            ports.Add(new PortDetails
            {
                Id = 5,
                Name = "Vandenberg Air Force Base",
                Description = "A prime location for testing intercontinental ballistic missiles and sending satellites into polar orbit, the military facility has conducted hundreds of launches. In 1959, it launched the world’s first polar orbiting satellite, the Discoverer 1, part of the Corona reconnaissance satellite program. Even though it failed to reach the orbit, it paved way for sending other satellites into space.",
                Country = "United States",
                Lat = 34.7420,
                Long = 120.5724,
                ImageUrl = ""

            });

            ports.Add(new PortDetails
            {
                Id = 6,
                Name = "Baikonur Cosmodrome",
                Description = "The construction of the oldest and largest space launch facility in the world began in 1955, and today it is jointly maintained by the Kazakh and Russian governments. In October 1957, Russia launched from Baikonur the first artificial satellite, Sputnik 1. The first human spaceflight, Vostok 1, also took place from the site. It is now mostly used to send Soyuz astronauts to the International Space Station.",
                Country = "Kazakhstan",
                Lat = 45.9646,
                Long = 63.3052,
                ImageUrl = ""

            });

            ports.Add(new PortDetails
            {
                Id = 7,
                Name = "Guiana Space Centre",
                Description = "Situated in Kourou, French Guiana, the site is primarily used by the French government and the European Space Agency. With a high success rate, the site has launched vehicles such as Ariane 5, Soyuz and Vega.",
                Country = "France",
                Lat = 5.1673,
                Long = 52.6832,
                ImageUrl = ""

            });

            ports.Add(new PortDetails
            {
                Id = 10,
                Name = "Satish Dhawan Space Centre",
                Description = "India’s spaceport is situated in Sriharikota, Andhra Pradesh, and was named after former chairman of Indian Space Research Organisation (ISRO) Satish Dhawan. It has most notably served as the launching pad for India's lunar orbiter Chandrayaan-1 in 2008 and Mars orbiter Mangalyaan in 2013. In 2017, the country launched a record 104 nano satellites into orbit onboard a single rocket.",
                Country = "India",
                Lat = 13.7331,
                Long = 80.2047,
                ImageUrl = ""

            });

            ports.Add(new PortDetails
            {
                Id = 11,
                Name = "Woomera",
                Description = "Situated approximately 310.6 miles (500 km) northwest of Adelaide, the complex is primarily used by the Royal Australian Air Force (RAAF). Covering an area of 47,177 square miles (122,188 square km), Woomera is the world’s largest land-based testing range.",
                Country = "Australia",
                Lat = 31.1656,
                Long = 136.8193,
                ImageUrl = ""

            });

            ports.Add(new PortDetails
            {
                Id = 12,
                Name = "Xichang Satellite Launch Center",
                Description = "Located in the Sichuan province, the center became operational in 1984. It serves as the main gateway to the geostationary orbit (a circular orbit above the equator and in the direction of Earth's rotation) and has been used to deploy Long March 3 and CZ-2E rockets. It has two main zones – Launch Complex 2 and Launch Complex 3. The Launch Complex 1 functions as a viewing area.",
                Country = "China",
                Lat = 28.2409,
                Long = 102.0226,
                ImageUrl = ""

            });

            return ports;
        }
        private static List<PortDetails> GenerateMoonPorts()
        {
            var ports = new List<PortDetails>();

            ports.Add(new PortDetails
            {
                Id = 13,
                Name = "Sea of Tranquility",
                Description = "Mare Tranquillitatis (Latin for Sea of Tranquillity or Sea of Tranquility; see spelling differences) is a lunar mare that sits within the Tranquillitatis basin on the Moon. The mare material within the basin consists of basalt formed in the intermediate to young age group of the Upper Imbrian epoch. The surrounding mountains are thought to be of the Lower Imbrian epoch, but the actual basin is probably Pre-Nectarian. The basin has irregular margins and lacks a defined multiple-ringed structure. The irregular topography in and near this basin results from the intersection of the Tranquillitatis, Nectaris, Crisium, Fecunditatis, and Serenitatis basins with two throughgoing rings of the Procellarum basin. Palus Somni, on the northeastern rim of the mare, is filled with the basalt that spilled over from Tranquillitatis.",
                Lat = 20.16,
                Long = 30.77,
                ImageUrl = ""

            });

            return ports;
        }
        private static List<PortDetails> GenerateMarsPorts()
        {
            var ports = new List<PortDetails>();

            ports.Add(new PortDetails
            {
                Id = 14,
                Name = "Mount Sharp",
                Description = "Mount Sharp, officially Aeolis Mons, is a mountain on Mars. It forms the central peak within Gale crater and is located around 5.08°S 137.85°E, rising 5.5 km (18,000 ft) high from the valley floor. It has the ID of 15,000 in the Gazetteer of Planetary Nomenclature from the US Geological Survey.",
                Lat = 4.5,
                Long = 137.4,
                ImageUrl = ""

            });

            return ports;
        }
    }
}