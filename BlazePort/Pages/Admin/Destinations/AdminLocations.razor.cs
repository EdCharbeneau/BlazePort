using BlazePort.Components;
using BlazePort.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Blazor.Components;

namespace BlazePort.Pages.Admin.Destinations
{
    public partial class AdminLocations : IFormBehaviors
    {
        bool editorPanelVisible;
        [Inject] BlazePortContext Db { get; set; }
        Notification SuccessNotification { get; set; }
        Notification FailNotification { get; set; }
        public FormMode FormMode { get; set; }

        LocationDetails[] locations;

        LocationDetails editItem = new LocationDetails();

        IEnumerable<LocationDetails> selectedItems = Enumerable.Empty<LocationDetails>();
  
        protected override async Task OnInitializedAsync()
        {
            await LoadGrid();
        }

        async Task LoadGrid()
        {
            locations = await Db.Locations.ToArrayAsync();
        }

        async Task SaveLocation()
        {
            editorPanelVisible = false;

            await TrySaving(
                OnSuccess: SuccessFullySaved,
                OnFail: FailNotification.Show
                );
        }

        async Task SuccessFullySaved()
        {
            ClearSelections();
            await LoadGrid();
            StateHasChanged();
            await SuccessNotification.Show();
        }

        private async Task TrySaving(Func<Task> OnSuccess, Func<Task> OnFail)
        {
            try
            {
                if (FormMode == FormMode.New)
                {
                    Db.Locations.Add(editItem);
                }

                await Db.SaveChangesAsync();
                await OnSuccess();
            }
            catch (Exception e)
            {
                // TODO: Logging
                await OnFail();
            }
        }

        void HandleCreate(GridCommandEventArgs _)
        {
            editItem = new LocationDetails();
            FormMode = FormMode.New;
            editorPanelVisible = true;
        }

        async Task HandleSelected(GridCommandEventArgs e)
        {
            editItem = await Db.Locations.FindAsync(selectedItems.First().Id);
            FormMode = FormMode.Edit;
            editorPanelVisible = true;
        }

        void ClearSelections() => selectedItems = Enumerable.Empty<LocationDetails>();

        void HandleCancel() => editorPanelVisible = false;


    }
}
