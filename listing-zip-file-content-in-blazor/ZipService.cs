using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace BlazorZipExplorer
{
    public class ZipService
    {
        public async Task<List<ZipEntry>> ExtractFiles(Stream fileData)
        {
            await using var ms = new MemoryStream();
            await fileData.CopyToAsync(ms);

            using var archive = new ZipArchive(ms);

            var entries = new List<ZipEntry>();

            foreach (var entry in archive.Entries)
            {
                await using var fileStream = entry.Open();
                var fileBytes = await fileStream.ReadFully();
                var content = Encoding.UTF8.GetString(fileBytes);

                entries.Add(new ZipEntry { Name = entry.FullName, Content = content });
            }

            return entries;
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
