using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppWasm
{

    public class ZipService
    {
        public async Task<List<ZipEntry>> ExtractFiles(Stream fileData)
        {
            await using var ms = new MemoryStream();
            await fileData.CopyToAsync(ms);

            using var archive = new ZipArchive(ms, ZipArchiveMode.Update);

            CleanUpSolution(archive);

            var entries = new List<ZipEntry>();

            foreach (var entry in archive.Entries.Where(x => x.FullName.EndsWith(".cs")))
            {
                await using var fileStream = entry.Open();
                var fileBytes = await fileStream.ReadFully();
                var content = Encoding.UTF8.GetString(fileBytes);

                entries.Add(new ZipEntry { Name = entry.FullName, Content = content });
            }

            return entries;
        }

        private static void CleanUpSolution(ZipArchive archive)
        {
            var foldersToDelete = new[] { "bin", "obj", ".vs", ".git", ".vscode" }
                .Select(x => $"/{x}/")
                .ToArray();

            var entriesToDelete = archive.Entries
                .Where(entry => foldersToDelete.Any(folder => entry.FullName.Contains(folder)))
                .ToList();

            foreach (var entry in entriesToDelete)
            {
                archive
                    .GetEntry(entry.FullName)
                    .Delete();
            }
        }
    }

    public static class StreamExtension
    {
        public static async Task<byte[]> ReadFully(this Stream input)
        {
            await using var ms = new MemoryStream();
            await input.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
