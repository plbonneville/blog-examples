using System.Threading.Tasks;
using ConsoleAppWasm.Store.CSharpCompilationUseCase;
using ConsoleAppWasm.Store.ZipFileUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace ConsoleAppWasm.Pages
{
    public partial class Index
    {
        [Inject]
        private IDispatcher Dispatcher { get; set; }

        protected override Task OnInitializedAsync()
        {
            Dispatcher.Dispatch(new LoadAssembliesAction());

            return base.OnInitializedAsync();
        }

        private void Click()
        {
            Dispatcher.Dispatch(new CreateZipFileAction());
        }
    }
}
