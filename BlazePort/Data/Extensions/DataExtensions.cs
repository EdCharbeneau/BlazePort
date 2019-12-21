using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BlazePort.Data
{
    public static class DataExtensions
    {
        public static IIncludableQueryable<LocationDetails, List<PortDetails>> IncludePorts(this IQueryable<LocationDetails> locations)
            => locations.Include(l => l.Ports);

        public static IQueryable<LocationDetails> AllLocationDetails(this BlazePortContext blazePortContext)
            => blazePortContext.Locations.IncludePorts();
    }
}
