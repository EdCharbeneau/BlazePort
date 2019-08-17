using BlazePort.Components;
using BlazePort.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BlazePort.Pages.Admin.Ports
{
    public class AdminPortsBase : ComponentBase
    {
        [Inject] BlazePortContext Db { get; set; }
        protected FlyoutPanel EditorPanel { get; set; }
        protected Notification SuccessNotification { get; set; }
        protected Notification FailNotification { get; set; }

        protected PortDetailsGridView[] portsGridView;

        protected PortDetailsForm portForm = new PortDetailsForm();

        protected LocationDetails[] locations;

        protected override async Task OnInitializedAsync()
        {
            portsGridView = await LoadPortsViewModel();
            locations = await Db.Locations.ToArrayAsync();
        }

        protected async Task CloseEditor()
        {
            await EditorPanel.HideAsync();
        }

        protected async Task OpenEditor()
        {
            await EditorPanel.ShowAsync();
        }

        protected async Task<PortDetailsGridView[]> LoadPortsViewModel() =>
                await Db.PortDetails.Include(p => p.Location)
                    .Select(p => PortDetailsGridView.FromPort(p))
                    .ToArrayAsync();

        protected async Task SaveLocation()
        {
            Db.PortDetails.Add(portForm.ToPortDetails());

            try
            {
                await Db.SaveChangesAsync();
            }
            catch (System.Exception)
            {
                await FailNotification.Show();
            }

            portsGridView = await LoadPortsViewModel();

            portForm = new PortDetailsForm();

            await EditorPanel.HideAsync();

            await SuccessNotification.Show();
        }
    }
}
