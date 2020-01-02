using BlazePort.Components;
using BlazePort.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Blazor.Components;

namespace BlazePort.Pages.Admin.Ports
{
    public partial class AdminPorts : IFormBehaviors
    {
        [Inject] BlazePortContext Db { get; set; }

        bool editorPanelVisible;
        Notification SuccessNotification { get; set; }
        Notification FailNotification { get; set; }

        PortDetailsGridView[] portsGridView;
        LocationDetails[] locations;

        IEnumerable<PortDetailsGridView> selectedItems = Enumerable.Empty<PortDetailsGridView>();

        LocationDetails locationEditItem;
        PortDetails editItem;

        string FormTitle => FormMode == FormMode.Edit ?
            $"{editItem.Name} (editing)" : "New Port";

        public FormMode FormMode { get; set; }

        protected override async Task OnInitializedAsync()
        {
            locations = await Db.Locations.ToArrayAsync();
            LoadGrid();
        }

        private void LoadGrid()
        {

            portsGridView = locations
                .SelectMany(p => p.Ports)
                .Select(p => PortDetailsGridView.FromPort(p))
                .ToArray();
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
            LoadGrid();
            StateHasChanged();
            await SuccessNotification.Show();
        }

        private async Task TrySaving(Func<Task> OnSuccess, Func<Task> OnFail)
        {
            try
            {
                if (FormMode == FormMode.New)
                {
                    Db.PortDetails.Add(editItem);
                }

                await Db.SaveChangesAsync();
                await OnSuccess();
            }
            catch (Exception e)
            {
                // TODO: Logging
                Console.WriteLine(e);
                await OnFail();
            }
        }

        void HandleCreate(GridCommandEventArgs _)
        {
            editItem = new PortDetails();
            FormMode = FormMode.New;
            editorPanelVisible = true;

        }

        async Task HandleSelected(GridCommandEventArgs e)
        {
            /// Find the location to edit
            locationEditItem = await Db.Locations.FindAsync(selectedItems.First().LocationId);
            /// Get Port node
            editItem = locationEditItem.Ports.Find(p => p.Id == selectedItems.First().Id);
            FormMode = FormMode.Edit;
            editorPanelVisible = true;
        }

        void ClearSelections() => selectedItems = Enumerable.Empty<PortDetailsGridView>();

        void HandleCancel() => editorPanelVisible = false;
    }
}
