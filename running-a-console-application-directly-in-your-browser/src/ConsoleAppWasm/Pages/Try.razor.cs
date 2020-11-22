using ConsoleAppWasm.Store.CSharpCompilationUseCase;
using ConsoleAppWasm.Store.ZipFileUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace ConsoleAppWasm.Pages
{
    public partial class Try
{
    [Inject]
    private IDispatcher Dispatcher { get; set; }

    [Inject]
    private IState<ZipFileState> ZipFileState { get; set; }

    [Inject]
    private IState<CSharpCompilationState> CSharpCompilationState { get; set; }

    private void Run()
    {
        Dispatcher.Dispatch(new RunAction { Files = ZipFileState.Value.ZipEntries.Values });
    }
}
}
