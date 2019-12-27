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
    public partial class AdminPorts
    {
        [Inject] BlazePortContext Db { get; set; }
        FlyoutPanel EditorPanel { get; set; }
        Notification SuccessNotification { get; set; }
        Notification FailNotification { get; set; }

        PortDetailsGridView[] portsGridView;
        LocationDetails[] locations;

        IEnumerable<PortDetailsGridView> selectedItems = Enumerable.Empty<PortDetailsGridView>();

        PortDetailsForm portForm;

        PortDetails editItem;

        string FormTitle => portForm.FormMode == FormMode.Edit ?
            $"{editItem.Name} (editing)" : "New Port";

        protected override async Task OnInitializedAsync()
        {
            locations = await Db.Locations.ToArrayAsync();
            await LoadGrid();
        }

        private async Task LoadGrid()
        {
            portsGridView = await Db.PortDetails.Select(p => PortDetailsGridView.FromPort(p)).ToArrayAsync();
        }

        async Task SaveLocation()
        {
            await EditorPanel.HideAsync();

            await TrySaving(
                OnSuccess: SuccessNotification.Show,
                OnFail: FailNotification.Show
                );

            await LoadGrid();
            ClearSelections();
        }

        private async Task TrySaving(Func<Task> OnSuccess, Func<Task> OnFail)
        {
            try
            {
                if (portForm.FormMode == FormMode.New)
                {
                    Db.PortDetails.Add(portForm.ToPortDetails());
                }
                else
                {
                    editItem = portForm.ApplyFormToEntity(editItem);
                }

                await Db.SaveChangesAsync();
                await OnSuccess();
            }
            catch (System.Exception)
            {
                // TODO: Logging
                await OnFail();
            }
        }

        async Task HandleCreate(GridCommandEventArgs _)
        {
            portForm = new PortDetailsForm(FormMode.New);
            await EditorPanel.ShowAsync();
        }

        async Task HandleSelected(GridCommandEventArgs e)
        {
            editItem = await Db.PortDetails.FindAsync(selectedItems.First().Id);
            portForm = PortDetailsForm.FromPortDetails(
               port: editItem,
               formMode: FormMode.Edit);
            await EditorPanel.ShowAsync();
        }

        void ClearSelections() => selectedItems = Enumerable.Empty<PortDetailsGridView>();

        async Task HandleCancel() => await EditorPanel.HideAsync();
        // Cosmos or EF reports a false record with id of negative value
        //private Func<PortDetails, bool> AreValidRecords = (PortDetails port) => port.Id > 0;
    }
}
