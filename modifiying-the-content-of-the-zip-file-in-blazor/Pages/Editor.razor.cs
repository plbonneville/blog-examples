using System.Threading.Tasks;
using BlazorMonaco;
using BlazorMonaco.Bridge;
using BlazorZipExplorer.Store.ZipFileUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BlazorZipExplorer.Pages
{
    public partial class Editor
    {
        private bool _hasLocationChanged = false;

        [Parameter]
        public string Filename { get; set; }

        [Inject]
        private IState<ZipFileState> ZipFileState { get; set; }

        [Inject]
        private IDispatcher Dispatcher { get; set; }

        private string CsCode { get; set; } = "";

        private MonacoEditor MonacoEditor { get; set; }

        private StandaloneEditorConstructionOptions EditorConstructionOptions(MonacoEditor editor)
        {
            return new StandaloneEditorConstructionOptions
            {
                Language = "csharp",
                GlyphMargin = true
            };
        }        

        public async Task LoadFile()
        {
            var file = ZipFileState.Value.ZipEntries[Filename];

            CsCode = file.Content;

            await (MonacoEditor?.SetValue(file.Content) ?? Task.Run(() => { }));
        }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                _hasLocationChanged = true;
                await LoadFile();
            }
            finally
            {
                _hasLocationChanged = false;
            }

            await base.OnParametersSetAsync();
        }

        protected override void Dispose(bool disposing)
        {
            MonacoEditor?.DisposeEditor();
            MonacoEditor?.Dispose();

            base.Dispose(disposing);
        }

        private async Task EditorOnDidChangeModelContent()
        {
            if (_hasLocationChanged)
            {
                return;
            }

            await OnChange();
        }

        private async Task OnChange()
        {
            var value = await MonacoEditor.GetValue();

            Dispatcher.Dispatch(new UpdateZipFileAction { ZipEntry = new ZipEntry { Name = Filename, Content = value } });
        }

        private async Task EditorOnDidInit(MonacoEditorBase editor)
        {
            await LoadFile();
        }
    }
}
