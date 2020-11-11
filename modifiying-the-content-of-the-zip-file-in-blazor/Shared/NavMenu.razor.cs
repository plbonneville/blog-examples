using BlazorZipExplorer.Store.ZipFileUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorZipExplorer.Shared
{
    public partial class NavMenu
    {
        [Inject]
        private IState<ZipFileState> ZipFileState { get; set; }

        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
