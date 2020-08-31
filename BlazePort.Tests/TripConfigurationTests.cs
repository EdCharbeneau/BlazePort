using BlazePort.Data;
using BlazePort.Pages.Home;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace BlazePort.Tests
{
    public class TripConfigurationModelTests
    {
        // Some known GUIDs
        private readonly Guid GUID1 = new Guid("6fbb4a14-e707-11ea-adc1-0242ac120002");
        private readonly Guid GUID2 = new Guid("6fbb4c12-e707-11ea-adc1-0242ac120002");

        [Fact(DisplayName = "When DepartureLocations is set, default Id values are populated.")]
        public void Set_DepartureLocations_Default_Ids_Populated()
        {
            var locations = BlazePortContext.GenerateLocations().ToArray();
            locations[0].Id = GUID1;
            locations[1].Id = GUID2;

            var uut = new TripConfigurationModel
            {
                DepartureLocations = locations
            };

            uut.DepartureLocation.Id.Should().Be(GUID1);
            uut.DeparturePort.Id.Should().Be(4);
            uut.ArrivalLocation.Id.Should().Be(GUID2);
            uut.ArrivalPort.Id.Should().Be(13);
        }

        [Fact(DisplayName = "When SelectedDepartureLocationId is set, default DeparturePort.Id is selected.")]
        public void Setting_DepartureLocationId_Sets_PortId()
        {
            // arrange
            var locations = BlazePortContext.GenerateLocations().ToArray();
            locations[1].Id = GUID2;

            // act
            var uut = new TripConfigurationModel
            {
                DepartureLocations = locations
            };
            uut.SelectedDepartureLocationId = GUID2;

            // assert
            uut.DeparturePort.Id.Should().Be(13);
        }

        [Fact(DisplayName = "When SelectedDepartureLocationId is set, default ArrivalPort.Id is selected.")]
        public void Setting_SelectedArrivalLocationId_Sets_PortId()
        {
            // arrange
            var locations = BlazePortContext.GenerateLocations().ToArray();
            locations[1].Id = GUID2;

            // act
            var uut = new TripConfigurationModel
            {
                DepartureLocations = locations
            };
            uut.SelectedArrivalLocationId = GUID2;

            // assert
            uut.ArrivalPort.Id.Should().Be(13);
        }

        [Fact(DisplayName = "When SelectedDepartureLocationId is set, the id is excluded from ArrivalLocations")]
        public void Setting_DepartureLocationId_Excludes_Duplicate_From_ArrivalLocations()
        {
            // arrange
            var locations = BlazePortContext.GenerateLocations().ToArray();
            locations[0].Id = GUID1;

            // act
            var uut = new TripConfigurationModel
            {
                DepartureLocations = locations
            };
            uut.SelectedDepartureLocationId = GUID1;

            // assert
            uut.ArrivalLocations.Select(al => al.Id).Should().NotContain(GUID1);
        }

        [Fact(DisplayName = "When SelectedDepartureLocationId is reset, the default DeparturePort.Name is selected.")]
        public void Resetting_SelectedDepartureLocationId_After_Setting_SelectedDeparturePortId_Sets_Default_Id()
        {
            // arrange
            var locations = BlazePortContext.GenerateLocations().ToArray();
            locations[0].Id = GUID1;
            locations[1].Id = GUID2;

            // act
            var uut = new TripConfigurationModel
            {
                DepartureLocations = locations
            };
            uut.SelectedDepartureLocationId = GUID2;
            uut.SelectedDeparturePortId = uut.DepartureLocations.First(l => l.Id == GUID2).Ports[0].Name;

            uut.SelectedDepartureLocationId = GUID1;

            // assert
            uut.DeparturePort.Name.Should().Be("Cape Canaveral Air Force Station");
        }

    }
}
