using System.Threading.Tasks;
using BlazorMonaco;
using BlazorMonaco.Bridge;
using BlazorZipExplorer.Store.ZipFileUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorZipExplorer.Pages
{
    public partial class Editor
    {
        [Inject]
        private IState<ZipFileState> ZipFileState { get; set; }

        [Inject]
        private IDispatcher Dispatcher { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Parameter]
        public string Filename { get; set; }

        private string CsCode { get; set; } = "";

        private MonacoEditor _editor { get; set; }

        private StandaloneEditorConstructionOptions EditorConstructionOptions(MonacoEditor editor)
        {
            return new StandaloneEditorConstructionOptions
            {
                Language = "csharp",
                GlyphMargin = true
            };
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            NavigationManager.LocationChanged += NavigationManager_LocationChanged;
        }

        private bool _hasLocationChanged = false;

        private void NavigationManager_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            InvokeAsync(async () =>
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
            });
        }

        private async Task EditorOnDidChangeModelContent()
        {
            if (_hasLocationChanged)
            {
                return;
            }

            await OnChange();
        }

        private async Task EditorOnDidInit(MonacoEditorBase editor)
        {
            await LoadFile();
        }

        public async Task LoadFile()
        {
            var file = ZipFileState.Value.ZipEntries[Filename];

            CsCode = file.Content;

            await (_editor?.SetValue(file.Content) ?? Task.Run(() => { }));
        }

        private async Task OnChange()
        {
            var val = await _editor.GetValue();

            Dispatcher.Dispatch(new UpdateZipFileAction { ZipEntry = new ZipEntry { Name = Filename, Content = val } });
        }

        protected override void Dispose(bool disposing)
        {
            NavigationManager.LocationChanged -= NavigationManager_LocationChanged;

            _editor?.DisposeEditor();
            _editor?.Dispose();

            base.Dispose(disposing);
        }
    }
}
