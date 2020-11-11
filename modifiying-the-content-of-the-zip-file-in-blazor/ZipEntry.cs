namespace BlazorZipExplorer
{
    public record ZipEntry
    {
        public string Name { get; init; }
        public string Content { get; init; }
    }
}