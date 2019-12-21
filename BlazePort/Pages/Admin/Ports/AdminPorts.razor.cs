using BlazePort.Components;
using BlazePort.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BlazePort.Pages.Admin.Ports
{
    public partial class AdminPorts
    {
        [Inject] BlazePortContext Db { get; set; }
        FlyoutPanel EditorPanel { get; set; }
        Notification SuccessNotification { get; set; }
        Notification FailNotification { get; set; }

        PortDetailsGridView[] portsGridView;

        PortDetailsForm portForm = new PortDetailsForm();

        LocationDetails[] locations;

        protected override async Task OnInitializedAsync()
        {
            //locations = await Db.AllLocationDetails().ToArrayAsync();

            //portsGridView = locations.SelectMany(p => p.Ports).Select(p => PortDetailsGridView.FromPort(p)).ToArray();
            locations = await Db.Locations.ToArrayAsync();
            portsGridView = Db.PortDetails.Select(p => PortDetailsGridView.FromPort(p)).ToArray();
        }

        async Task CloseEditor()
        {
            await EditorPanel.HideAsync();
        }

        async Task OpenEditor()
        {
            await EditorPanel.ShowAsync();
        }

        async Task SaveLocation()
        {
            Db.PortDetails.Add(portForm.ToPortDetails());

            await Db.SaveChangesAsync();

            locations = await Db.Locations.ToArrayAsync();

            portsGridView = locations
                .SelectMany(p => p.Ports)
                //.Where(AreValidRecords)
                .Select(p => PortDetailsGridView.FromPort(p))
                .ToArray();

            portForm = new PortDetailsForm();

            await EditorPanel.HideAsync();

            await SuccessNotification.Show();
        }

        // Cosmos or EF reports a false record with id of negative value
        //private Func<PortDetails, bool> AreValidRecords = (PortDetails port) => port.Id > 0;
    }
}
