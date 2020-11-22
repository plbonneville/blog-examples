using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace ConsoleAppWasm
{
    public interface IDependencyResolver
    {
        Task<List<MetadataReference>> GetAssemblies();
    }

    public class BlazorDependencyResolver : IDependencyResolver
    {
        private readonly HttpClient _http;

        public BlazorDependencyResolver(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<MetadataReference>> GetAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic)
                .Select(x => x.GetName().Name)
                .Union(new[]
                {
                // Add any required dll that are not referenced in the Blazor application
                "System.Console",
                    //"",
                    //""
                })
                .Distinct()
                .Select(x => $"_framework/{x}.dll");

            var references = new List<MetadataReference>();

            foreach (var assembly in assemblies)
            {
                // Download the assembly
                references.Add(
                    MetadataReference.CreateFromStream(
                        await _http.GetStreamAsync(assembly)));
            }

            return references;
        }
    }
}
