using BlazePort.Components;
using BlazePort.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Blazor.Components;

namespace BlazePort.Pages.Admin.Destinations
{
    public partial class AdminLocations
    {
        [Inject] BlazePortContext Db { get; set; }
        FlyoutPanel EditorPanel { get; set; }
        Notification SuccessNotification { get; set; }
        Notification FailNotification { get; set; }

        LocationDetails[] locations;

        LocationDetails newLocation = new LocationDetails();

        IEnumerable<LocationDetails> selectedItems = Enumerable.Empty<LocationDetails>();

        protected override async Task OnInitializedAsync()
        {
            await LoadLocations();
        }

        async Task LoadLocations()
        {
            locations = await Db.Locations.ToArrayAsync();
        }

        async Task SaveLocation()
        {
            Db.Locations.Add(newLocation);

            await Db.SaveChangesAsync();

            newLocation = new LocationDetails();

            await LoadLocations();

            await CloseEditor();
        }

        async Task HandleCreate(GridCommandEventArgs _)
        {
            //portForm = new PortDetailsForm(FormMode.New);
            await EditorPanel.ShowAsync();
        }

        async Task HandleSelected(GridCommandEventArgs e)
        {
            //editItem = await Db.PortDetails.FindAsync(selectedItems.First().Id);
            //portForm = PortDetailsForm.FromPortDetails(
            //   port: editItem,
            //   formMode: FormMode.Edit);
            await EditorPanel.ShowAsync();
        }

        async Task CloseEditor()
        {
            await EditorPanel.HideAsync();
        }

        async Task OpenEditor()
        {
            await EditorPanel.ShowAsync();
        }
        async Task HandleCancel() => await EditorPanel.HideAsync();

    }
}
