using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;

namespace BlazorZipExplorer.Store.ZipFileUseCase
{
    public record ZipFileState
    {
        public bool IsLoading { get; init; }
        public IReadOnlyDictionary<string, ZipEntry> ZipEntries { get; init; }
    }

    public class Feature : Feature<ZipFileState>
    {
        public override string GetName()
        {
            return "ZipFile";
        }

        protected override ZipFileState GetInitialState()
        {
            return new ZipFileState { IsLoading = false, ZipEntries = null };
        }
    }

    public record DefaultDataAction
    {
    }

    public record UploadZipFileAction
    {
        public Stream File { get; init; }
    }

    public record UploadZipFileResultAction
    {
        public IReadOnlyDictionary<string, ZipEntry> ZipEntries { get; init; }
    }

    public record UpdateZipFileAction
    {
        public ZipEntry ZipEntry { get; init; }
    }

    public static class Reducers
    {
        [ReducerMethod]
        public static ZipFileState ReduceDefaultDataAction(ZipFileState state, DefaultDataAction action)
        {
            return new ZipFileState { IsLoading = true, ZipEntries = null };
        }

        [ReducerMethod]
        public static ZipFileState ReduceUploadZipFileAction(ZipFileState state, UploadZipFileAction action)
        {
            return new ZipFileState { IsLoading = true, ZipEntries = null };
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
