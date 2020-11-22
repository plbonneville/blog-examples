using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ConsoleAppWasm
{
    public class CSharpCompilationService
    {
        private readonly List<MetadataReference> _references = new();
        private readonly IDependencyResolver _dependencyResolver;

        public CSharpCompilationService(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public List<string> Logs { get; } = new();

        public async Task Init()
        {
            if (_references.Any())
            {
                return;
            }

            _references.AddRange(await _dependencyResolver.GetAssemblies());
        }

        public async Task<string> CompileAndRun(params ZipEntry[] csharpFiles)
        {
            Logs.Clear();

            var assembly = await Compile(csharpFiles);

            return Run(assembly);
        }

        public async Task<Assembly> Compile(params ZipEntry[] csharpFiles)
        {
            // Make sure the needed assembly references are available.
            await Init();

            // Convert the C# files into syntax trees.
            var syntaxTrees = csharpFiles
                .Select(x => CSharpSyntaxTree
                .ParseText(x.Content, new CSharpParseOptions(LanguageVersion.Preview), x.Name));

            // Create a new compilation with the source code and the assembly references.
            var compilation = CSharpCompilation.Create(
                "ConsoleAppWasm.Demo",
                syntaxTrees,
                _references,
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            await using var stream = new MemoryStream();

            // Emit the IL for the compiled source code into the stream.
            var result = compilation.Emit(stream);

            foreach (var diagnostic in result.Diagnostics)
            {
                Logs.Add(diagnostic.ToString());
            }

            if (!result.Success)
            {
                Logs.Add("");
                Logs.Add("Build FAILED.");
                throw new CSharpCompilationException();
            }

            Logs.Add("");
            Logs.Add("Build succeeded.");

            // Reset stream to beginning.
            stream.Seek(0, SeekOrigin.Begin);

            // Load the newly created assembly into the current application domain.
            var assembly = AppDomain.CurrentDomain.Load(stream.ToArray());

            return assembly;
        }

        public static string Run(Assembly assembly)
        {
            // Capture the Console outputs.
            using var sw = new StringWriter();
            Console.SetOut(sw);

            var main = assembly.EntryPoint;

            var parameters = main.GetParameters().Any()
                ? new object[] { Array.Empty<string>() }
                : null;

            main.Invoke(null, parameters);

            return sw.ToString();
        }
    }
}
