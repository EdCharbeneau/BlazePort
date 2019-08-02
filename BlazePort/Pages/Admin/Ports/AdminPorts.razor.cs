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

        protected bool EditorVisible { get; set; }

        protected string PanelCss { get; set; }

        protected PortDetailsGridView[] portsGridView;

        protected PortDetailsForm portForm = new PortDetailsForm();

        protected LocationDetails[] locations;

        protected override async Task OnInitAsync()
        {
            portsGridView = await LoadPortsViewModel();
            locations = await Db.Locations.ToArrayAsync();
        }

        protected async Task CloseEditor()
        {
            PanelCss = "fly-out-panel-close";
            await Task.Delay(700);
            EditorVisible = false;
        }

        protected void OpenEditor()
        {
            PanelCss = "fly-out-panel";
            EditorVisible = true;
        }

        protected async Task<PortDetailsGridView[]> LoadPortsViewModel() =>
                await Db.PortDetails.Include(p => p.Location)
                    .Select(p => PortDetailsGridView.FromPort(p))
                    .ToArrayAsync();

        protected async Task SaveLocation()
        {
            Db.PortDetails.Add(portForm.ToPortDetails());

            await Db.SaveChangesAsync();

            portsGridView = await LoadPortsViewModel();

            portForm = new PortDetailsForm();
        }
    }
}
