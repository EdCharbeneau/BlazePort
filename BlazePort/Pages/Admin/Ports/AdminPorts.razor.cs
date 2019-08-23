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
            locations = await Db.Locations.ToArrayAsync();

            portsGridView = locations.SelectMany(p => p.Ports).Select(p => PortDetailsGridView.FromPort(p)).ToArray();
        }

        protected async Task CloseEditor()
        {
            await EditorPanel.HideAsync();
        }

        protected async Task OpenEditor()
        {
            await EditorPanel.ShowAsync();
        }

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

            locations = await Db.Locations.ToArrayAsync();

            portsGridView = locations.SelectMany(p => p.Ports).Select(p => PortDetailsGridView.FromPort(p)).ToArray();

            portForm = new PortDetailsForm();

            await EditorPanel.HideAsync();

            await SuccessNotification.Show();
        }
    }
}
