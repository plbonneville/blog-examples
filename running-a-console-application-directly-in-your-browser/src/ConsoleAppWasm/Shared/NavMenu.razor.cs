using ConsoleAppWasm.Store.ZipFileUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace ConsoleAppWasm.Shared
{
    public partial class NavMenu
    {
        [Inject]
        private IState<ZipFileState> ZipFileState { get; set; }

        private bool _collapseNavMenu = true;

        private string NavMenuCssClass => _collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            _collapseNavMenu = !_collapseNavMenu;
        }
    }
}
