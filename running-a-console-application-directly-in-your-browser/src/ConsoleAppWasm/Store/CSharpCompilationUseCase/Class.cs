using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;

#nullable enable
namespace ConsoleAppWasm.Store.CSharpCompilationUseCase
{
    public record CSharpCompilationState
    {
        public bool IsRunning { get; init; }
        public string CompilationLogs { get; init; } = "";
        public string Output { get; init; } = "";
    }

    public class Feature : Feature<CSharpCompilationState>
    {
        public override string GetName()
        {
            return "CSharpCompilation";
        }

        protected override CSharpCompilationState GetInitialState()
        {
            return new CSharpCompilationState { IsRunning = false };
        }
    }

    public record LoadAssembliesAction
    {
    }

    public record RunAction
    {
        public IEnumerable<ZipEntry> Files { get; init; } = Array.Empty<ZipEntry>();
    }

    public record RunResultAction
    {
        public string CompilationLogs { get; init; } = "";
        public string Output { get; init; } = "";
    }

    public static class Reducers
    {
        [ReducerMethod]
        public static CSharpCompilationState ReduceRunAction(CSharpCompilationState state, RunAction action)
        {
            return new CSharpCompilationState { IsRunning = true };
        }

        [ReducerMethod]
        public static CSharpCompilationState ReduceRunAction(CSharpCompilationState state, RunResultAction action)
        {
            return new CSharpCompilationState { IsRunning = false, CompilationLogs = action.CompilationLogs, Output = action.Output };
        }

        [ReducerMethod]
        public static CSharpCompilationState ReduceRunAction(CSharpCompilationState state, LoadAssembliesAction action)
        {
            return new CSharpCompilationState { IsRunning = false };
        }
    }

    public class Effects
    {
        private readonly CSharpCompilationService _compilerService;

        public Effects(CSharpCompilationService compilerService)
        {
            _compilerService = compilerService;
        }

        [EffectMethod]
        public async Task HandleUploadZipFileAction(RunAction action, IDispatcher dispatcher)
        {
            var resultAction = new RunResultAction { };

            try
            {
                var csharpFiles = action.Files.ToArray();

                var resultText = await _compilerService.CompileAndRun(csharpFiles);

                resultAction = resultAction with { Output = resultText };
            }
            catch (CSharpCompilationException)
            {
            }
            catch (Exception ex)
            {
                _compilerService.Logs.Add("");
                _compilerService.Logs.Add(ex.Message);
                _compilerService.Logs.Add(ex.ToString());
            }
            finally
            {
                var compileText = string.Join("\r\n", _compilerService.Logs);

                resultAction = resultAction with { CompilationLogs = compileText };
            }

            dispatcher.Dispatch(resultAction);
        }

        [EffectMethod]
        public async Task HandleUploadZipFileAction(LoadAssembliesAction action, IDispatcher dispatcher)
        {
            await _compilerService.Init();
        }
    }
}
