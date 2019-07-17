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

    }
}
