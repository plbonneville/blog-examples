using System;
using System.Net.Http;
using System.Threading.Tasks;
using Fluxor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppWasm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(
                sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<ZipService>();

            builder.Services.AddScoped<IDependencyResolver, BlazorDependencyResolver>();
            builder.Services.AddScoped<CSharpCompilationService>();

            builder.Services.AddFluxor(options => options
                .ScanAssemblies(typeof(Program).Assembly)
            );

            await builder.Build().RunAsync();
        }
    }
}
