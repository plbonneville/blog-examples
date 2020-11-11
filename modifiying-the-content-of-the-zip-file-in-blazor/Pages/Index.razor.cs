using System.Threading.Tasks;
using BlazorZipExplorer.Store.ZipFileUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorZipExplorer.Pages
{
    public partial class Index
    {
        private const string DefaultStatus = "Choose a zip file";
        private string _status = DefaultStatus;

        [Inject]
        private IDispatcher Dispatcher { get; set; }

        private async Task OnInputFileChange(InputFileChangeEventArgs e)
        {
            await using var stream = e.File.OpenReadStream();

            Dispatcher.Dispatch(new UploadZipFileAction { File = stream });

            _status = DefaultStatus;
        }
    }
}
