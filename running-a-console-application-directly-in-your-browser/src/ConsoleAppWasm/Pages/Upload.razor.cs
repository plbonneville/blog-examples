using System.Threading.Tasks;
using ConsoleAppWasm.Store.ZipFileUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ConsoleAppWasm.Pages
{
    public partial class Upload
    {
        private const string DefaultStatus = "Choose a zip file";

        [Inject]
        private IDispatcher Dispatcher { get; set; }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            await using var stream = e.File.OpenReadStream();

            Dispatcher.Dispatch(new UploadZipFileAction { File = stream });
        }
    }
}
