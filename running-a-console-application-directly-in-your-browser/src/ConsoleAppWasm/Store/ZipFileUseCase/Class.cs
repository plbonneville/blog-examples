using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;

#nullable enable
namespace ConsoleAppWasm.Store.ZipFileUseCase
{
    public record ZipFileState
    {
        public bool IsLoading { get; init; }
        public IReadOnlyDictionary<string, ZipEntry> ZipEntries { get; init; } = new Dictionary<string, ZipEntry>();
    }

    public class Feature : Feature<ZipFileState>
    {
        public override string GetName()
        {
            return "ZipFile";
        }

        protected override ZipFileState GetInitialState()
        {
            return new ZipFileState { IsLoading = false, ZipEntries = new Dictionary<string, ZipEntry>() };
        }
    }

    public record UploadZipFileAction
    {
        public Stream? File { get; init; }
    }

    public record UploadZipFileResultAction
    {
        public IReadOnlyDictionary<string, ZipEntry> ZipEntries { get; init; } = new Dictionary<string, ZipEntry>();
    }

    public record UpdateZipFileAction(ZipEntry ZipEntry);

    public record CreateZipFileAction();

    public static class Reducers
    {
        [ReducerMethod]
        public static ZipFileState ReduceUploadZipFileAction(ZipFileState state, CreateZipFileAction action)
        {
            return new ZipFileState
            {
                IsLoading = true,
                ZipEntries = new Dictionary<string, ZipEntry>
                {
                    ["Program.cs"] = new ZipEntry
                    {
                        Name = "Program.cs", Content = @"using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
  public static void Main()
  {
    foreach (var i in Fibonacci().Take(20))
    {
      Console.WriteLine(i);
    }
  }

  private static IEnumerable<int> Fibonacci()
  {
    int current = 1, next = 1;

    while (true) 
    {
      yield return current;
      next = current + (current = next);
    }
  }
}
"
                    }
                }
            };
        }

        [ReducerMethod]
        public static ZipFileState ReduceUploadZipFileAction(ZipFileState state, UploadZipFileAction action)
        {
            return new ZipFileState { IsLoading = true, ZipEntries = new Dictionary<string, ZipEntry>() };
        }

        [ReducerMethod]
        public static ZipFileState ReduceUploadZipFileResultAction(ZipFileState state, UploadZipFileResultAction action)
        {
            return new ZipFileState { IsLoading = false, ZipEntries = action.ZipEntries };
        }

        [ReducerMethod]
        public static ZipFileState ReduceUpdateZipFileAction(ZipFileState state, UpdateZipFileAction action)
        {
            var entries = state.ZipEntries
                .ToDictionary(k => k.Key, v => v.Value);

            entries[action.ZipEntry.Name] = action.ZipEntry;

            return new ZipFileState { IsLoading = false, ZipEntries = entries };
        }
    }

    public class Effects
    {
        private readonly ZipService _zipService;

        public Effects(ZipService zipService)
        {
            _zipService = zipService;
        }

        [EffectMethod]
        public async Task HandleUploadZipFileAction(UploadZipFileAction action, IDispatcher dispatcher)
        {
            var files = await _zipService.ExtractFiles(action.File);
            dispatcher.Dispatch(new UploadZipFileResultAction { ZipEntries = files.ToDictionary(x => x.Name, v => v) });
        }
    }
}
