using System.Threading.Tasks;
using BlazorMonaco;
using BlazorMonaco.Bridge;
using ConsoleAppWasm.Store.ZipFileUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace ConsoleAppWasm.Pages
{
    public partial class Editor
    {
        private bool _hasLocationChanged;

        [Parameter]
        public string Filename { get; set; }

        [Inject]
        private IState<ZipFileState> ZipFileState { get; set; }

        [Inject]
        private IDispatcher Dispatcher { get; set; }

        private MonacoEditor MonacoEditor { get; set; }

        private static StandaloneEditorConstructionOptions EditorConstructionOptions(MonacoEditor editor)
        {
            return new StandaloneEditorConstructionOptions
            {
                Language = "csharp",
                GlyphMargin = true
            };
        }        

        private async Task LoadFile()
        {
            var file = ZipFileState.Value.ZipEntries[Filename];

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

            Dispatcher.Dispatch(new UpdateZipFileAction(new ZipEntry { Name = Filename, Content = value }));
        }

        private async Task EditorOnDidInit(MonacoEditorBase editor)
        {
            await LoadFile();
        }
    }
}
