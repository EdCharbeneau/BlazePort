using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazePort.Extensions
{
    public static class Sugar
    {
        public static TResult Map<TSource, TResult>(this TSource @this, Func<TSource, TResult> fn) =>
            fn(@this);
    }
}
