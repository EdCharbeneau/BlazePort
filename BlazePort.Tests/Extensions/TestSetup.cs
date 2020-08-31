using BlazePort.Pages.Home;
using BlazePort.TripCost.Service;
using BlazorSize;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace BlazePort.Tests
{
    public static class TestSetup
    {
        public static void RegisterServices(SnapshotTest test)
        {
            string modelPath = Path.Combine(Environment.CurrentDirectory, "Data", "TripCostModel.zip");
            test.Services.AddSingleton<ITripCostPredictionService>(new TripCostPredictionService(modelPath));
            test.Services.AddScoped<TripConfigurationState>();
            test.Services.AddScoped<ResizeListener>();
            test.Services.AddTelerikBlazor();
        }
    }
}
